using GEAR.Localization;
using Maroon;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class IntroSound : MonoBehaviour
{
    [SerializeField] private bool playMultiple = true;

    [SerializeField] private AudioClip germanIntro;
    [SerializeField] private AudioClip englishIntro;

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = LanguageManager.Instance.CurrentLanguage == SystemLanguage.German ? germanIntro : englishIntro;

        LanguageManager.Instance.OnLanguageChanged.AddListener((language =>
        {
            _audioSource.clip = language == SystemLanguage.German ? germanIntro : englishIntro;
        }));

        if(playMultiple) PlayIntro();
        else if (!GameManager.Instance.LabLoaded) PlayIntro();

        GameManager.Instance.LabLoaded = true;
    }

    public void PlayIntro()
    {
        _audioSource.Play();
    }
}
