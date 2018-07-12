using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISelectable : MonoBehaviour
{
    public Button button;
    public Text text;

    public void init(string title, UnityAction action)
    {
        text.text = title;
        button.onClick.AddListener(action);
    }
}
