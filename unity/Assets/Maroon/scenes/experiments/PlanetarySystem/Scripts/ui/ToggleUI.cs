//
//Author: Marcel Lohfeyer
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUI : MonoBehaviour
{
    public GameObject ui;

    // Start is called before the first frame update
    void Start()
    {
        //ui = GetGetComponent<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.H))
        {

            if (ui.activeSelf == false)
            {
                ui.SetActive(true);
                Debug.Log("UI Active Self: " + ui.activeSelf);
            }
            else
            {
                ui.SetActive(false);
                Debug.Log("UI Active Self: " + ui.activeSelf);
            }
        }

    }
}
