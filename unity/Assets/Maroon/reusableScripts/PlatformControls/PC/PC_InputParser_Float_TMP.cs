﻿using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TextChangeFloat : UnityEvent<float> {}

[RequireComponent(typeof(TMP_InputField))]
public class PC_InputParser_Float_TMP : MonoBehaviour
{
    private TMP_InputField _textField;
    [SerializeField]
    public float minimum = float.MinValue;
    [SerializeField]
    public float maximum = float.MaxValue;
    
    public string textFormat = "F";
    private string _prevText;
    private float _value = 0;
    
    public TextChangeFloat onValueChangedFloat;
    
    private void Start()
    {
        _textField = GetComponent<TMP_InputField>();
        _textField.onEndEdit.AddListener(OnTextChanged);
        _prevText = _textField.textComponent.text;
    }

    private void OnTextChanged(string changedText)
    {
        string text;
        if (float.TryParse(changedText, NumberStyles.Any, CultureInfo.CurrentCulture, out var number))
        {
            if (number < minimum)
                number = minimum;
            if (number > maximum)
                number = maximum;

            text = number.ToString(textFormat, CultureInfo.CurrentCulture);
            _value = number;
            _prevText = text;
            onValueChangedFloat.Invoke(number);
        }
        else
        {
            text = _prevText;
            _textField.text = text;
        }

        _textField.textComponent.text = text;
    }

    public float GetValue()
    {
        return _value;
    }

    public void SetValue(float value)
    {
        _textField.text = value.ToString(textFormat, CultureInfo.CurrentCulture);
    }
}
