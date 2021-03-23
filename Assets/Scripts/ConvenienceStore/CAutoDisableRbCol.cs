/*
 * 2초후에 오브젝트의 리지드바디와 콜라이더 비활성화 시켜주기
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAutoDisableRbCol : MonoBehaviour
{
    private Rigidbody _rb;
    private Collider _col;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();

        Invoke("DisableRbCol", 2f);
    }

    private void DisableRbCol()
    {
        _rb.isKinematic = true;
        _col.enabled = false;
    }
}
