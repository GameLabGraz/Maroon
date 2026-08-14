using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splitter : MonoBehaviour
{
    GameObject input;
    GameObject output1;
    GameObject output2;
    // Start is called before the first frame update
    void Start()
    {
	input = transform.Find("Input").gameObject;
	output1 = transform.Find("Output1").gameObject;
	output2 = transform.Find("Output2").gameObject; 
    }

    // Update is called once per frame
    void Update()
    {
        output1.GetComponent<Output>().OutputValue = input.GetComponent<InputS>().InputValue;
	output2.GetComponent<Output>().OutputValue = input.GetComponent<InputS>().InputValue;
    }
}
