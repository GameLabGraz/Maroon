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
        // Listen to music slider
        _musicSlider.GetComponent<Slider>().onValueChanged
                    .AddListener((value) => this.OnChangeMusicSlider(value));

        // Listen to sound effect slider
        _soundEffectSlider.GetComponent<Slider>().onValueChanged
                          .AddListener((value) => this.OnChangeSoundEffectSlider(value));
    }

    void OnChangeMusicSlider(float value)
    {
        SoundManager.Instance.MusicVolume = value;
    }

    void OnChangeSoundEffectSlider(float value)
    {
        SoundManager.Instance.SoundEffectVolume = value;
    }
}
