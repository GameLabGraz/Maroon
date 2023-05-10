using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSortingGame : MonoBehaviour
{                                      //want it:
    public GameObject MainCamera;      //off
    public GameObject SortingMinigame; //on
    public GameObject Userinterface;   //off
    public GameObject UserinterfaceHideUI;   //off


    private void Awake()
    {
        //Debug.Log("Star Sorting Game Awake()");
        SortingMinigame.SetActive(false);
    }


    void OnMouseDown()
    {
        StartSortingGameOnInput();
    }


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            StartSortingGameOnInput();
        }
    }


   /*
    * 
    */
    void StartSortingGameOnInput()
    {
        //Debug.Log("StartSortingGameOnInput OnMouseDown() pressed!");
        SortingMinigame.SetActive(true);
        Userinterface.SetActive(false);
        UserinterfaceHideUI.SetActive(false);
        MainCamera.SetActive(false);
    }
}
