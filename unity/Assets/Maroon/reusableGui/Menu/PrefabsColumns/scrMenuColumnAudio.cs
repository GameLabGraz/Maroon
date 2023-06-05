using Maroon.GlobalEntities;
using UnityEngine;
using UnityEngine.UI;


public class scrMenuColumnAudio : MonoBehaviour
{
    // #################################################################################################################
    // Members

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Buttons/Sliders

    [SerializeField] private GameObject _musicSlider;

    [SerializeField] private GameObject _soundEffectSlider;

    // #################################################################################################################
    // Methods

    void Start()
    {
        // Apply initial slider value from Music AudioSource and listen to music slider
        if (_musicSlider)
        {
            var musicSliderComponent = _musicSlider.GetComponent<Slider>();
            musicSliderComponent.value = SoundManager.Instance.MusicVolume;
            musicSliderComponent.onValueChanged.AddListener(this.OnChangeMusicSlider);
        }
        
        // Apply initial slider value from Sound Effect AudioSource and listen to sound effect slider
        if (_soundEffectSlider)
        {
            var soundEffectSliderComponent = _soundEffectSlider.GetComponent<Slider>();
            soundEffectSliderComponent.value = SoundManager.Instance.SoundEffectVolume;
            soundEffectSliderComponent.onValueChanged.AddListener(this.OnChangeSoundEffectSlider);
        }
    }

    public void OnChangeMusicSlider(float value)
    {
        SoundManager.Instance.MusicVolume = value;
    }

    public void OnChangeSoundEffectSlider(float value)
    {
        SoundManager.Instance.SoundEffectVolume = value;
    }
}
