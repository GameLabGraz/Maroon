using UnityEngine;
using System;
using System.Collections;

public class ExternalFieldElectricQuadrupoleScript : MonoBehaviour {
    public GameObject electromagneticFieldController = null;
    private ElectromagneticFieldControllerScript controller;
    
    public float strength = 1.0f;

	void Start() {
	    if (!electromagneticFieldController) {
	        electromagneticFieldController = GameObject.Find("ElectromagneticFieldController");
	        if (!electromagneticFieldController) {
	            throw new System.Exception("Could not find ElectromagneticFieldController");
	        }
	    }
	    controller = electromagneticFieldController.GetComponent<ElectromagneticFieldControllerScript>();
	    
	    controller.RegisterElectricField(ElectricField);
	}

	Vector3 ElectricField(Vector3 pos) {
	    Vector3 r = transform.InverseTransformPoint(pos);
	    return transform.TransformDirection(new Vector3(r.x, -2.0f * r.y, r.z) * strength);
	}
}
