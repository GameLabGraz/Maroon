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
    private float _startTime;

    private const int CountdownLength = 5;

    public void SetSceneName(string sceneName)
    {
        _sceneName = sceneName;
    }

    private void Start()
    {
        _startTime = Time.time;
    }

    private void Update()
    {
        int secondsPassed = (int) (Time.time - _startTime);
        int timeLeft = CountdownLength - secondsPassed;

        countdownText.text = timeLeft.ToString();
        if (timeLeft <= 0)
        {
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
}
