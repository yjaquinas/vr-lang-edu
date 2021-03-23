using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMicVolIndicatorCanvas : MonoBehaviour
{
    private const float VOLUME_MAX = 0.5f;
    private const float VOLUME_3 = 0.1f;
    private const float VOLUME_2 = 0.01f;
    private const float VOLUME_1 = 0.001f;

    [SerializeField] private GameObject[] _volumeIndicators;

    public void VolumeChange(float volume)
    {
        if(volume >= VOLUME_MAX)
        {
            for(int i = 0; i < 4; i++)
            {
                _volumeIndicators[i].SetActive(true);
            }
        }
        else if(volume >= VOLUME_3)
        {
            _volumeIndicators[3].SetActive(false);
            _volumeIndicators[2].SetActive(true);
            _volumeIndicators[1].SetActive(true);
            _volumeIndicators[0].SetActive(true);
        }
        else if(volume >= VOLUME_2)
        {
            _volumeIndicators[3].SetActive(false);
            _volumeIndicators[2].SetActive(false);
            _volumeIndicators[1].SetActive(true);
            _volumeIndicators[0].SetActive(true);
        }
        else if(volume >= VOLUME_1)
        {
            _volumeIndicators[3].SetActive(false);
            _volumeIndicators[2].SetActive(false);
            _volumeIndicators[1].SetActive(false);
            _volumeIndicators[0].SetActive(true);
        }
        else
        {
            for(int i = 0; i < 4; i++)
            {
                _volumeIndicators[i].SetActive(false);
            }
        }
    }
}
