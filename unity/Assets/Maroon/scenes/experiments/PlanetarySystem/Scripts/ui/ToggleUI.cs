using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleUI : MonoBehaviour
{
    public GameObject ui;
    public Toggle toggleHideUi;

   /*
    *  change with boolean from key and set flag for ToggleGroup state
    */
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.H))
        {
            if (ui.activeSelf == false)
            {
                ui.SetActive(true); //turn on UI
                toggleHideUi.isOn = false;
                //Debug.Log("UI Active Self: " + ui.activeSelf);
            }
            else
            {
                ui.SetActive(false); //turn off UI
                toggleHideUi.isOn = true;
                //Debug.Log("UI Active Self: " + ui.activeSelf);
            }
        }
    }


   /*
    *  change with boolean from ToggleGroup function
    */
    public void ToggleHideUI(bool hide)
    {
        //Debug.Log("ToggleHideUI: " +  hide);
        if (hide)
        {
            ui.SetActive(false); //turn on UI
            //Debug.Log("ToggleHideUI: " + ui.activeSelf);
        }
        else
        {
            ui.SetActive(true); //turn off UI
            //Debug.Log("ToggleHideUI: " + ui.activeSelf);
        }
    }
}
