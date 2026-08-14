using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputS : MonoBehaviour
{
    public int InputValue = 0;
    public bool Connected = false;
    public GameObject ConnectedTo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Connected)
	{
	    InputValue = ConnectedTo.GetComponent<Output>().OutputValue;
	}
	else
	{
	    InputValue = 0;
	}
    }

    void OnMouseDown()
    {
	if(Connected)
	{
	    if(GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().CounterObject == ConnectedTo)
	    {
	        Connected = false;
		ConnectedTo.GetComponent<Output>().Connected = false;
		ConnectedTo.GetComponent<Output>().ConnectedTo = null;
		ConnectedTo = null;
		GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().CounterObject = null;
		GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().Counter = 0;
		return;
	    }
	
	    if(GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().CounterObject == this.gameObject)
	    {
	        Connected = false;
		ConnectedTo.GetComponent<Output>().Connected = false;
		ConnectedTo.GetComponent<Output>().ConnectedTo = null;
		ConnectedTo = null;
		GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().CounterObject = null;
		GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().Counter = 0;
		return;
	    }

	    GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().CounterObject = this.gameObject;
	    GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().Counter = 1;
	    GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().ClickedInput = null;
	    GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().ClickedOutput = null;
	    return;
	}


        GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().SetClickedInput(this.gameObject);
    }
}
