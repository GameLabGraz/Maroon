using UnityEngine;
using VRTK;

[RequireComponent(typeof(VRTK_Slider))]
public class VR_RestrictSlitWidthSliderValue : RestrictSlitWidthSliderValue
{
    private VRTK_Slider _slider;

    private void Start()
    {
        _slider = GetComponent<VRTK_Slider>();

        setSliderMaxValue += maxValue =>
        {
            _slider.maximumValue = maxValue;
        };
    }
}
