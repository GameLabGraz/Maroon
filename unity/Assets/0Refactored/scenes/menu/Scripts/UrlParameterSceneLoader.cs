﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrlParameterSceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(WebGLUrlParameterReader.GetUrlParameter("LoadExperiment"));
    }
}
