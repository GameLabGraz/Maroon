using UnityEngine;
using System.Collections;

public class TurnOnMagnet : MonoBehaviour {
    public GameObject light1 = null;
    public GameObject light2 = null;
    public GameObject externalFieldSolenoid = null;
    public GameObject magneticDipole = null;
    
    private ExternalFieldSolenoidScript externalFieldSolenoidScript;
    private MagneticDipoleScript magneticDipoleScript;

    private float level = 0.0f;
    private float level2 = 0.0f;
    
    void Start() {
        if (!light1) { light1 = GameObject.Find("Light-1"); }
        if (!light2) { light2 = GameObject.Find("Light-2"); }
        if (!externalFieldSolenoid) { externalFieldSolenoid = GameObject.Find("ExternalFieldSolenoid"); }
        if (!magneticDipole) { magneticDipole = GameObject.Find("MagneticDipole"); }
        externalFieldSolenoidScript = externalFieldSolenoid.GetComponent<ExternalFieldSolenoidScript>();
        magneticDipoleScript = magneticDipole.GetComponent<MagneticDipoleScript>();
    }

    public void OnMouseUp() {
        level = 1.0e-9f;
        GameObject.Find("RotatingPlane").GetComponent<HingeJoint>().useMotor = true;
    }
    
    void FixedUpdate() {
        if (level != 0.0f) {
            level = 1.0f - 0.999f * (1.0f - level);
            if (level > 0.5f) {
                level2 = 1.0f - 0.999f * (1.0f - level2);
            }
        
            light1.GetComponent<Light>().intensity = 2.0f * level;
            light2.GetComponent<Light>().intensity = 2.0f * level;
    
            externalFieldSolenoidScript.strength = 3.0e8f * level2;
            magneticDipoleScript.strength = 3.0e3f * level2;
        }
    }
}
