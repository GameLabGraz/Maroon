using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PasswordUI : MonoBehaviour
{
    public bool isHost;
    
    [SerializeField] private TMP_InputField passwordInput;

    private float _timeScaleRestore = 0;

    private void Start()
    {
        passwordInput.Select();
        _timeScaleRestore = Time.timeScale;
        Time.timeScale = 0;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            OnOkButtonClicked();
        }
    }

    private void OnDisable()
    {
        Time.timeScale = _timeScaleRestore;
    }

    public void OnOkButtonClicked()
    {
        if (isHost)
        {
            MaroonNetworkManager.Instance.Password = passwordInput.text;
            MaroonNetworkManager.Instance.StartHost();
        }
        else
        {
            MaroonNetworkManager.Instance.ClientPassword = passwordInput.text;
            MaroonNetworkManager.Instance.StartClient();
        }
        Destroy(gameObject);
    }
}
