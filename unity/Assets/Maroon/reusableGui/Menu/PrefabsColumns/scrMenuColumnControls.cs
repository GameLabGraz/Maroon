using Maroon.GlobalEntities.ControlsManager;
using UnityEngine;
using UnityEngine.UI;


public class scrMenuColumnControls : MonoBehaviour
{
    [Header("Buttons/Sliders")]
    [SerializeField] private Slider _mouseSensitivitySlider;

    private void Start()
    {
        if (_mouseSensitivitySlider)
        {
            _mouseSensitivitySlider.value = ControlsManager.Instance.MouseSensitivity;
            _mouseSensitivitySlider.onValueChanged.AddListener(this.OnChangeMouseSensitivitySlider);
        }
    }

    public void OnChangeMouseSensitivitySlider(float value)
    {
        ControlsManager.Instance.MouseSensitivity = value;
    }
}
