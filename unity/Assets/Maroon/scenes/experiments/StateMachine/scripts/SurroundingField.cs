using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;


public class SurroundingField
{    
    
    private int _value = 0;
    private List<string> _names = new List<string>();

    public SurroundingField(int value = 0) {
        _names.Add(" ");
        _names.Add("E");
        _names.Add("W");
        _names.Add("B");
        value = _value;
    }

    public void UpdateValue () {

        _value++;
        
        if (_value >= _names.Count) {
            _value = 0;
        }
    }

    public string GetName() {
        return _names[_value];
    }

    public int GetValue() {
        return _value;
    }
}

