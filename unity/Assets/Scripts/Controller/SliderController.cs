using System.Collections.Generic;
using GEAR.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
using VRTK.UnityEventHelper;

public class SliderController : VRTK_Slider, IResetObject
{
    [SerializeField]
    private GameObject invokeObject;

    [SerializeField]
    private string methodName;

    [SerializeField]
    private List<string> _optionKeys = new List<string>(); 

    [SerializeField]
    private Text ValueText;
    
    [SerializeField]
    private TextMeshPro ValueTMP;

    [SerializeField]
    private bool isInteger = false;

    private VRTK_Control_UnityEvents controlEvents;

    private Vector3 startPos;

    private Quaternion startRot;

    private void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;

        controlEvents = GetComponent<VRTK_Control_UnityEvents>();
        if (controlEvents == null)
        {
            controlEvents = gameObject.AddComponent<VRTK_Control_UnityEvents>();
        }

        UpdateValueText();

        controlEvents.OnValueChanged.AddListener(HandleChange);
    }

    protected override ControlValueRange RegisterValueRange()
    {
        if(_optionKeys.Count > 0)
        {
            minimumValue = 0;
            maximumValue = _optionKeys.Count - 1;
        }

        return new ControlValueRange()
        {
            controlMin = minimumValue,
            controlMax = maximumValue
        };
    }

    private void UpdateValueText()
    {
        if(!ValueText && !ValueTMP)
            return;

        var text = "";

        if (_optionKeys.Count > 0)
            text = LanguageManager.Instance.GetString(_optionKeys[(int)GetValue()]);
        else if (isInteger)
            text = ((int)GetValue()).ToString();
        else
            text = GetValue().ToString("0.00");

        if (ValueText)
            ValueText.text = text;
        if (ValueTMP)
            ValueTMP.text = text;
    }

    private void HandleChange(object sender, Control3DEventArgs e)
    {
        UpdateValueText();

        if(invokeObject != null)
        {
            if (isInteger)
                invokeObject.SendMessage(methodName, (int)GetValue());
            else
                invokeObject.SendMessage(methodName, GetValue());
        }
    }

    public void ResetObject()
    {
        transform.position = startPos;
        transform.rotation = startRot;
    }
}
