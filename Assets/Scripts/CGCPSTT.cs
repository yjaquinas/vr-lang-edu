/*
 * 구글 클라우드 플랫폼의 Speech To Text 기능 사용하기
 * 로직:
 *  +------------------+    +----------------------------+    +--------
 *  | microphone check | -> | record voice and save file | -> | 
 *  +------------------+    +----------------------------+    +--------
 *  
 *  
 *  유저의 음성이 없을경우 끝내기
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * Acquired from https://github.com/steelejay/LowkeySpeech                   *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;        // PATH
using System.Net;       // HttpWebRequest
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class CGCPSTT : MonoBehaviour
{
    private const float MIC_INPUT_VOLUMNE_THRESHOLD = 0.01f;            // 노이즈보다 높게,, 목소리보단 낮게
    private const float MIC_INPUT_TIME_THRESHOLD = 1f;                  // 목소리 없기 시작 1초후 녹음 종료

    [SerializeField] private string _GoogleCloudPlatformAPIKey; // 영준 GCP api key: AIzaSyB5MuxxWD6DnsMy05gOgICOzrRimAqhTW8

    private HttpWebRequest _httpWebRequest;

    private string _recordedFilePath;

    public AudioSource _audioSource;

    private bool _micConnected = false;
    private int _micMinFreq, _micMaxFreq;

    private string _GCPResponse = "";
    public bool _isGCPReady = false;
    public string _transcript = "";
    public string _confidence = "";
    private bool _isRecording = false;
    private bool _isVoiceInputStarted = false;

    private float _micInputVolume;
    int _sampleWindow = 128;
    AudioClip _clipRecord;
    private float _micSilenceTimer = 0;
    [SerializeField] private CMicVolIndicatorCanvas _micVolIndicatorCanvas;

    public UnityEvent OnRecordEnd;

    // 오디오소스 참조
    // 마이크 유무 체크
    // 마이크 주파수 세팅
    // 버튼 초기화
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        // 마이크 있나 체크
        if(Microphone.devices.Length <= 0)
        {
            Debug.Log("마이크가 없어요!");
            _micConnected = false;  // 마이크 플래그 리셋
        }
        else
        {
            _micConnected = true;   // 마이크 플래그 셋
            Microphone.GetDeviceCaps(null, out _micMinFreq, out _micMaxFreq);   // 마이크 스펙 (주파수) 가져오기

            // UnityEngine의 Microphone의 다큐멘테이션에 의하면 최소/최대 주파수가 0으로 뜨면, 모든 주파수 허용이라함
            if(_micMinFreq == 0 && _micMaxFreq == 0)
            {
                //...meaning 44100 Hz can be used as the recording sampling rate
                _micMaxFreq = 44100;    // 모든 주파수 허용이니까 44.1 KHz 쓰자
            }
        }
    }

    private void Update()
    {

        // levelMax equals to the highest normalized value power 2, a small number because < 1
        // pass the value to a static var so we can access it from anywhere
        _micInputVolume = LevelMax();

        if(_micVolIndicatorCanvas)
            _micVolIndicatorCanvas.VolumeChange(_micInputVolume);

        if(_micInputVolume >= MIC_INPUT_VOLUMNE_THRESHOLD)
        {
            _isVoiceInputStarted = true;
        }

        if(_isVoiceInputStarted)
        {
            if(_micSilenceTimer < MIC_INPUT_TIME_THRESHOLD)
            {
                _micSilenceTimer += Time.deltaTime;
            }
            if(_micInputVolume > MIC_INPUT_VOLUMNE_THRESHOLD)
            {
                _micSilenceTimer = 0;
            }
        }


        if(_isRecording && _micInputVolume < MIC_INPUT_VOLUMNE_THRESHOLD && _micSilenceTimer >= MIC_INPUT_TIME_THRESHOLD)
        {
            OnSTTRecordEnd();
            if(_micVolIndicatorCanvas != null) _micVolIndicatorCanvas.gameObject.SetActive(false);
            _isRecording = false;
            OnRecordEnd.Invoke();
        }

    }

    // 마이크 녹음 시작을 위한 버튼/트리거/행동 등 어떤 이벤트에서 호출할 메소드
    public void OnSTTRecordStart()
    {
        // 마이크 없으면 gg
        if(!_micConnected)
        {
            Debug.Log("마이크가 없어요");
            return;
        }

        // 마이크 녹음 중 아니면 녹음 시작
        // deviceName의 매개변수 "" 이나 null을 사용하면 디폴트 마이크 사용
        if(!Microphone.IsRecording(null))
        {
            _isGCPReady = false;    // 녹음 시작시 받아쓰기 준비안됨으로 리셋
            _transcript = _confidence = ""; // 받아쓰기, 일치율 리셋
            // 녹음 시작, 녹음되는 오디오클립을 참조한 오디오소스에 연결
            _audioSource.clip = Microphone.Start(null, true, 10, _micMaxFreq);  // Start 한 후로, End할때까지 마지막 n초만 녹음
            _clipRecord = _audioSource.clip;
            _isRecording = true;
            _micSilenceTimer = 0;
            _isVoiceInputStarted = false;
            if(_micVolIndicatorCanvas != null) _micVolIndicatorCanvas.gameObject.SetActive(true);
            //Debug.Log("말해주세요");
        }
    }

    public void STTRecordQuit()
    {
        Microphone.End(null);           // 마이크 녹음 끝
    }

    // 마이크 녹음 끝을 위한 버튼/트리거/행동 등 어떤 이벤트에서 호출할 메소드
    // 녹음 파일 임시 저장
    // 구글 클라우드 플랫폼에 전송
    //public string OnSTTRecordEnd()
    public void OnSTTRecordEnd(int i)
    {
        OnSTTRecordEnd();
    }

    public void OnSTTRecordEnd()
    {

        Microphone.End(null);           // 마이크 녹음 끝
        //=====================================================================
        // 녹음 파일 저장
        string recordedFileName = "RecordedFile" + DateTime.Now.ToString("yyyyMMddHHmmss");
        // 파일명에 확장자 안 붙었다면 붙여주기
        if(!recordedFileName.ToLower().EndsWith(".wav"))
        {
            recordedFileName += ".wav";
        }
        _recordedFilePath = Path.Combine("recorded/", recordedFileName);
        _recordedFilePath = Path.Combine(Application.dataPath, _recordedFilePath);

        // 파일 디렉토리 체크
        Directory.CreateDirectory(Path.GetDirectoryName(_recordedFilePath));
        SavWav.Save(_recordedFilePath, _audioSource.clip);

        //=====================================================================
        // 구글 클라우드 플랫폼에 전송, 받아쓰기 한 내용 출력
        string apiURL = "https://speech.googleapis.com/v1/speech:recognize?&key=" + _GoogleCloudPlatformAPIKey; // 구글 클라우드 플랫폼 api키

        _GCPResponse = HttpUploadFile(apiURL, _recordedFilePath, "file", "audio/wav; rate=" + _micMaxFreq);

    }


    public string HttpUploadFile(string url, string file, string paramName, string contentType)
    {
        ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
        //Debug.Log(string.Format("Uploading {0} to {1}", file, url));

        Byte[] bytes = File.ReadAllBytes(file);
        String file64 = Convert.ToBase64String(bytes,
                                         Base64FormattingOptions.None);

        //Debug.Log(file64);
        File.Delete(_recordedFilePath);      // 임시저장된 녹음 파일 삭제

        try
        {

            _httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            _httpWebRequest.ContentType = "application/json";
            _httpWebRequest.Method = "POST";

            using(var streamWriter = new StreamWriter(_httpWebRequest.GetRequestStream()))
            {
                string json = "{ \"config\": { \"languageCode\" : \"en-US\" }, \"audio\" : { \"content\" : \"" + file64 + "\"}}";
                //string json = "{ \"config\": { \"languageCode\" : \"ko-KR\" }, \"audio\" : { \"content\" : \"" + file64 + "\"}}";

                //Debug.Log(json);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            //var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            _httpWebRequest.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);

        }
        catch(WebException ex)
        {
            var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
            Debug.Log(resp);
        }


        return "empty";

    }


    void FinishWebRequest(IAsyncResult asyncResult)
    {
        var httpResponse = _httpWebRequest.EndGetResponse(asyncResult);


        //Debug.Log(httpResponse);
        string asyncResultString = "";
        using(var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            asyncResultString = streamReader.ReadToEnd();
        }

        var jsonResponse = SimpleJSON.JSON.Parse(asyncResultString);

        Debug.Log("구글의 응답: " + asyncResultString);
        Debug.Log("JSON Parsed: " + jsonResponse);

        if(jsonResponse != null)
        {
            //=====================================================================
            // 받아쓰기 내용 저장
            _transcript = jsonResponse["results"][0]["alternatives"][0]["transcript"].ToString().Replace("\"", "");
            _confidence = jsonResponse["results"][0]["alternatives"][0]["confidence"].ToString().Replace("\"", "");
            Debug.Log("transcript : " + _transcript);
            Debug.Log("confidence : " + _confidence);
        }
        _isGCPReady = true;     // 구글에서 답이오면 true로 셋
                                // 음성에서 아무것도 parse하지 못했으면 "" 빈 string으로 그대로
                                //_audioSource.Play();
                                //=====================================================================
                                // 임시 파일 정리 (삭제)

        Debug.Log("구글이 이해한 유저의 말: "+_transcript);
        Debug.Log("정확도 (0 ~ 1): "+_confidence);

        ////=====================================================================
        //// 녹음 파일 파일명 저장
        //int _recordCount = PlayerPrefs.GetInt("RECORD_COUNT", 0);
        //PlayerPrefs.SetString("RECORD_" + _recordCount.ToString(), _recordedFilePath);
        //_recordCount++;
        //PlayerPrefs.SetInt("RECORD_COUNT", _recordCount);
        //PlayerPrefs.Save();

    }


    //get data from microphone into audioclip
    private float LevelMax()
    {
        if(_clipRecord == null)
            return 0;

        float levelMax = 0;
        float[] waveData = new float[_sampleWindow];
        int micPosition = Microphone.GetPosition(null) - (_sampleWindow + 1); // null means the first microphone
        if(micPosition < 0) return 0;
        _clipRecord.GetData(waveData, micPosition);
        // Getting a peak on the last 128 samples
        for(int i = 0; i < _sampleWindow; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if(levelMax < wavePeak)
            {
                levelMax = wavePeak;
            }
        }
        return levelMax;
    }

}
