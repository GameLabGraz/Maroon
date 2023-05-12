using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSortingGame : MonoBehaviour
{                                      //want it:
    public GameObject MainCamera;      //off
    public GameObject SortingMinigame; //on
    public GameObject Userinterface;   //off
    public GameObject PlanetInfoUI;    //on
    public GameObject FormulaUI;       //off


    private void Awake()
    {
        //Debug.Log("Star Sorting Game Awake()");
        SortingMinigame.SetActive(false);
        PlanetInfoUI.SetActive(false);
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
        PlanetInfoUI.SetActive(true);
        FormulaUI.SetActive(false);
        MainCamera.SetActive(false);
        PlanetaryController.Instance.DisplayMessageByKey("EnterSortingGame");
    }
}
