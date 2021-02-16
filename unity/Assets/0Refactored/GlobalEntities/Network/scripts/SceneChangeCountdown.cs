using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneChangeCountdown : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    
    [SerializeField] private TextMeshProUGUI cancelText;

    private string _sceneName;

    private int _countdownTime = 5;

    public void SetSceneName(string sceneName)
    {
        _sceneName = sceneName;
    }

    private void Start()
    {
        Maroon.NetworkManager.Instance.SceneCountdownActive = true;
        countdownText.text = _countdownTime.ToString();
        if (!Maroon.NetworkManager.Instance.IsInControl)
        {
            cancelText.gameObject.SetActive(false);
            GetComponentInChildren<Button>().interactable = false;
        }
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        while (_countdownTime >= 0)
        {
            yield return new WaitForSecondsRealtime(1);
            _countdownTime--;
            countdownText.text = _countdownTime.ToString();
        }
        if (isServer)
        {
            Maroon.NetworkManager.Instance.ServerChangeScene(_sceneName);
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void OnCancel()
    {
        CmdStopCountdown();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Maroon.NetworkManager.Instance.SceneCountdownActive = false;
    }

    [Command(ignoreAuthority = true)]
    private void CmdStopCountdown()
    {
        Destroy(gameObject);
    }
}
