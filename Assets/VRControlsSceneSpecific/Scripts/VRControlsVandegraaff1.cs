using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VRControlsVandegraaff1 : MonoBehaviour
{
    public Slider positionSlider;
    public Toggle startToggle, electricFieldToggle, chargeToggle;
    public GuiVandeGraaffExperiment1 vandeGraaffGui;
    public MoveLeftRight moveLeftRight;

    Vector3 rightEndPosition = new Vector3(3.367f, -3.107512f, 0.5573473f);
    Vector3 leftEndPosition = new Vector3(3.367f, -3.107512f, 2.531353f);

	// Use this for initialization
	void Start ()
	{
	    startToggle.isOn = false;
	    electricFieldToggle.isOn = false;
	    chargeToggle.isOn = false;
	    positionSlider.value = (moveLeftRight.transform.localPosition.z - rightEndPosition.z)/
	                           (leftEndPosition.z - rightEndPosition.z);
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

    public void onChangeSlider()
    {
        moveLeftRight.transform.localPosition = Vector3.Lerp(leftEndPosition, rightEndPosition, positionSlider.value);
    }

    void moveSlider(float delta)
    {
        positionSlider.value += delta;
        onChangeSlider();
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            float sliderDelta = 0f;
#if !UNITY_EDITOR
            sliderDelta = Input.GetAxis("Mouse X") * -.04f;
#else
            sliderDelta = Input.GetAxis("Mouse ScrollWheel") * .4f;
#endif
            if(Mathf.Abs(sliderDelta) >= 0.001f)
                moveSlider(sliderDelta);
        }
    }
}
