using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mode : MonoBehaviour
{
    private string _name;

    public Mode (string modeName) {
        _name = modeName;
    }

    public string GetModeName() {
        return _name;
    }
}
