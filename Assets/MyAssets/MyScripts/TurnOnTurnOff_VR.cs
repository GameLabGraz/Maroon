using UnityEngine;
using System.Collections;
using VRTK;

public class TurnOnTurnOff_VR : VRTK_InteractableObject { //MonoBehaviour

    public GameObject charge;
    public Light glow;

    private StaticChargeScript chargeScript;
    private int target = 0;
    private float current = 0.0f;

	private void Start()
	{
        chargeScript = charge.GetComponent<StaticChargeScript>();
        //target = 1; //for debug purposes, switch it on right away
    }
	
    /* desktop version 
    void OnMouseUp() {
        if (target == 0) {
            target = 1;
        }
        else {
            target = 0;
        }
    } */

    public override void StartUsing(GameObject usingObject)
    {
        Debug.Log("Generator StartUsing");

        base.StartUsing(usingObject);

        if (target == 0)
        {
            target = 1;
        }
        else {
            target = 0;
        }

        Debug.Log("Generator turned ON/OFF");
    }

    public override void StopUsing(GameObject usingObject)
    {
        Debug.Log("Generator StopUsing");
        base.StopUsing(usingObject);
    }

    protected override void FixedUpdate() {

        if (target == 0) {
            current *= 0.9f;
        }
        else {
            current = 1.0f - 0.99f * (1.0f - current);
        }
    
        chargeScript.strength = 1e-4f * current;
        glow.intensity = 8.0f * current;

        //Debug.Log("Generator Updated");

        //TODO 
    }
}
