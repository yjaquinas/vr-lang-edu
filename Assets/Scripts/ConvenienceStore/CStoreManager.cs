/*
 * 새로 들어온 주문에 맞는 선반 반짝여주기
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStoreManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _shelves;

    public void FlashShelf(int shelfIndex)
    {
        _shelves[shelfIndex].GetComponent<Animator>().SetBool("FlashOutline", true);
    }

    public void DisableFlashShelf()
    {
        for(int i = 0; i < _shelves.Length; i++)
        {
            _shelves[i].GetComponent<Animator>().SetBool("FlashOutline", false);
        }
    }
}
