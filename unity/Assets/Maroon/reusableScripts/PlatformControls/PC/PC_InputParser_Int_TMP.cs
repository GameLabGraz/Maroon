using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TextChangeInt : UnityEvent<float> {}

[RequireComponent(typeof(TMP_InputField))]
public class PC_InputParser_Int_TMP : MonoBehaviour
{
    private TMP_InputField _textField;
    [SerializeField]
    public int minimum = int.MinValue;
    [SerializeField]
    public int maximum = int.MaxValue;
    
    public string textFormat = "D";
    private string _prevText;
    private int _value = 0;
    
    public TextChangeInt onValueChangedInt;
    
    private void Start()
    {
        _textField = GetComponent<TMP_InputField>();
        _textField.onEndEdit.AddListener(OnTextChanged);
        _prevText = _textField.textComponent.text;
    }

    private void OnTextChanged(string changedText)
    {
        string text;
        if (int.TryParse(changedText, out var number))
        {
            if (number < minimum)
                number = minimum;
            if (number > maximum)
                number = maximum;

            text = number.ToString(textFormat);
            _value = number;
            _prevText = text;
            onValueChangedInt.Invoke(number);
        }
        else
        {
            text = _prevText;
            _textField.text = text;
        }

        _textField.textComponent.text = text;
    }

    public int GetValue()
    {
        return _value;
    }

    public void SetValue(float value)
    {
        var intValue = (int) value;
        _textField.text = intValue.ToString(textFormat);
    }
}
