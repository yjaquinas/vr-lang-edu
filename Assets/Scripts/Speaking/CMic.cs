using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CMic : MonoBehaviour
{

    [SerializeField] private CPlayManager _playManager;
    [SerializeField] private CGCPSTT _cgcpstt;

    private bool _isRecording;
    private bool _waitReturn;                       // 음성 인식 프로그램에서 리턴 오길 기다림

	[SerializeField] GameObject _micTimer;
	[SerializeField] Image _timeBar;
	[SerializeField] Image _micImage;

	[SerializeField] private int _gameDuration; // 게임 시간

	private float _endTime;

	private float _timeRemaining;

	// Use this for initialization
	void Start()
    {
		_micTimer.SetActive(false);
		_waitReturn = false;
        _isRecording = false;                                           // 녹음 중 아님
        //GetComponent<MeshRenderer>().material.color = Color.white;      // 마이크 색상 보통색 
    }

	public void CancelRecording()
	{
		_micTimer.SetActive(false);

		StopCoroutine("TimeCheck");

		_waitReturn = false;
		_cgcpstt.STTRecordQuit();
	}

    public void SetRecording()
    {
        if (_waitReturn)                // 녹음 결과 기다리고 있을때는 눌러도 반응 없도록
            return;

		_waitReturn = true;

		_cgcpstt.OnSTTRecordStart();

		OnTimeStart();
	}

    // Update is called once per frame
    void Update()
    {
        if(_waitReturn)
        {
            if (_cgcpstt._isGCPReady)
            {
                _playManager.CheckMyVoice(_cgcpstt._transcript);
                _waitReturn = false;

				_micTimer.SetActive(false);

				StopCoroutine("TimeCheck");
			}
        }
    }

	// 게임 시작시 처리
	public void OnTimeStart()
	{
		_micTimer.SetActive(true);

		_micImage.enabled = true;

		StopCoroutine("TimeCheck");

		StartCoroutine("TimeCheck");
	}

	IEnumerator TimeCheck()
	{
		// 시간 계산
		_endTime = Time.time + _gameDuration;

		do
		{
			_timeRemaining = _endTime - Time.time;

			_timeBar.fillAmount = _timeRemaining / _gameDuration;

			yield return null;
		}
		while (_timeRemaining > 0);
	}
}
