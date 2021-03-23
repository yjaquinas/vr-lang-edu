/*
 * 버튼 클릭시 호출될 이벤트 메소드
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CReviewButton : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void OnReviewButtonClick()
    {
        _audioSource.Play();
    }
}
