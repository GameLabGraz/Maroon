//-----------------------------------------------------------------------------
// ScriptSlider.cs
//
// Script to manage sliders
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// script to manage sliders
/// </summary>
public class ScriptSlider : MonoBehaviour
{
    /// <summary>
    /// Sets the input field value to the 
    /// value of the slider
    /// </summary>
    /// <param name="inputField">input field that will
    /// have it's value set</param>
	public void setInputFieldValue(InputField inputField)
    {
        float value = GetComponent<Slider>().value;
        value = Mathf.Round(value * 100f) / 100f;
        inputField.text = value.ToString();
    }

    /// <summary>
    /// Sets the value of the slider
    /// </summary>
    /// <param name="slider">The slider that will
    /// have it's value set</param>
	public void setSliderValue(Slider slider)
    {
        slider.value = float.Parse(GetComponent<InputField>().text);
    }
}
