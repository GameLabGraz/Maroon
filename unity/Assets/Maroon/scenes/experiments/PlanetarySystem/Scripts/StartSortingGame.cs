using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSortingGame : MonoBehaviour
{
    //public GameObject Environment;
    public GameObject MainCamera;      //off
    public GameObject SortingMinigame; //on
    public GameObject Userinterface;   //off

    void OnMouseDown()
    {
        Debug.Log("StartSortingGameScreen OnMouseDown() pressed!");

        SortingMinigame.SetActive(true);
        Userinterface.SetActive(false);
        MainCamera.SetActive(false);
    }


    private void Awake()
    {
        Debug.Log("Star Sorting Game Awake()");
        SortingMinigame.SetActive(false);
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            SortingMinigame.SetActive(true);
            Userinterface.SetActive(false);
            MainCamera.SetActive(false);

        }

    }
}
