using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSortingGame : MonoBehaviour
{                                      //want it:
    public GameObject MainCamera;      //off
    public GameObject SortingMinigame; //on
    public GameObject Userinterface;   //off


    /*
     * 
     */
    private void Awake()
    {
        //Debug.Log("Star Sorting Game Awake()");
        SortingMinigame.SetActive(false);
    }


    /*
     * 
     */
    void OnMouseDown()
    {
        //Debug.Log("StartSortingGameScreen OnMouseDown() pressed!");
        SortingMinigame.SetActive(true);
        Userinterface.SetActive(false);
        MainCamera.SetActive(false);
    }


    /*
     * 
     */
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            SortingMinigame.SetActive(true);
            Userinterface.SetActive(false);
            MainCamera.SetActive(false);

        }

    }
}
