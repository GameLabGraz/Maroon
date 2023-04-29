using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSortingGame : MonoBehaviour
{
    //public GameObject Environment;
    public GameObject MainCamera;
    public GameObject SortingGameCamera;
    public GameObject Planets;

    void OnMouseDown()
    {
        Debug.Log("StartSortingGameScreen pressed!");

        //Environment.SetActive(false);
        MainCamera.SetActive(false);
        SortingGameCamera.SetActive(true);
        Planets.SetActive(true);
    }


    // Start is called before the first frame update
    void Start()
    {

        Planets.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            MainCamera.SetActive(false);
            SortingGameCamera.SetActive(true);
            Planets.SetActive(true);
        }


    }
}
