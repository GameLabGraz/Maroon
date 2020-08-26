using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkScreen : MonoBehaviour
{
    [SerializeField] private GameObject ImageScreen;
    
    [SerializeField] private GameObject TextScreen;

    private void Start()
    {
        ImageScreen.SetActive(true);
        TextScreen.SetActive(false);
    }

    public void DisplayMessage(string message)
    {
        TextScreen.GetComponent<TextMeshProUGUI>().text = message;
        ImageScreen.SetActive(false);
        TextScreen.SetActive(true);
    }
}
