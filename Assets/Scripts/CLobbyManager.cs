/*
 * 매장 캔버스 띄우기, 숨기기
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject _lobbyMenuCanvas;
    [SerializeField] private GameObject _storeSelectionCanvas;

    private void Awake()
    {
        _lobbyMenuCanvas.SetActive(true);
        _storeSelectionCanvas.SetActive(false);
    }

    public void OnStoreSelectionButtonClick()
    {
        _lobbyMenuCanvas.SetActive(false);
        _storeSelectionCanvas.SetActive(true);
    }

    public void OnMainMenuButtonClick()
    {
        _lobbyMenuCanvas.SetActive(true);
        _storeSelectionCanvas.SetActive(false);
    }
}
