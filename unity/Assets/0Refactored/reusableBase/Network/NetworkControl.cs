using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkControl : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI inControlText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private Button controlButton;
    
    private enum Mode
    {
        InControl,
        InControlRequest,
        NotInControl,
        NotInControlMyRequest,
        NotInControlOtherRequest
    }

    private Mode _mode;
    private string _clientWithRequest;

    private const int CountdownDuration = 5;
    private int _countdownTime;
    private bool _countdownActive;
    
    // Start is called before the first frame update
    void Start()
    {
        controlButton.onClick.AddListener(OnClickCommandButton);
        MaroonNetworkManager.Instance.newClientInControlEvent.AddListener(OnNewClientInControl);
        if (isServer)
        {
            if (MaroonNetworkManager.Instance.ClientInControl == null)
            {
                MaroonNetworkManager.Instance.ServerSetClientInControl(MaroonNetworkManager.Instance.PlayerName);
            }
        }

        if (MaroonNetworkManager.Instance.IsInControl)
        {
            _mode = Mode.InControl;
        }
        else
        {
            _mode = Mode.NotInControl;
        }
        UpdateAppearance(); //init
    }

    private void OnNewClientInControl()
    {
        if (MaroonNetworkManager.Instance.IsInControl)
        {
            _mode = Mode.InControl;
        }
        else
        {
            _mode = Mode.NotInControl;
        }
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        switch (_mode)
        {
            case Mode.InControl:
                inControlText.text = "IN CONTROL";
                statusText.gameObject.SetActive(false);
                countdownText.gameObject.SetActive(false);
                controlButton.interactable = false;
                controlButton.GetComponent<Image>().color = new Color(0.5f,1, 0.5f, 0.6f);
                break;
            
            case Mode.InControlRequest:
                inControlText.text = _clientWithRequest + " requests control";
                statusText.text = "Click to deny";
                statusText.gameObject.SetActive(true);
                countdownText.gameObject.SetActive(true);
                controlButton.interactable = true;
                controlButton.GetComponent<Image>().color = new Color(1,1, 0.5f, 0.6f);
                break;
            
            case Mode.NotInControl:
                inControlText.text = MaroonNetworkManager.Instance.ClientInControl + " is in Control";
                statusText.text = "Click to take control";
                statusText.gameObject.SetActive(true);
                countdownText.gameObject.SetActive(false);
                controlButton.interactable = true;
                controlButton.GetComponent<Image>().color = new Color(1,0.5f, 0.5f, 0.6f);
                break;
            
            case Mode.NotInControlMyRequest:
                inControlText.text = "control requested";
                statusText.text = "Click to cancel";
                statusText.gameObject.SetActive(true);
                countdownText.gameObject.SetActive(true);
                controlButton.interactable = true;
                controlButton.GetComponent<Image>().color = new Color(1,1, 0.5f, 0.6f);
                break;
            
            case Mode.NotInControlOtherRequest:
                inControlText.text = _clientWithRequest + " requests control";
                statusText.gameObject.SetActive(false);
                countdownText.gameObject.SetActive(true);
                controlButton.interactable = false;
                controlButton.GetComponent<Image>().color = new Color(0.5f,0.5f, 0.5f, 0.6f);
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnClickCommandButton()
    {
        switch (_mode)
        {
            case Mode.InControl:
                // cannot happen
                break;
            case Mode.InControlRequest:
                CancelCountdown();
                break;
            case Mode.NotInControl:
                RequestControl();
                break;
            case Mode.NotInControlMyRequest:
                CancelCountdown();
                break;
            case Mode.NotInControlOtherRequest:
                // cannot happen
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RequestControl()
    {
        CmdClientRequestsControl(MaroonNetworkManager.Instance.PlayerName);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdClientRequestsControl(string client)
    {
        if (_countdownActive)
            return;
        RpcClientRequestsControl(client);
    }

    [ClientRpc]
    private void RpcClientRequestsControl(string client)
    {
        _clientWithRequest = client;
        if (client == MaroonNetworkManager.Instance.PlayerName)
        {
            _mode = Mode.NotInControlMyRequest;
        } 
        else if (MaroonNetworkManager.Instance.IsInControl)
        {
            _mode = Mode.InControlRequest;
        }
        else
        {
            _mode = Mode.NotInControlOtherRequest;
        }
        UpdateAppearance();
        StartCountdown();
    }

    private void StartCountdown()
    {
        _countdownTime = CountdownDuration;
        countdownText.text = _countdownTime.ToString();
        countdownText.gameObject.SetActive(true);
        _countdownActive = true;
        InvokeRepeating(nameof(DecreaseCountdown), 1, 1);
    }

    private void DecreaseCountdown()
    {
        _countdownTime--;
        if (_countdownTime <= 0)
        {
            StopCountdown();
            if (isServer)
            {
                MaroonNetworkManager.Instance.ServerSetClientInControl(_clientWithRequest);
            }
        }
        countdownText.text = _countdownTime.ToString();
    }

    private void StopCountdown()
    {
        _countdownTime = CountdownDuration;
        countdownText.gameObject.SetActive(false);
        _countdownActive = false;
        CancelInvoke(nameof(DecreaseCountdown));
    }

    private void CancelCountdown()
    {
        if (!_countdownActive)
            return;
        
        CmdCancelCoundown();
    }

    [Command(ignoreAuthority = true)]
    private void CmdCancelCoundown()
    {
        RpcCancelCoundown();
    }
    
    [ClientRpc]
    private void RpcCancelCoundown()
    {
        StopCountdown();
        //TODO: cooldown handling
        if (MaroonNetworkManager.Instance.IsInControl)
        {
            _mode = Mode.InControl;
        }
        else
        {
            _mode = Mode.NotInControl;
        }
        UpdateAppearance();
    }
}
