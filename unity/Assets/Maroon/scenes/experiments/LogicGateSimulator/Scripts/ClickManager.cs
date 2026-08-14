using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public GameObject ClickedInput;
    public GameObject ClickedOutput;
    public int Counter = 0;
    public GameObject CounterObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetClickedInput(GameObject input)
    {
        if (ClickedInput != null)
        {
            ClickedInput = null;
            return;
        }

	if (CounterObject != null)
	{
	    CounterObject = null;
	    Counter = 0;
	}

        ClickedInput = input;

	if (ClickedInput != null && ClickedOutput != null)
	{
	    if (GetBlockObject(ClickedInput) == GetBlockObject(ClickedOutput))
            {
                ClickedOutput = null;
                ClickedInput = null;
                return;
            }

            ClickedInput.GetComponent<InputS>().Connected = true;
	    ClickedInput.GetComponent<InputS>().ConnectedTo = ClickedOutput;
            ClickedOutput.GetComponent<Output>().Connected = true;
	    ClickedOutput.GetComponent<Output>().ConnectedTo = ClickedInput;
	    
	    ClickedOutput = null;
	    ClickedInput = null;
	}
    }

    public void SetClickedOutput(GameObject output)
    {
        if (ClickedOutput != null)
        {
            ClickedOutput = null;
            return;
        }

	if (CounterObject != null)
	{
	    CounterObject = null;
	    Counter = 0;
	}

        ClickedOutput = output;

	if (ClickedInput != null && ClickedOutput != null)
	{
	    if (GetBlockObject(ClickedInput) == GetBlockObject(ClickedOutput))
            {
                ClickedOutput = null;
                ClickedInput = null;
                return;
            }

	    ClickedInput.GetComponent<InputS>().Connected = true;
	    ClickedInput.GetComponent<InputS>().ConnectedTo = ClickedOutput;
            ClickedOutput.GetComponent<Output>().Connected = true;
	    ClickedOutput.GetComponent<Output>().ConnectedTo = ClickedInput;
	    
	    ClickedOutput = null;
	    ClickedInput = null;
	}
    }

    public void ResetClicks()
    {
        ClickedOutput = null;
	ClickedInput = null;
    }

    GameObject GetBlockObject(GameObject connectionPoint)
    {
        Transform currentObject = connectionPoint.transform;
        Transform blockContainer = transform.parent;

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
