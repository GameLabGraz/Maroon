using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleUI : MonoBehaviour
{
    public GameObject AnimationUI;
    public GameObject SortingGameUIPlanetInfo;
    public Toggle toggleHideUI;

   /*
    *  change with boolean from key and set flag for ToggleGroup state
    */
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.H))
        {
            if (AnimationUI.activeSelf == false)// && (SortingGameUIPlanetInfo.activeSelf == false))
            {
                AnimationUI.SetActive(true); //turn on UI
                SortingGameUIPlanetInfo.SetActive(true); //turn on UI
                toggleHideUI.isOn = false;
                Debug.Log("KeyCode.H UI.H Active Self: " + AnimationUI.activeSelf);
            }
            else
            {
                AnimationUI.SetActive(false); //turn off UI
                SortingGameUIPlanetInfo.SetActive(false); //turn off UI
                toggleHideUI.isOn = true;
                Debug.Log("KeyCode.H UI Active Self: " + AnimationUI.activeSelf);
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
            AnimationUI.SetActive(false); //turn on UI
            SortingGameUIPlanetInfo.SetActive(false); //turn on UI
            //Debug.Log("ToggleHideUI: " + ui.activeSelf);
        }
        else
        {
            AnimationUI.SetActive(true); //turn off UI
            SortingGameUIPlanetInfo.SetActive(true); //turn off UI
            //Debug.Log("ToggleHideUI: " + ui.activeSelf);
        }
    }
}
