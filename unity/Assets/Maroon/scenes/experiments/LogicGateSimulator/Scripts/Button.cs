using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    GameObject output;
    Material mat;
    Vector3 oldpos;
    Vector3 oldpos2;
    Transform blockAnchor;


    // Start is called before the first frame update
    void Start()
    {
        output = transform.Find("Output").gameObject;
        mat = this.GetComponent<Renderer>().material;
	blockAnchor = transform.parent; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
    void OnMouseDown()
    {
	output.GetComponent<Output>().OutputValue = 1;
	this.GetComponent<Renderer>().material = Resources.Load("green_emissive", typeof(Material)) as Material;
	oldpos = transform.localPosition;
        oldpos2 = blockAnchor.InverseTransformPoint(output.transform.position);
	transform.localPosition = oldpos + new Vector3(0f, -0.025f, 0f);
	output.transform.position = blockAnchor.TransformPoint(oldpos2);
    }

    void OnMouseUp()
    {
	output.GetComponent<Output>().OutputValue = 0;
	this.GetComponent<Renderer>().material = mat;
	transform.localPosition = oldpos;
	output.transform.position = blockAnchor.TransformPoint(oldpos2);
    }
}
