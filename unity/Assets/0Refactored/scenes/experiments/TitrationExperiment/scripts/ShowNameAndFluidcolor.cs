﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowNameAndFluidcolor : MonoBehaviour {

    public Text label;

    private ShowFluid showFluidScript;
    private Renderer rend;
    private MeshRenderer meshRend;

    void Start () 
    {
        showFluidScript = ShowFluid.Instance;
        rend = gameObject.transform.GetChild(2).gameObject.GetComponent<Renderer>();
        meshRend = gameObject.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>();
        changeName(label);
    }
	
	public void changeName (Text label) 
    {
        rend.material.mainTexture = Resources.Load<Texture>("Sprites/" + label.text);

        if (label.text.Equals("HNO3"))
            meshRend.material = showFluidScript.FluidWaterYellow;
        else
            meshRend.material = showFluidScript.FluidWaterColorless;
    }
}
