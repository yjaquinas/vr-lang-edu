using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool _isOrder;
    int _needBread = 0, _needEgg = 0, _needRice = 0, _needWatermelon = 0;
    bool _isRight;
    public GameObject _customer;
    private CCumstomerController _cc;
    public Transform _customerPos;
    [SerializeField] private GameObject _vivePointers;

    // Use this for initialization
    void Start()
    {
        //NewCustomer();
        _isOrder = false;
    }

    public void NewCustomer()
    {
        GameObject _newCustomer = Instantiate(_customer, _customerPos);
        _cc = _newCustomer.GetComponent<CCumstomerController>();
        _cc._gameManager = this;

        _cc.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
    }

    public void Order(int hamburger, int pizza, int wine)
    {
        //_isOrder = true;
        //_needHam = hamburger;
        //_needPiz = pizza;
        //_needWine = wine;
        //Debug.Log("주문전달됨" + _needHam + _needPiz + _needWine);
        Debug.Log("사용하지않는 메소드 - 영준");
    }

    public bool OrderBell(int bread, int egg, int rice, int watermelon)
    {
        // 벨 누름
        if(_needBread == bread && _needEgg == egg && _needRice == rice && _needWatermelon == watermelon)
        {
            _isRight = true;
        }
        else
        {
            _isRight = false;
        }
        _cc.OrderCheck(_isRight);
        _isOrder = false;

        return _isRight;
    }

    public void TurnOnVivePointers()
    {
        _vivePointers.SetActive(true);
    }
}
