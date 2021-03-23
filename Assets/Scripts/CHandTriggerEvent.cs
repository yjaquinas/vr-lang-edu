using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using HTC.UnityPlugin.Vive;

public class CHandTriggerEvent : MonoBehaviour
{

    private bool isFoodTrigger = false;             // 음식에 콜라이더 들어갔을때
    private bool isBellTrigger = false;             // 벨에 콜라이더 들어갔을때
    private bool isMicTrigger = false;              // 마이크에...
    private bool isGarbageTrigger = false;          // 쓰레기통에!

    [SerializeField] private HandRole _handRole;
    [SerializeField] private ControllerButton _btn;
    [SerializeField] private UnityEvent _OnFoodDrop;

    private Transform _itemLocator;


    private GameObject inFoodGameObject;
    private GameObject food;
    private bool haveFood = false;

    private GameObject _bookObject;
    private GameObject _micObject;

    private void Awake()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).name == "ItemLocator")
            {
                _itemLocator = transform.GetChild(i);
            }
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(food == null)
        {
            haveFood = false;
        }

        bool isBtnDown = ViveInput.GetPressDown(_handRole, _btn);

        if(isBtnDown)
        {
            if(haveFood)    // 음식 놓기
            {
                food.transform.parent = null;
                haveFood = false;
                inFoodGameObject = null;
                Rigidbody rigid = food.GetComponent<Rigidbody>();
                Collider collider = food.GetComponent<Collider>();
                rigid.useGravity = true;
                collider.isTrigger = false;
                _OnFoodDrop.Invoke();
            }
            else if(isFoodTrigger && !haveFood)
            {
                // 복제된 음식이면 그냥 줍기
                if(inFoodGameObject.name.Contains("Clone"))
                {
                    food = inFoodGameObject;
                    food.transform.parent = _itemLocator;
                }
                else
                {
                    food = Instantiate(inFoodGameObject, Vector3.zero, Quaternion.identity, _itemLocator);
                }

                food.transform.position = _itemLocator.position;
                food.transform.rotation = _itemLocator.rotation;
                haveFood = true;
                food.GetComponent<Rigidbody>().useGravity = false;
                food.GetComponent<Collider>().isTrigger = true;
            }
            if(isMicTrigger)
            {
                Debug.Log("마이크 누름");
                _micObject.GetComponent<CMic>().SetRecording();
            }



        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Food")
        {
            isFoodTrigger = true;
            inFoodGameObject = other.gameObject;
        }

        if(other.tag == "Bell")
            isBellTrigger = true;

        if(other.tag == "Mic")
        {
            isMicTrigger = true;
            _micObject = other.gameObject;
        }

        if(other.name == "GarbageCan")
        {
            isGarbageTrigger = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Food")
        {
            isFoodTrigger = false;

            inFoodGameObject = other.gameObject;
        }

        if(other.tag == "Bell")
            isBellTrigger = false;

        if(other.tag == "Mic")
        {
            isMicTrigger = false;
        }

        if(other.name == "GarbageCan")
        {
            isGarbageTrigger = false;
        }


    }
}
