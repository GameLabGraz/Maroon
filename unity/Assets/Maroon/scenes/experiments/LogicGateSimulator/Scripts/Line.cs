using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    //public bool Connected = false;
    //public int OutputValue = 0;
    //public GameObject ConnectedTo;
    public Output Output;
    public LineRenderer line;
    public Transform position1;
    public Transform position2;
    Transform blockContainer;

    // Start is called before the first frame update
    void Start()
    {
        Output = GetComponent<Output>();
    	line = GetComponent<LineRenderer>();
	line.enabled = false;
	line.positionCount = 4;
	line.startWidth = 0.024f;
	line.endWidth = 0.024f;

	GameObject clickManager = GameObject.Find("InputOutputClickManager");

	if (clickManager != null)
	{
    	    blockContainer = clickManager.transform.parent;
	}

	//ConnectedTo = Output.ConnectedTo;
	//Connected = Output.Connected;
	//OutputValue = Output.OutputValue;
    }

    // Update is called once per frame
    void Update()
    {
        position1 = GetComponent<Transform>();
	if (Output.Connected == true && Output.ConnectedTo != null)
	{
	    line.enabled = true;
	    GameObject outputBlock = GetBlockObject(transform);
	    GameObject inputBlock = GetBlockObject(Output.ConnectedTo.transform);

	    if (Output.OutputValue == 1)
	    {
		line.material = Resources.Load("green_emissive", typeof(Material)) as Material;
	    }
	    else
	    {
		line.material = Resources.Load("line_zero", typeof(Material)) as Material;
	    }
	
	    position1 = GetComponent<Transform>();
	    position2 = Output.ConnectedTo.GetComponent<Transform>();

	    Quaternion rotationFix = Quaternion.Euler(0f, 180f, 0f);

	    if((outputBlock.name.StartsWith("Switch")) || (outputBlock.name.StartsWith("1Block")) || (outputBlock.name.StartsWith("Button")) || (outputBlock.name.StartsWith("Splitter")))
	    {
		line.SetPosition(0, position1.position - rotationFix * new Vector3(0.09f,-0.03f,0.03f));
	    }
	    else if((outputBlock.name.StartsWith("AND")) || (outputBlock.name.StartsWith("BUFFER")) || (outputBlock.name.StartsWith("NAND")) || (outputBlock.name.StartsWith("NOR")) || (outputBlock.name.StartsWith("NOT")) || (outputBlock.name.StartsWith("OR")) || (outputBlock.name.StartsWith("XOR")) || (outputBlock.name.StartsWith("XNOR")))
	    {
		line.SetPosition(0, position1.position -  rotationFix * new Vector3(0.09f,0.03f,0.03f));
	    }
	    else
	    {
	    }


	    if((outputBlock.name.StartsWith("Switch")) || (outputBlock.name.StartsWith("1Block")) || (outputBlock.name.StartsWith("Button")) || (outputBlock.name.StartsWith("Splitter")))
	    {
		line.SetPosition(1, position1.position - rotationFix * new Vector3(0.09f,-0.03f,0.03f) -  rotationFix * new Vector3(0.024f,0.0f,0.0f));
	    }
	    else if((outputBlock.name.StartsWith("AND")) || (outputBlock.name.StartsWith("BUFFER")) || (outputBlock.name.StartsWith("NAND")) || (outputBlock.name.StartsWith("NOR")) || (outputBlock.name.StartsWith("NOT")) || (outputBlock.name.StartsWith("OR")) || (outputBlock.name.StartsWith("XOR")) || (outputBlock.name.StartsWith("XNOR")))
	    {
		line.SetPosition(1, position1.position - rotationFix * new Vector3(0.09f,0.03f,0.03f) - rotationFix * new Vector3(0.024f,0.0f,0.0f));
	    }
	    else
	    {
	    }
	    
	    if((inputBlock.name.StartsWith("AND")) || (inputBlock.name.StartsWith("NAND")) || (inputBlock.name.StartsWith("NOR")) || (inputBlock.name.StartsWith("OR")) || (inputBlock.name.StartsWith("XOR")) || (inputBlock.name.StartsWith("XNOR")) || (inputBlock.name.StartsWith("BUFFER")) || (inputBlock.name.StartsWith("NOT")))
	    {
		line.SetPosition(2, position2.position - rotationFix * new Vector3(-0.09f,-0.03f,0.03f) - rotationFix * new Vector3(-0.024f,0.0f,0.0f));
	    }
	    else if((inputBlock.name.StartsWith("Blue")) || (inputBlock.name.StartsWith("Green")) || (inputBlock.name.StartsWith("Red")) || (inputBlock.name.StartsWith("Splitter")) || (inputBlock.name.StartsWith("White")) || (inputBlock.name.StartsWith("Yellow")))
	    {
		line.SetPosition(2, position2.position - rotationFix * new Vector3(-0.09f,-0.03f,-0.03f) - rotationFix * new Vector3(-0.024f,0.0f,0.0f));
	    }
	    else
	    {
	    }

	    if((inputBlock.name.StartsWith("AND")) || (inputBlock.name.StartsWith("NAND")) || (inputBlock.name.StartsWith("NOR")) || (inputBlock.name.StartsWith("OR")) || (inputBlock.name.StartsWith("XOR")) || (inputBlock.name.StartsWith("XNOR")) || (inputBlock.name.StartsWith("BUFFER")) || (inputBlock.name.StartsWith("NOT")))
	    {
		line.SetPosition(3, position2.position - rotationFix * new Vector3(-0.09f,-0.03f,0.03f));
	    }
	    else if((inputBlock.name.StartsWith("Blue")) || (inputBlock.name.StartsWith("Green")) || (inputBlock.name.StartsWith("Red")) || (inputBlock.name.StartsWith("Splitter")) || (inputBlock.name.StartsWith("White")) || (inputBlock.name.StartsWith("Yellow")))
	    {
		line.SetPosition(3, position2.position - rotationFix * new Vector3(-0.09f,-0.03f,-0.03f));
	    }
	    else
	    {
	    }
	    
	}
	else
	{
	    line.enabled = false;
	}
    }

    GameObject GetBlockObject(Transform connectionPoint)
    {
        Transform currentObject = connectionPoint;

        if (blockContainer != null)
        {
            while (currentObject.parent != null && currentObject.parent != blockContainer)
            {
                currentObject = currentObject.parent;
            }
        }

        else
        {
            while (currentObject.parent != null)
            {
                currentObject = currentObject.parent;
            }
        }

        return currentObject.gameObject;
    }
}
