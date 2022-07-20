using System.Collections.Generic;
using UnityEngine;
using Maroon.CSE.StateMachine;

public enum ModeCode {
    EMPTY = 0,
    HIT = 1,
    EMTPY_AND_END = 2,
}
public class Mode
{
   
    private string _name;

    private string _modeKey;
    // 0 = go on empty field
    // 1 = hit figure
    // 2 = go on empty field and end move
    // .....
    private ModeCode _modeCode;

    public Mode (string modeName, string modeKey, ModeCode modeCode) {
        _name = modeName;
        _modeKey = modeKey;
        _modeCode = modeCode;
    }

    public string GetModeName() {
        return _name;
    }

    public string GetModeKey()
    {
        return _modeKey;
    }

    public ModeCode GetModeCode() {
        return _modeCode;
    }

    public void SetModeName(string modeName) {
        _name = modeName;
    }
}
