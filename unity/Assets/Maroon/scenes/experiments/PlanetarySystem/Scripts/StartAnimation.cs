using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAnimation : MonoBehaviour
{
    public Material stars_skybox;

    public GameObject Environment;
    public GameObject MainCamera;
    public GameObject SolarSystemCamera;
    public GameObject Planets;
    //public GameObject sun;

    void OnMouseDown()
    {
        Debug.Log("StartAnimationScreen pressed!");

        RenderSettings.skybox = stars_skybox;

        Environment.SetActive(false);
        MainCamera.SetActive(false);
        SolarSystemCamera.SetActive(true);
        Planets.SetActive(true);
       // sun.SetActive(true);
    }


    // Start is called before the first frame update
    void Start()
    {
        Planets.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            RenderSettings.skybox = stars_skybox;

            Environment.SetActive(false);
            MainCamera.SetActive(false);
            SolarSystemCamera.SetActive(true);
            Planets.SetActive(true);
            //sun.SetActive(true);
        }
        

    }
}
