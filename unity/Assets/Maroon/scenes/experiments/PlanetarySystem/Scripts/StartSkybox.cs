using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSkybox : MonoBehaviour
{
    public Material stars_skybox;

    public GameObject Experiment;
    public GameObject Environment;
    public GameObject MainCamera;
    public GameObject SolarSystemCamera;
    public GameObject Planets;

    void OnMouseDown()
    {
        Debug.Log("Button pressed!");

        RenderSettings.skybox = stars_skybox;

        Experiment.SetActive(false);
        Environment.SetActive(false);
        MainCamera.SetActive(false);
        SolarSystemCamera.SetActive(true);
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
        if (Input.GetKeyUp(KeyCode.Space))
        {
            RenderSettings.skybox = stars_skybox;

            Experiment.SetActive(false);
            Environment.SetActive(false);
            MainCamera.SetActive(false);
            SolarSystemCamera.SetActive(true);
            Planets.SetActive(true);
        }
        

    }
}
