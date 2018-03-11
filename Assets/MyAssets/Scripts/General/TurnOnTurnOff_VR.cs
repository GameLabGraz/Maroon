using UnityEngine;
using System.Collections;
using VRTK;

public class TurnOnTurnOff_VR : VRTK_InteractableObject
{
    [SerializeField]
    private GameObject charge;

    [SerializeField]
    private Light glow;

    private StaticChargeScript chargeScript;
    private int target = 0;
    private float current = 0.0f;

	private void Start()
	{
        chargeScript = charge.GetComponent<StaticChargeScript>();
    }

    public override void StartUsing(GameObject usingObject)
    {
        Debug.Log("Generator StartUsing");

        base.StartUsing(usingObject);

        if (target == 0)
            target = 1;
        else 
            target = 0;

        Debug.Log("Generator turned ON/OFF");
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (target == 0)
            current *= 0.9f;
        else
            current = 1.0f - 0.99f * (1.0f - current);
    
        chargeScript.strength = 0.1e-4f * current;
        glow.intensity = 5f * current;
    }
}
