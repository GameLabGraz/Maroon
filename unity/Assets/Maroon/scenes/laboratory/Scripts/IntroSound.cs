using GEAR.Localization;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class IntroSound : MonoBehaviour
{
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

        PlayIntro();
    }

    public void PlayIntro()
    {
        _audioSource.Play();
    }
}
