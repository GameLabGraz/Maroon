using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ValueChangeFloat : UnityEvent<float>
{
}

[System.Serializable]
public class ValueChangeString : UnityEvent<string>
{
}

public class Resistor : MonoBehaviour
{
    [Range(0f, 100f)]
    public float resistance;

    public ValueChangeFloat onValueChangedFloat;
    public ValueChangeString onValueChangedString;

    private float _prevResistance;
    
    public string GetResistanceFormatted()
    {
        return resistance.ToString("F") + " \u2126"; //Ohm/Omega Symbol
    }

    private void Start()
    {
        _prevResistance = resistance;
    }

    private void Update()
    {
//        if (!(Math.Abs(_prevResistance - resistance) > 0.0001)) return;
//        
//        _prevResistance = resistance;
//        onValueChangedFloat.Invoke(resistance);
//        onValueChangedString.Invoke(GetResistanceFormatted());
    }

    public void SetResistance(float res)
    {
        resistance = res;
        onValueChangedFloat.Invoke(resistance);
        onValueChangedString.Invoke(GetResistanceFormatted());
    }
}
