using Localization;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Slider musicSlider;
    public Slider soundSlider;

    public Toggle ger;
    public Toggle eng;

    public AudioSource highlightSound;
    public AudioSource closeSound;
    
    private void Start()
    {
        musicSlider.value = SoundManager.Instance.MusicVolume;
        soundSlider.value = SoundManager.Instance.EfxVolume;
        highlightSound.volume = SoundManager.Instance.EfxVolume;
        closeSound.volume = SoundManager.Instance.EfxVolume;

        switch (LanguageManager.Instance.CurrentLanguage)
        {
            case SystemLanguage.German:
                ger.isOn = true;
                break;
            default:
                eng.isOn = true;
                break;
        }
        UpdateLocalizedText();
    }

    public void ClickOnGerman()
    {
        LanguageManager.Instance.CurrentLanguage = SystemLanguage.German;
        UpdateLocalizedText();
    }

    public void ClickOnEnglish()
    {
        LanguageManager.Instance.CurrentLanguage = SystemLanguage.English;
        UpdateLocalizedText();
    }

    public void SetMusicVolume()
    {
        SoundManager.Instance.MusicVolume = musicSlider.value;
    }

    public void SetSoundVolume()
    {
        SoundManager.Instance.EfxVolume = soundSlider.value;
        highlightSound.volume = soundSlider.value;
        closeSound.volume = soundSlider.value;
    }

    private void UpdateLocalizedText()
    {
        foreach(var text in GameObject.FindObjectsOfType<LocalizedText>())
            text.UpdateLocalizedText();
    }
}
