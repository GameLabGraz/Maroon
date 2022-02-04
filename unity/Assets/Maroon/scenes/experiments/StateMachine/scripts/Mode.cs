using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mode
{
    private string _name;

    private string _modeKey;
    // 0 = go on empty field
    // 1 = hit figure
    // 2 = go on empty field and end move
    // .....
    private int _modeCode;

    public Mode (string modeName, string modeKey, int modeCode) {
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

    public int GetModeCode() {
        return _modeCode;
    }

    public void SetModeName(string modeName) {
        _name = modeName;
    }
}
