using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PC_TextFormatter_TMP : MonoBehaviour
{
    public string format = "F";

    public void FormatString(float number)
    {
        GetComponent<TMP_InputField>().text = number.ToString(format);
    }
}
