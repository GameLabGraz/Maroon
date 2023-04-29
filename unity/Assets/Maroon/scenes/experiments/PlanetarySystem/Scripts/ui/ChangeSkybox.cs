//
//Author: Marcel Lohfeyer
//s
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkybox : MonoBehaviour
{
    public Material stars_skybox;
    public Material space_skybox;
    public Material black_skybox;


    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.skybox = stars_skybox;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RenderSettings.skybox = stars_skybox;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RenderSettings.skybox = space_skybox;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RenderSettings.skybox = black_skybox;
        }
    }
}
