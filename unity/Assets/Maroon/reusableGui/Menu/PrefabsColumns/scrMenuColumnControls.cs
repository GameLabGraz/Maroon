using Maroon.GlobalEntities;
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
            // TODO: _mouseSensitivitySlider.value = SoundManager.Instance.MusicVolume;
            _mouseSensitivitySlider.onValueChanged.AddListener(this.OnChangeMouseSensitivitySlider);
        }
    }

    public void OnChangeMouseSensitivitySlider(float value)
    {
        // TODO
        // e.g. SoundManager.Instance.MusicVolume = value;
    }
}
