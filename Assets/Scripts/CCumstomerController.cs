using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CCumstomerController : MonoBehaviour {

	//주문 음성 리스트
	public AudioClip[] _orderSounds;
	//주문 음성 플레이어
	public AudioSource _cVoice;
	//내가 주문한 음식 번호
	public int _myOrderNum;

	public int myHam = 0, myPiz = 0, myWine = 0;

	public GameManager _gameManager;

	//주문할 수 있는 음식 리스트
	enum OrderList
	{
		OneHamburger,
		OnePizza,
		OneWine,
		OneHamburgerOnePizza,
		ThreeHamburger,
		ThreePizzaOneWine,
		OneHamburgerOnePizzaOneWine,
		FiveHamburgerTwoPizza,
		ThreeWine,
		TwoHamburger
	}

	//나의 주문
	OrderList _myOrder;

	private void Start()
	{
		//무작위로 주문음식을 정함
		_myOrderNum = Random.Range(0, 10);
		_myOrder = (OrderList)_myOrderNum;
		SetFoodNum(_myOrderNum);
		//사운드를 해당 음식의 주문 음성으로 설정함
		_cVoice.clip = _orderSounds[_myOrderNum];
	}

	void SetFoodNum(int num)
	{
		switch (num)
		{
			case 0:
				myHam = 1;
				break;
			case 1:
				myPiz = 1;
				break;
			case 2:
				myWine = 1;
				break;
			case 3:
				myHam = 1;
				myPiz = 1;
				break;
			case 4:
				myHam = 1;
				break;
			case 5:
				myPiz = 1;
				myWine = 1;
				break;
			case 6:
				myHam = 1;
				myPiz = 1;
				myWine = 1;
				break;
			case 7:
				myHam = 1;
				myPiz = 1;
				break;
			case 8:
				myWine = 1;
				break;
			case 9:
				myHam = 1;
				break;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		//플레이어와 충돌하면 주문음성을 플레이함
		if(other.gameObject.tag == "Player")
		{
			_cVoice.Play();
			_gameManager.Order(myHam, myPiz, myWine);
		}
	}

	public void OrderCheck(bool isRight)
	{
		//맞는 음식을 받았을 때
		if(isRight)
		{
			Debug.Log("고마워요!");
			_gameManager.NewCustomer();
			Destroy(gameObject);
		}
		//틀린 음식을 받았을 때
		else
		{
			Debug.Log("이거 주문 안했어요");
		}
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
