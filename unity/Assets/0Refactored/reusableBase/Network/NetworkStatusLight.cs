using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkStatusLight : MonoBehaviour
{
    public GameObject lightObject;

    private Light _light;
    
    // Start is called before the first frame update
    void Start()
    {
        _light = lightObject.GetComponent<Light>();
    }

    private void UpdateLightStatus()
    {
        if (NetworkClient.active && ClientScene.ready)
        {
            _light.color = Color.green;
        }
        else if (FindObjectOfType<ListServer>().GetListServerStatus())
        {
            _light.color = Color.yellow;
        }
        else
        {
            _light.color = Color.red;
        }
    }

    public void SetActive(bool status)
    {
        lightObject.SetActive(status);
        if (status)
        {
            InvokeRepeating(nameof(UpdateLightStatus), 0, 1);
        }
        else
        {
            CancelInvoke();
        }
    }
}
