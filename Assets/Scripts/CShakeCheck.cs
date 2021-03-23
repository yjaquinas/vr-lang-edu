using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CShakeCheck : MonoBehaviour {

	[SerializeField] private float delayTime = 1f;
	[SerializeField] private GameObject _cleckObject;
	[SerializeField] private AudioSource _bellSound;
	private float threshold = 0.1f;
	private Vector3 lastPos;

	// Use this for initializaton
	void Start () {
		lastPos = transform.position;						// 현재 벨의 위치
		StartCoroutine(movingCheck(delayTime));				// 움직임 체크 코루틴 시작
	}

	IEnumerator movingCheck(float delaytime)
	{
		while (true)
		{
			//Instantiate(gameObject, transform.position, Quaternion.identity);
			
			yield return new WaitForSeconds(delaytime);		// 지정된 시간 만큼 
			Vector3 offset = transform.position - lastPos;
			lastPos = transform.position;
			//print("offset.x : " + offset.x);
			//print("offset.y : " + offset.y);
			if(Mathf.Abs(offset.x) > threshold || Mathf.Abs(offset.y) > threshold )
			{
				if (!_bellSound.isPlaying)
					_bellSound.Play();
				print("벨 흔들 거림");
				CallClerk();
			}
		}
	}

	public void CallClerk()
	{
		if (_cleckObject.GetComponent<CClerkManager>()._isMoved)
			return;

		//Debug.Log("책 트리거로 건드림");
		_cleckObject.GetComponent<CClerkManager>().MoveToPlayer();
	}
}
