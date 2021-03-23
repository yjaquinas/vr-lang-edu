/*
 * 슬라이딩 도어 센서
 * 콜라이더-트리거 체크시, 도어 오픈
 * 도어 오픈시 - 사운드 이펙트
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CSlidingDoorSensor : MonoBehaviour
{
    [SerializeField] private CSlidingDoor[] _slidingDoors;
    public UnityEvent OnDoorOpen;

    private void OnTriggerEnter(Collider other)
    {
        foreach(CSlidingDoor slidingDoor in _slidingDoors)
        {
            slidingDoor.OnSlideOpen();
            OnDoorOpen.Invoke();
        }
    }
}
