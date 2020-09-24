using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class SceneChangeCountdown : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private string _sceneName;

    private int _countdownTime = 5;

    public void SetSceneName(string sceneName)
    {
        _sceneName = sceneName;
    }

    private void Start()
    {
        countdownText.text = _countdownTime.ToString();
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
            MaroonNetworkManager.Instance.ServerChangeScene(_sceneName);
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
