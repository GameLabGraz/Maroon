using Maroon.GlobalEntities.ControlsManager;
using UnityEngine;
using UnityEngine.UI;


public class scrMenuColumnControls : MonoBehaviour
{
    public const float MIN_MOUSE_SENSITIVITY = 50;
    public const float MAX_MOUSE_SENSITIVITY = 1000;

    [Header("Buttons/Sliders")]
    [SerializeField] private Slider _mouseSensitivitySlider;

    
    private void Start()
    {
        if (_mouseSensitivitySlider)
        {
            // Set Slider range from 0 to 1
            _mouseSensitivitySlider.minValue = 0f;
            _mouseSensitivitySlider.maxValue = 1f;
            // Convert mouse sensitivity to value between 0 and 1 for the logarithmic slider
            float mouseSensitivity = ControlsManager.Instance.MouseSensitivity;
            float sliderValue01 = Mathf.Log(mouseSensitivity / MIN_MOUSE_SENSITIVITY) / Mathf.Log(MAX_MOUSE_SENSITIVITY / MIN_MOUSE_SENSITIVITY);
            _mouseSensitivitySlider.value = sliderValue01;

            _mouseSensitivitySlider.onValueChanged.AddListener(this.OnChangeMouseSensitivitySlider);
        }
    }

    public void OnChangeMouseSensitivitySlider(float sliderValue01)
    {
        // Convert slider value (betwenn 0 and 1) logarithmically to a mouse sensitvity value between min and max
        float mouseSensitivity = Mathf.Pow((MAX_MOUSE_SENSITIVITY / MIN_MOUSE_SENSITIVITY), sliderValue01) * MIN_MOUSE_SENSITIVITY;
        ControlsManager.Instance.MouseSensitivity = mouseSensitivity;
    }
}
