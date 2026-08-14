using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XNOR : MonoBehaviour
{
    GameObject input1;
    GameObject input2;
    GameObject output;
    // Start is called before the first frame update
    void Start()
    {
        input1 = transform.Find("Input1").gameObject;
	input2 = transform.Find("Input2").gameObject;
	output = transform.Find("Output").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if ((input1.GetComponent<InputS>().InputValue == 1) && (input2.GetComponent<InputS>().InputValue == 0))
	{
	   output.GetComponent<Output>().OutputValue = 0;
	}
	else if ((input1.GetComponent<InputS>().InputValue == 0) && (input2.GetComponent<InputS>().InputValue == 1))
	{
	   output.GetComponent<Output>().OutputValue = 0;
	}
	else
	{
	   output.GetComponent<Output>().OutputValue = 1; 
	}
    }

}
