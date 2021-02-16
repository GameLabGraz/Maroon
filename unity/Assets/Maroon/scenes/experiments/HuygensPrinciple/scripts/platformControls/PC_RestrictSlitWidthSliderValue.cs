using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class PC_RestrictSlitWidthSliderValue : RestrictSlitWidthSliderValue
{
    private Slider _slider;

    private void Start()
    {
        _slider = GetComponent<Slider>();

        setSliderMaxValue += maxValue =>
        { 
            _slider.maxValue = maxValue;
        };
    }
}
