using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Output : MonoBehaviour
{
    public int OutputValue = 0;
    public bool Connected = false;
    public GameObject ConnectedTo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
 
    }

   void onTriggerEnter(Collider obj)
   {
	//transform.Translate(0,0,10);
   }

    void OnMouseDown()
    {
	if(Connected)
	{
	    if(GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().CounterObject == ConnectedTo)
	    {
	        Connected = false;
		ConnectedTo.GetComponent<InputS>().Connected = false;
		ConnectedTo.GetComponent<InputS>().ConnectedTo = null;
		ConnectedTo = null;
		GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().CounterObject = null;
		GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().Counter = 0;
		return;
	    }
	
	    if(GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().CounterObject == this.gameObject)
	    {
	        Connected = false;
		ConnectedTo.GetComponent<InputS>().Connected = false;
		ConnectedTo.GetComponent<InputS>().ConnectedTo = null;
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

        GameObject.Find("InputOutputClickManager").GetComponent<ClickManager>().SetClickedOutput(this.gameObject);
    }
}
