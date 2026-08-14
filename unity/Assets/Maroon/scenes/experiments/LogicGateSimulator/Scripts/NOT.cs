using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NOT : MonoBehaviour
{
    GameObject input;
    GameObject output;
    // Start is called before the first frame update
    void Start()
    {
        input = transform.Find("Input").gameObject;
	output = transform.Find("Output").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (input.GetComponent<InputS>().InputValue == 1)
	{
	   output.GetComponent<Output>().OutputValue = 0; 
	}
	else
	{
	   output.GetComponent<Output>().OutputValue = 1;
	} 
    }
}
