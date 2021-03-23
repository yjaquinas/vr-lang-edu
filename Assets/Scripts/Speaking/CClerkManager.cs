using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class CClerkManager : MonoBehaviour {

    [SerializeField] private GameObject _player;			// 플레이어
    [SerializeField] private CPlayManager _playManager;		// 플레이매니저
	[SerializeField] private CMic _CmicManager;				// 마이크 매니저
    [SerializeField] private Text descText;					// 힌트용 text
	[SerializeField] private Text targetText;               // 말해야 하는 text 모국어
	[SerializeField] private Text speakingText;             // 내가 말한 영어
	[SerializeField] private int _waitForTime;				// 사용자 얼마나 대기할지

	private Animator _animator;
    private Rigidbody _rigidbody;

    private float moveFloat;            // 이동 float

    public bool _isMove;                // 이동 하였는가
	public bool _isMoved;				// 움직임 완료

    //주문 음성 리스트
    public AudioClip[] _clerkSounds;    // 1. 뭐드실? 2. 주문 알겠음 3. 무슨말이신지? 

    //주문 음성 플레이어
    public AudioSource _cVoice;

    private Vector3 _playerPosition;    // z 값만 추려내기 위해 만든 변수
    private int _questionCount;         // 3번까지 질문 후 다음으로 넘어감
	private int _waitTime;

    // 플레이어에게 오도록
    public void MoveToPlayer()
    {
        Debug.Log("MoveToPlayer Enter");
        _isMove = true;
    }

    private void Awake()
    {
		_animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    // 지정된 사운드 재생
    public void PlaySound(int playNum)
    {
        if(_cVoice.clip != _clerkSounds[playNum])
            _cVoice.clip = _clerkSounds[playNum];

		if (_questionCount == 1)
		{
			descText.text = StringToUnderLine(_playManager._guideLine);
		}
		else if (_questionCount == 2)
		{
			descText.text = _playManager._guideLine;
		}
		else if (_questionCount >= 4)
		{
			descText.text = "";
			speakingText.text = "";
			targetText.text = "";
			_playManager.GetComponent<CPlayManager>().PlayClear(false, 1);
			CancelInvoke();

			return;
		}

		if (!_cVoice.isPlaying)
			_cVoice.Play();

		//playNum = 1;

		CancelInvoke();

		if (playNum == 0)										// 무엇을 주문하시겠습니까 물어보고
        {
			Invoke("RecordingStart", 2f);						// 사용자 음성 인식 시작

			//_playManager.SpawnOrder();						// 주문 요청 음성 내보내고 요청할 음식 Spawn
			targetText.text = _playManager._meanSpeechString;   // 말해야 될 내용 표시

			Invoke("WaitForUser", _waitForTime);				// 10초간 사용자의 음성 대기

			if (_questionCount > 1)
				_animator.SetTrigger("No");
		}
		else if(playNum == 1)                                   // 정상적으로 주문을 완료 했을때
		{
            descText.text = "";
			speakingText.text = "";
			targetText.text = "";
			_animator.SetTrigger("Yes");
            _playManager.GetComponent<CPlayManager>().PlayClear(true, 1);
		}
        else if (playNum == 2)									// 말한 내용을 듣지 못했을때
        {
			//_CmicManager.SetRecording();						// 사용자 음성 인식 시작
			Invoke("RecordingStart", 2f);                       // 사용자 음성 인식 시작

			Invoke("WaitForUser", _waitForTime);							// 10초간 사용자의 음성 대기

			_animator.SetTrigger("No");
        }

		_questionCount++;
	}
	
	private void RecordingStart()
	{
		_CmicManager.SetRecording();
	}

    // Use this for initialization
    void Start () {
        _playerPosition = new Vector3(_player.transform.position.x, 0, 0);

        transform.LookAt(_playerPosition);

        descText.text = "";
		targetText.text = "";
		speakingText.text = "";
	}
	
	// Update is called once per frame
	void Update () {
        if(_isMove)
        {
            if(_animator.GetBool("Walking") == false)
            {
                _animator.SetBool("Walking", true);
            }

            moveFloat = Time.deltaTime * 0.1f;
            this.transform.position = Vector3.Lerp(this.transform.position, _playerPosition, moveFloat);
        }
        else
        {
            if (_animator.GetBool("Walking") == true)
                _animator.SetBool("Walking", false);
        }
    }

    // 플레이어 근처 위치에 닿으면 정지
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "StopPoint")
        {
			_playManager.SpawnOrder();                          // 주문 요청 음성 내보내고 요청할 음식 Spawn

			_isMove = false;

			_isMoved = true;

			PlaySound(0);                           // 주문 음성 재생
		}
    }

	private void WaitForUser()
	{
		_CmicManager.CancelRecording();
		_waitTime = 0;
		CancelInvoke();
		PlaySound(0);
	}

    private string StringToUnderLine(string str)
    {
        string returnStr = "";

        if (str == null || str.Length == 0)
            return returnStr;

        char[] charArray = str.ToCharArray();

        for(int i = 0; i < charArray.Length; i++)
        {
            if(char.IsWhiteSpace(charArray[i]))
            {
                returnStr = returnStr.Insert(returnStr.Length, " ");
            }
            else
            {
                returnStr = returnStr.Insert(returnStr.Length, "_");
            }
        }

        return returnStr;
    }
}
