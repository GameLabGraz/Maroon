using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LED : MonoBehaviour
{
    GameObject input;
    GameObject led;
    string color;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        input = transform.Find("Input").gameObject;
	led = transform.Find("LED").gameObject;
	color = this.transform.parent.gameObject.name;
	mat = led.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
	if (input.GetComponent<InputS>().InputValue == 0)
	{
	    if(mat != led.GetComponent<Renderer>().material)
	    {
		led.GetComponent<Renderer>().material = mat;
	    }
	}
	else if(color.StartsWith("RedLED"))
	{
	    led.GetComponent<Renderer>().material = Resources.Load("red_emissive", typeof(Material)) as Material;
	}
	else if(color.StartsWith("BlueLED"))
	{
	    led.GetComponent<Renderer>().material = Resources.Load("blue_emissive", typeof(Material)) as Material;
	}
	else if(color.StartsWith("GreenLED"))
	{
	    led.GetComponent<Renderer>().material = Resources.Load("green_emissive", typeof(Material)) as Material;
	}
	else if(color.StartsWith("WhiteLED"))
	{
	    led.GetComponent<Renderer>().material = Resources.Load("glass_emissive", typeof(Material)) as Material;
	}
	else if(color.StartsWith("YellowLED"))
	{
	    led.GetComponent<Renderer>().material = Resources.Load("yellow_emissive", typeof(Material)) as Material;
	}
        
    }
}
