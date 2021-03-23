using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //UI
    public GameObject startUI;
    public GameObject practiceUI;
    public GameObject resultUI;
    //가이드 보이스
    public AudioSource voice;
    //가이드 보이스 리스트
    private AudioClip[] _clips;
    public AudioClip[] _audioClipNumbers;
    public AudioClip[] _audioClipFoods;
    public AudioClip[] _audioClipConvenienceStore;
    public AudioClip[] _audioClipRestaurant;
    //가이드 대사
    public Text guideLine;
    public Text koreanLine;
    //플레이어 대사
    public Text playerLine;
    public CGCPSTT _stt;
    //텍스트
    public Text startGuideText;
    public Text resultText;
    //녹음으로 받아온 대사
    private string _scripts;
    private string _currentScripts;
    //다음 버튼
    public Button _nextButton;
    //녹음 버튼
    public Button _recordButton;
    //녹음중지 버튼
    public Button _stopButton;
    //아날라이즈
    public GameObject analyzerline;
    public GameObject playerAnalyzerline;
    public CAudioAnalyzer analyzer;
    public CAudioAnalyzer playerAnalyzer;
    //현재 
    private bool _isNotListen = true;
    //연습문장들을 담아놓는 문장배열
    private string[] nextLines;
    private string[] koreanLines;
    private int count = 0;
    //실패 횟수
    private int failCount = 0;
    bool getStarted = false;
    //성공여부
    bool[] isPassed;
    //패널
    public GameObject guidePanel;
    public GameObject playerPanel;

    //플레이어의 상태
    enum PlayerState
    {
        Break,
        PracticeUIGuide,
        PracticeUIPlayer,
        End
    }

    PlayerState myState = PlayerState.Break;

    //문장배열 초기화
    //=========================================================================================
    private void SetNumberLines()
    {
        nextLines[0] = "One";
        nextLines[1] = "Two";
        nextLines[2] = "Three";
        nextLines[3] = "Four";
        nextLines[4] = "Five";
    }
    private void SetKoreanNumberLines()
    {
        koreanLines[0] = "일 (1)";
        koreanLines[1] = "이 (2)";
        koreanLines[2] = "삼 (3)";
        koreanLines[3] = "사 (4)";
        koreanLines[4] = "오 (5)";
    }

    private void SetFoodLines()
    {
        nextLines[0] = "Egg";
        nextLines[1] = "Rice";
        nextLines[2] = "Shrimp";
        nextLines[3] = "Watermelon";
    }
    private void SetKoreanFoodLines()
    {
        koreanLines[0] = "계란";
        koreanLines[1] = "밥";
        koreanLines[2] = "새우";
        koreanLines[3] = "수박";
    }

    private void SetRestaurantLines()
    {
        nextLines[0] = "I want a bowl of rice";
        nextLines[1] = "I want a slice of watermelon";
        nextLines[2] = "I want one egg";
        nextLines[3] = "I want one shrimp";
    }
    private void SetKoreanRestaurantLines()
    {
        koreanLines[0] = "밥 한공기 주세요";
        koreanLines[1] = "수박 한쪽 주세요";
        koreanLines[2] = "계란 한개 주세요";
        koreanLines[3] = "새우 한개 주세요";
    }

    private void SetConvenienceStoreLines()
    {
        nextLines[0] = "That is on the shelf number one";
    }
    private void SetKoreanConvenienceStoreLines()
    {
        koreanLines[0] = "그것은 1번 선반에 있습니다";
    }

    public void SetWordPracticeNumbers()
    {
        _clips = _audioClipNumbers;
        nextLines = new string[_clips.Length];
        koreanLines = new string[_clips.Length];
        isPassed = new bool[_clips.Length];

        SetNumberLines();
        SetKoreanNumberLines();

        guideLine.text = nextLines[0];
        koreanLine.text = koreanLines[0];
        voice.clip = _clips[0];
    }

    public void SetWordPracticeFoods()
    {
        _clips = _audioClipFoods;
        nextLines = new string[_clips.Length];
        koreanLines = new string[_clips.Length];
        isPassed = new bool[_clips.Length];

        SetFoodLines();
        SetKoreanFoodLines();

        guideLine.text = nextLines[0];
        koreanLine.text = koreanLines[0];
        voice.clip = _clips[0];
    }

    public void SetSentencePracticeConvenienceStore()
    {
        _clips = _audioClipConvenienceStore;
        nextLines = new string[_clips.Length];
        koreanLines = new string[_clips.Length];
        isPassed = new bool[_clips.Length];

        SetConvenienceStoreLines();
        SetKoreanConvenienceStoreLines();

        guideLine.text = nextLines[0];
        koreanLine.text = koreanLines[0];
        voice.clip = _clips[0];
    }

    public void SetSentencePracticeRestaurant()
    {
        _clips = _audioClipRestaurant;
        nextLines = new string[_clips.Length];
        koreanLines = new string[_clips.Length];
        isPassed = new bool[_clips.Length];

        SetRestaurantLines();
        SetKoreanRestaurantLines();

        guideLine.text = nextLines[0];
        koreanLine.text = koreanLines[0];
        voice.clip = _clips[0];
    }

    private void Awake()
    {
        _clips = _audioClipNumbers;
        nextLines = new string[_clips.Length];
        koreanLines = new string[_clips.Length];
        isPassed = new bool[_clips.Length];

        SetNumberLines();
        SetKoreanNumberLines();
    }

    private void Start()
    {
        StartCoroutine("Blink");
    }

    //시작가이드텍스트를 깜빡임
    IEnumerator Blink()
    {
        while(!getStarted)
        {
            startGuideText.enabled = false;
            yield return new WaitForSeconds(0.5f);
            startGuideText.enabled = true;
            yield return new WaitForSeconds(0.5f);
        }
    }

    //시작을 클릭하면 연습UI를 활성화
    public void ClickStart()
    {
        StopCoroutine("Bilnk");
        myState = PlayerState.PracticeUIGuide;
        StartCoroutine("HighlightGuide");
        getStarted = true;
        startUI.SetActive(false);
        practiceUI.SetActive(true);
    }


    //듣기를 클릭하면 Listen()을 호출
    public void ClickListen()
    {
        if(_isNotListen)
        {
            guidePanel.GetComponent<Image>().sprite = Resources.Load("WhiteBack", typeof(Sprite)) as Sprite;
            StopCoroutine("HighlightGuide");
            myState = PlayerState.Break;
            _isNotListen = false;
            analyzer._lineRenderer.positionCount = 0;
            StartCoroutine("Listen");
        }
    }

    //가이드 보이스가 재생된다.
    IEnumerator Listen()
    {
        voice.Play();
        analyzerline.SetActive(true);
        guideLine.color = Color.green;
        yield return new WaitForSeconds(voice.clip.length);
        guideLine.color = Color.white;
        _isNotListen = true;
        myState = PlayerState.PracticeUIPlayer;
        StartCoroutine("HighlightPlayer");
    }

    //녹음을 시작하면 아날라이저들을 숨긴다.
    public void HideLine()
    {
        playerAnalyzer._lineRenderer.positionCount = 0;
        playerAnalyzerline.gameObject.SetActive(false);
        playerPanel.GetComponent<Image>().sprite = Resources.Load("WhiteBack", typeof(Sprite)) as Sprite;
        StopCoroutine("HighlightPlayer");
        myState = PlayerState.Break;
        _stopButton.gameObject.SetActive(true);
        _recordButton.gameObject.SetActive(false);
    }

    //녹음을 끝냄
    public void StopRecord()
    {
        _stopButton.gameObject.SetActive(false);
        _recordButton.gameObject.SetActive(true);
        _currentScripts = _scripts = "null";
    }

    //보이스가 새로 녹음되면 Check()를 호출함
    private void Update()
    {
        _scripts = _stt._transcript;
        if(_scripts != "" && _currentScripts != _scripts)
        {
            playerAnalyzerline.GetComponent<AudioSource>().clip = _stt._audioSource.clip;
            playerAnalyzerline.SetActive(true);
            Check(_scripts);
            _currentScripts = _scripts;
        }
    }

    //받아온 보이스의 스크립트와 원래 스크립트가 일치하는지 확인해서
    //일치할시 다음 줄로 넘어감
    public void Check(string transcript)
    {
        playerLine.text = transcript.ToLower().Replace("\"", "");
        playerLine.gameObject.SetActive(true);

        bool isRight = GetComponent<CheckManager>().CheckLine(playerLine.text, guideLine.text);

        /*
        //================================================================================
        // VR캠프 한국어 테스트용 띄어쓰기 무시
        string playerLineNoSpaces = playerLine.text.ToString().ToLower().Replace(" ", string.Empty);
        string guideLineNoSpaces = guideLine.text.ToString().ToLower().Replace(" ", string.Empty);

        //if(playerLine.text == guideLine.text.ToString().ToLower())
		*/
        if(isRight)
        //================================================================================
        {
            isPassed[count] = true;
            playerLine.color = Color.cyan;
            _nextButton.interactable = true;
        }
        else
        {
            failCount++;
            if(failCount == 3)
            {
                isPassed[count] = false;
                playerLine.color = Color.red;
                //playerLine.text += "실패";
                _nextButton.interactable = true;
            }
            else
            {
                playerLine.color = Color.red;
            }
        }

    }

    //다음 줄로 이동
    public void NextLine()
    {
        playerAnalyzer._lineRenderer.positionCount = 0;
        playerAnalyzerline.gameObject.SetActive(false);
        failCount = 0;
        count++;
        playerLine.gameObject.SetActive(false);
        analyzer.gameObject.SetActive(false);
        _nextButton.interactable = false;
        if(count == nextLines.Length)
        {
            myState = PlayerState.Break;
            koreanLine.text = "";
            practiceUI.SetActive(false);
            resultUI.SetActive(true);
            Result();
        }
        else
        {
            myState = PlayerState.PracticeUIGuide;
            StartCoroutine("HighlightGuide");
            guideLine.text = nextLines[count];
            koreanLine.text = koreanLines[count];
            voice.clip = _clips[count];
        }
    }

    // 스크롤뷰 scrollview 로 변환하기!
    void Result()
    {
        GetComponent<CPlayer>().AddExp(1);
        for(int i = 0; i < nextLines.Length; i++)
        {
            if(isPassed[i])
            {
                resultText.text += "<color=blue>" + nextLines[i] + "</color> " /*+ isPassed[i]*/ + "\n\n";
            }
            else
            {
                resultText.text += "<color=red>" + nextLines[i] + "</color> " /*+ isPassed[i]*/ + "\n\n";
            }
        }
    }

    //씬을 재시작
    public void Reply()
    {
        SceneManager.LoadScene("Practice");
    }

    IEnumerator HighlightGuide()
    {
        while(myState == PlayerState.PracticeUIGuide)
        {
            guidePanel.GetComponent<Image>().sprite = Resources.Load("MintBack", typeof(Sprite)) as Sprite;
            yield return new WaitForSeconds(0.5f);
            guidePanel.GetComponent<Image>().sprite = Resources.Load("WhiteBack", typeof(Sprite)) as Sprite;
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator HighlightPlayer()
    {
        while(myState == PlayerState.PracticeUIPlayer)
        {
            playerPanel.GetComponent<Image>().sprite = Resources.Load("MintBack", typeof(Sprite)) as Sprite;
            yield return new WaitForSeconds(0.5f);
            playerPanel.GetComponent<Image>().sprite = Resources.Load("WhiteBack", typeof(Sprite)) as Sprite;
            yield return new WaitForSeconds(0.5f);
        }
    }

    /*
	public void ClickPractice()
	{
		for(int i = 0; i<lines.Length; i++)
		{
			bool sendVoice = false;
			lines[i].color = Color.black;
			if (Input.GetAxis("Fire1")>0)
			{
				print("Speak Please");
			}
		}
	}
	*/

}
