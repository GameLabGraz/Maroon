using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInControlUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MaroonNetworkManager.Instance.onGetControl.AddListener(OnGetControl);
        MaroonNetworkManager.Instance.onLoseControl.AddListener(OnLoseControl);
        
        gameObject.SetActive(false);
    }

    private void OnGetControl()
    {
        gameObject.SetActive(true);
    }
    
    private void OnLoseControl()
    {
        gameObject.SetActive(false);
    }
}
