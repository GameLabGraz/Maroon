using TMPro;
using UnityEngine;

public class WhiteboardDisplay : MonoBehaviour
{
    public TextMeshProUGUI whiteboardText;

    public void UpdateWhiteboardText(string info)
    {
        whiteboardText.text = info;
    }
}