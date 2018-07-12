using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VRControlsVandegraaff2 : MonoBehaviour
{
    public Toggle startToggle, electricFieldToggle, chargeToggle;
    public GuiVandeGraaffExperiment2 vandeGraaffGui;

    //Vector3 rightEndPosition = new Vector3(3.367f, -3.107512f, 0.5573473f);
    //Vector3 leftEndPosition = new Vector3(3.367f, -3.107512f, 2.531353f);
    
	void Start ()
	{
	    startToggle.isOn = false;
	    electricFieldToggle.isOn = false;
	    chargeToggle.isOn = false;
	}

    public void onClickStart()
    {
        vandeGraaffGui.vandeGraaffController.Switch();
    }

    public void onClickElectricField()
    {
        vandeGraaffGui.vandeGraaffController.FieldLinesEnabled = electricFieldToggle.isOn;
    }

    public void onClickCharge()
    {
        vandeGraaffGui.glowEnabled = chargeToggle.isOn;
        vandeGraaffGui.EnableGlow(vandeGraaffGui.glowEnabled);
    }
}
