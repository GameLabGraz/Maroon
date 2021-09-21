using GEAR.VRInteraction;
using UnityEngine;

[RequireComponent(typeof(VRLinearDrive))]
public class VR_RestrictSlitWidthSliderValue : RestrictSlitWidthSliderValue
{
    private VRLinearDrive _slider;

    private void Start()
    {
        _slider = GetComponent<VRLinearDrive>();

        setSliderMaxValue += maxValue =>
        {
            _slider.maximum = maxValue;
        };
    }
}
