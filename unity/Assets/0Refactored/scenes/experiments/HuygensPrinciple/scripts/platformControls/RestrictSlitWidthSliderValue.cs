using Maroon.Physics.HuygensPrinciple;
using UnityEngine;


public abstract class RestrictSlitWidthSliderValue : MonoBehaviour
{
    [SerializeField] private SlitPlate slitPlate;

    protected delegate void SetSliderMaxValue(float maxValue);
    protected SetSliderMaxValue setSliderMaxValue;

    public void RestrictSliderValue()
    {
        if (slitPlate == null) return;

        var maxSlitWidth = (slitPlate.PlateWidth / slitPlate.NumberOfSlits) - 0.04f;
        maxSlitWidth = maxSlitWidth > 1.0f ? 1.0f : maxSlitWidth;
        
        setSliderMaxValue?.Invoke(maxSlitWidth);
    }
}
