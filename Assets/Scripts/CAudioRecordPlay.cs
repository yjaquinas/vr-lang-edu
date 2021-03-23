/*
 * 버튼 누르면 리코딩 시작,
 * 리코드 끊으면 애널라이저로 플레이하기
 */
using System.IO;
using UnityEngine;

public class CAudioRecordPlay : MonoBehaviour
{
    private const int RECORD_DURATION = 5;

    private AudioSource _audioSource;

    private bool _micConnected;
    private int _micMinFreq, _micMaxFreq;

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


    // 마이크 녹음 시작을 위한 버튼/트리거/행동 등 어떤 이벤트에서 호출할 메소드
    public void OnRecordStart()
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
            // 녹음 시작, 녹음되는 오디오클립을 참조한 오디오소스에 연결
            _audioSource.clip = Microphone.Start(null, true, RECORD_DURATION, _micMaxFreq);  // Start 한 후로, End할때까지 마지막 n초만 녹음
            Debug.Log("말해주세요");
        }
    }

    public void OnRecordEnd()
    {
        //=====================================================================
        // 녹음 파일 저장
        string recordedFileName = "RecordedFile" + System.DateTime.Now.ToString("yyyyMMdd'-'HHmmss");
        Microphone.End(null);           // 마이크 녹음 끝

        // 파일명에 확장자 안 붙었다면 붙여주기
        if(!recordedFileName.ToLower().EndsWith(".wav"))
        {
            recordedFileName += ".wav";
        }

        string recordedFilePath = Path.Combine("Recorded/", recordedFileName);
        recordedFilePath = Path.Combine(Application.dataPath, recordedFilePath);
        Debug.Log(recordedFilePath);

        // 파일 디렉토리 체크
        Directory.CreateDirectory(Path.GetDirectoryName(recordedFilePath));
        SavWav.Save(recordedFilePath, _audioSource.clip);

        _audioSource.Play();
    }

}
