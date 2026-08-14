using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUFFER : MonoBehaviour
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
        output.GetComponent<Output>().OutputValue = input.GetComponent<InputS>().InputValue;
    }
}
