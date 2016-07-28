using UnityEngine;
using System.Collections;

public class TurnOnTurnOff : MonoBehaviour {
    public GameObject charge;
    public Light glow;

    private StaticChargeScript chargeScript;
    private int target = 0;
    private float current = 0.0f;

	void Start() {
        chargeScript = charge.GetComponent<StaticChargeScript>();
	}
	
    void OnMouseUp() {
        if (target == 0) {
            target = 1;
        }
        else {
            target = 0;
        }
    }

    void FixedUpdate() {
        if (target == 0) {
            current *= 0.9f;
        }
        else {
            current = 1.0f - 0.99f * (1.0f - current);
        }
    
        chargeScript.strength = 1e-4f * current;
        glow.intensity = 8.0f * current;
    }
}
