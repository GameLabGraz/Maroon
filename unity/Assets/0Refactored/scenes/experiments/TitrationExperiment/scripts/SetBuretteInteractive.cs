using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBuretteInteractive : MonoBehaviour {

    public GameObject burette; 

    private OpenBurette openBuretteScript;

    void Start () {
        openBuretteScript = burette.gameObject.GetComponent<OpenBurette>();
	}
	
    public void enableBuretteTap()
    {
        openBuretteScript.interactable = true;
    }

    public void disableBuretteTap()
    {
        openBuretteScript.interactable = false;
    }
}
