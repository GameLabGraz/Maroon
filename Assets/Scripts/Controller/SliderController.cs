using System.Collections.Generic;
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
    private List<string> options = new List<string>(); 

    [SerializeField]
    private Text ValueText;

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

        if (ValueText != null)
        {
            if (options.Count > 0)
                ValueText.text = options[(int)GetValue()];
            else if (isInteger)
                ValueText.text = ((int)GetValue()).ToString();
            else
                ValueText.text = GetValue().ToString("0.00");
        }

        controlEvents.OnValueChanged.AddListener(HandleChange);
    }

    protected override ControlValueRange RegisterValueRange()
    {
        if(options.Count > 0)
        {
            minimumValue = 0;
            maximumValue = options.Count - 1;
        }

        return new ControlValueRange()
        {
            controlMin = minimumValue,
            controlMax = maximumValue
        };
    }

    private void HandleChange(object sender, Control3DEventArgs e)
    {
        if (ValueText != null)
        {
            if (options.Count > 0)
                ValueText.text = options[(int)GetValue()];
            else if (isInteger)
                ValueText.text = ((int)GetValue()).ToString();
            else
                ValueText.text = GetValue().ToString("0.00");
        }

        if(invokeObject != null)
        {
            if (isInteger)
                invokeObject.SendMessage(methodName, (int)GetValue());
            else
                invokeObject.SendMessage(methodName, GetValue());
        }
    }

    public void resetObject()
    {
        transform.position = startPos;
        transform.rotation = startRot;
    }
}
