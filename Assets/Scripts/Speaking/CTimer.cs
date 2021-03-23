using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CTimer : MonoBehaviour {

	[SerializeField] Image _timeBar;
	[SerializeField] Image _micImage;

	[SerializeField] private int _gameDuration; // 게임 시간

	private float _endTime;

	private float _timeRemaining;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// 게임 시작시 처리
	public void OnTimeStart()
	{
		_micImage.enabled = true;

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
