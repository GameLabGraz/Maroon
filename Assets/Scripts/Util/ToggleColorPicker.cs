using PlatformControls.BaseControls;
using UnityEngine;


public class ToggleColorPicker : MonoBehaviour
{
    private bool _colorPickerEnabled = false;

    private ColorPicker[] _colorPickers;

    private void Start()
    {
        _colorPickers = GameObject.FindObjectsOfType<ColorPicker>();
        EnableColorPickers(false);
    }

    private void EnableColorPickers(bool enabled)
    {
        foreach (var colorPicker in _colorPickers)
            colorPicker.enabled = enabled;
    }

    public void ToggleColorPickers()
    {
        _colorPickerEnabled ^= true;
        EnableColorPickers(_colorPickerEnabled);
    }
}
