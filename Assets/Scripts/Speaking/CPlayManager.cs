using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 상점에서 주문 받는 화면 관리
public class CPlayManager : MonoBehaviour {

    public enum State { Idle, Request, Ok, No };                         // 상태 enum값

    public State state = State.Idle;                        // 점원의 상태
    private int _myOrderNum;                                // 랜덤 생성될 문장 번호
    public string _guideLine;                      // 발음 해야 할 문장
    public string _meanSpeechString;                        // 발음 해야 할 문장해석
	public string _playerLine;
    [SerializeField] private GameObject[] Foods;            // 햄버거, 피자, 와인 프리팹
    [SerializeField] private BoxCollider _spawnCollider;    // 생성될 콜라이더
    [SerializeField] private CClerkManager _clerkManager;   // 점원

    [SerializeField] private string[] _sentences;
    [SerializeField] private string[] _meanSentences;

    [SerializeField] private GameObject _result;            // 결과 화면

    [SerializeField] private GameObject _cleck;             // 게임 클리어 후 안보이게 하기

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audios;           // 1. 음식 생성, 2. 맞춤, 3. 틀림

    [SerializeField] private GameObject _vivePointers;

	[SerializeField] private Text _speakingText;

	private int thisExp = 5; //성공시 획득할 경험치


    // 주문 할 음식 생성 될 위치
    public Vector3 SpawnPosition()
    {
        Vector3 center = _spawnCollider.bounds.center;
        Vector3 extents = _spawnCollider.bounds.extents;

        //float x = Random.Range(center.x - extents.x, center.x + extents.x);
        //float y = Random.Range(center.y - extents.y, center.y + extents.y);
        //float z = Random.Range(center.z - extents.z, center.z + extents.z);

        float x = center.x;
        float y = center.y;
        float z = center.z;

        return new Vector3(x, y, z);
    }

    // 주문 할 음식 생성 하기
    public void SpawnOrder()
    {
        //Debug.Log("스폰 오더 들어옴");
        _myOrderNum = Random.Range(0, Foods.Length - 1);

		_guideLine = "";

        //InstantSampleFood(_myOrderNum);						// 기존에 샘플로 말할 음식 띄워주기 삭제

        switch (_myOrderNum)
        {
            case 0:
				_meanSpeechString = "새우 한개 주세요";
				_guideLine = "I want one shrimp";
                break;
            case 1:
				_meanSpeechString = "계란 한개 주세요";
				_guideLine = "I want one egg";
                break;
            case 2:
				_meanSpeechString = "밥 한공기 주세요";
				_guideLine = "I want a bowl of rice";
                break;
            case 3:
				_meanSpeechString = "수박 한쪽 주세요";
				_guideLine = "I want a slice of watermelon";
                break;
        }

        _sentences[1] =  _meanSpeechString;
		_meanSentences[1] = _guideLine;
		state = State.Request;                      // 음성 듣기 요청 대기 상태
    }

    public void PlayClear(bool isClear, int playerSentence)
    {
        Invoke("HideClerk", 2f);

		if(isClear)
			InstantSampleFood(_myOrderNum);

		_result.GetComponent<CPlayResultManager>().setSentence(_sentences, true, isClear, playerSentence);
		_result.GetComponent<CPlayResultManager>().setSentence(_meanSentences, false, isClear, playerSentence);
	}

    private void InstantSampleFood(int foodNum)
    {
        _audioSource.clip = _audios[0];
        _audioSource.Play();
        GameObject food;
        food = Instantiate(Foods[foodNum], SpawnPosition(), Quaternion.identity, this.transform);
        Rigidbody rigid = food.GetComponent<Rigidbody>();
        Collider collider = food.GetComponent<Collider>();
        rigid.useGravity = true;
        collider.isTrigger = false;
    }

    private void HideClerk()
    {
        _cleck.SetActive(false);
        _result.SetActive(true);
        _vivePointers.SetActive(true);

		//_result.GetComponent<CPlayResultManager>().setSentence(_sentences, true, true, 1);
		//_result.GetComponent<CPlayResultManager>().setSentence(_meanSentences, false, true, 1);
	}

    // 녹화 받은 음성 string 과 비교
    public void CheckMyVoice(string myVoice)
    {
		_speakingText.text = "";

		bool isClear = false;
        //Debug.Log("returnString : " + myVoice);
        //myVoice = myVoice.Replace("1", "one");            // 1을 one 으로 replace
        myVoice = myVoice.Replace("\"", "");                // 구글에서 "" 따옴표가 같이 들어와서 제거함
        //myVoice = myVoice.Replace(" ", string.Empty);
        Debug.Log("인식된 음성 : " + myVoice);
        //Debug.Log("myVoice.Trim().ToLower()" + myVoice.Trim().ToLower());
        //Debug.Log("_targetSpeechString.ToLower()" + _targetSpeechString.ToLower());
        myVoice = myVoice.Trim().ToLower();

		_speakingText.text = myVoice;

		_playerLine = myVoice;
		isClear = GetComponent<CheckManager>().CheckLine(_playerLine, _guideLine);

		/*
		switch (_myOrderNum)
        {
            case 0:
                if(myVoice == "I want one shrimp")
                {
                    isClear = true;
                }
                break;
            case 1:
                if (myVoice == "I want one eggs")
                {
                    isClear = true;
                }
                break;
            case 2:
                if (myVoice == "I want a bowl of rice")
                {
                    isClear = true;
                }
                break;
            case 3:
                if (myVoice == "I want a slice of watermelon")
                {
                    isClear = true;
                }
                break;
        }
		*/

        if (isClear)
        {
			_speakingText.text = "";
			//GetComponent<CPlayer>().AddExp(thisExp);
			_audioSource.clip = _audios[1];
            _audioSource.Play();
            Debug.Log("음성이 동일함 게임 끝");
            _clerkManager.PlaySound(1);
            state = State.Ok;

            //_result.SetActive(true);
        }
        else
        {
            _audioSource.clip = _audios[2];
            _audioSource.Play();
            Debug.Log("음성이 다름 다시");
            _clerkManager.PlaySound(2);
            state = State.Request;
        }
        
    }

    // Use this for initialization
    void Start () {
        _result.SetActive(false);

    }
	
	public void GoToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void ReStart()
    {
        //SceneManager.LoadScene("PlayScene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
