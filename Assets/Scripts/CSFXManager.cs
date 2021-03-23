using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSFXManager : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _buttonAudioClip;
    [SerializeField] private AudioClip _foodDropClip;
    [SerializeField] private AudioClip _okClip;
    [SerializeField] private AudioClip _dingDongClip;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void OnButtonClick()
    {
        _audioSource.clip = _buttonAudioClip;
        _audioSource.volume = 0.2f;
        _audioSource.Play();
    }

    public void OnFoodDrop()
    {
        _audioSource.clip = _foodDropClip;
        _audioSource.volume = 1f;
        _audioSource.Play();
    }

    public void OnOk()
    {
        _audioSource.clip = _okClip;
        _audioSource.volume = 0.2f;
        _audioSource.Play();
    }

    public void PlayDingDong()
    {
        _audioSource.clip = _dingDongClip;
        _audioSource.volume = 1f;
        _audioSource.Play();
    }
}
