using UnityEngine;
using UnityEngine.UI;
using VRTK;
using VRTK.UnityEventHelper;

public class ControlReactorText : MonoBehaviour
{
    public Text go;

    private VRTK_Control_UnityEvents controlEvents;

    private void Start()
    {
        controlEvents = GetComponent<VRTK_Control_UnityEvents>();
        if (controlEvents == null)
        {
            controlEvents = gameObject.AddComponent<VRTK_Control_UnityEvents>();
        }

        controlEvents.OnValueChanged.AddListener(HandleChange);
    }

    private void HandleChange(object sender, Control3DEventArgs e)
    {
        go.text = e.value.ToString() + "(" + e.normalizedValue.ToString() + "%)";
    }
}