using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowNameAndFluidcolor : MonoBehaviour {

    public GameObject fluid;
    public Text label;

    public Material FluidWaterColorless;
    public Material FluidWaterYellow;

    Renderer rend;
    MeshRenderer meshRend;

    void Start () {
        rend = GetComponent<Renderer>();
        meshRend = fluid.GetComponent<MeshRenderer>();
        changeName(label);
    }
	
	public void changeName (Text label) {

        rend.material.mainTexture = Resources.Load<Texture>("Sprites/" + label.text);

        if (label.text.Equals("HNO3"))
            meshRend.material = FluidWaterYellow;
        else
            meshRend.material = FluidWaterColorless;
    }
}
