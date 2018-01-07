using UnityEngine;
using VRTK;

public class SliderReactor : MonoBehaviour
{
    public GameObject InvokeObject;
    public bool IsInteger;
    public string MethodName;
    private VRTK_Slider _slider;

    private void Start()
    {
        _slider.ValueChanged += SliderOnValueChanged;
        // fixes not firing event at start when value is 0
        SliderOnValueChanged(null,
            new Control3DEventArgs() {value = _slider.GetValue(), normalizedValue = _slider.GetNormalizedValue()});
    }

    private void SliderOnValueChanged(object sender, Control3DEventArgs control3DEventArgs)
    {
        float value = control3DEventArgs.value;
        value = IsInteger ? Mathf.RoundToInt(value) : value;
        InvokeObject.SendMessage(MethodName, value);
    }

    private void Awake()
    {
        _slider = GetComponentInChildren<VRTK_Slider>();
    }
}