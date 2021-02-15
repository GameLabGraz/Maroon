using System.Globalization;
using TMPro;
using UnityEngine;

public class PC_TextFormatter_TMP : MonoBehaviour
{
    public string format = "F";
    public string unit = "";
    public bool addSpaceBetweenUnit = false;


    public void FormatString(float number)
    {
        var text = number.ToString(format, CultureInfo.InvariantCulture) + (addSpaceBetweenUnit? " " : "") + unit;
        if (GetComponent<TMP_InputField>())
            GetComponent<TMP_InputField>().text = text;
        else if (GetComponent<TextMeshProUGUI>())
            GetComponent<TextMeshProUGUI>().text = text;
        else if (GetComponent<TextMeshPro>())
            GetComponent<TextMeshPro>().text = text;
    }
}
