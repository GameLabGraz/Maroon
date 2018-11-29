using UnityEngine;


public class ToggleColorPicker : MonoBehaviour
{
    private bool colorPickerEnabled = false;

    private ColorPicker[] colorPickers;

    private void Start()
    {
        colorPickers = GameObject.FindObjectsOfType<ColorPicker>();
        EnableColorPickers(false);
    }

    private void EnableColorPickers(bool enabled)
    {
        foreach (ColorPicker colorPicker in colorPickers)
            colorPicker.enabled = enabled;
    }

    public void ToggleColorPickers()
    {
        colorPickerEnabled ^= true;
        EnableColorPickers(colorPickerEnabled);
    }
}
