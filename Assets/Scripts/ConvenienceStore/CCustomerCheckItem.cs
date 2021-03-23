/*
 * 선반 위치를 들은 손님이, 해당 선반 앞에 가서,
 * 선반에 자신이 원한 아이템이 존재하는지 체크
 * 
 * 걸어오면서 선반 지나갔다고 체크되면 안되니까, 선반 도착했을때만 체크하기
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCustomerCheckItem : MonoBehaviour
{
    [SerializeField] private Collider _checkItemCollider;
    private CCustomerOrder.ORDER_ITEM _targetItem;
    private bool _isItemOnShelf;
    public bool IsItemOnShelf { get { return _isItemOnShelf; } }
    private CCustomerMove _customerMove;

    private void Awake()
    {
        _isItemOnShelf = false;
    }

    // 본 손님의 타겟 아이템 이름
    private void Start()
    {
        _targetItem = GetComponent<CCustomerOrder>().OrderItem;
        _customerMove = GetComponent<CCustomerMove>();
        _checkItemCollider.enabled = false;
    }

    private void Update()
    {
        if(_customerMove.FSMstate == 3)
        {
            _checkItemCollider.enabled = true;
        }
        else
        {
            _checkItemCollider.enabled = false;
        }
    }


    // 선반위에 음식 콜라이더 한개씩만 띄워주기
    // 전부 다 체크하면 trigger너무 많이 떠서 물리 계산 무거워짐.....
    // 그리고 이 메소드가 많이 불려서 쓸데없는 연산이 됨 ㅠ
    // ORDER_ITEM이 enum이니까, 콜라이더 이름에 번호를 붙여줍시다!
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ShelfItemCollider"))
        {
            //Debug.Log("other.name: " + other.name);
            //Debug.Log("target name: item" + ((int)_targetItem).ToString());
            if(other.name == "item" + ((int)_targetItem).ToString())
            {
                _isItemOnShelf = true;
                Debug.Log("target name: item" + ((int)_targetItem).ToString() + " found!");
            }
        }
    }
}
