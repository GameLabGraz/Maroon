using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Neues System für Loaden von Level und Screens
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    //UI elements
    public Text language;
    public Text maudio;
    public Text achievements;
    public Text resume;
    public Text back;
    public Text back2;
    public Text back3;
    public Text music;
    public Text sound;
    public Text language2;
    public Text german;
    public Text english;
    public Text french;
    public Text deactivate;
    public Text yes;
    public Text no;
    public Text achievement;


    public Slider musicSlider;
    public Slider soundSlider;

    public Toggle ger;
    public Toggle eng;
    public Toggle fre;

    public Toggle toggleYes;
    public Toggle toggleNo;

    public AudioSource highlightSound;
    public AudioSource closeSound;

    



    void Start()
    {
        musicSlider.value = SoundManager.instance.musicSource.volume;
        soundSlider.value = SoundManager.instance.efxSource.volume;
        Language current = GamificationManager.instance.l_manager.currentLanguage;
        if (current == Language.English)
            eng.isOn = true;
        else if (current == Language.German)
            ger.isOn = true;
        else if (current == Language.French)
            fre.isOn = true;
        UpdateLanguage();
        if (GamificationManager.instance.DeactivateDialogue)
            toggleYes.isOn = true;
        else
            toggleNo.isOn = true;
    }

    public void ClickOnGerman()
    {        
           GamificationManager.instance.l_manager.currentLanguage = Language.German;
           UpdateLanguage();
    }

    public void DeactivateDialogue()
    {
        GamificationManager.instance.DeactivateDialogue = false;
    }

    public void ActivateDialogue()
    {
        GamificationManager.instance.DeactivateDialogue = true;
    }

    public void Resume()
   {
       GamificationManager.instance.Resume();
   }

   public void ClickOnEnglish()
   {      
               GamificationManager.instance.l_manager.currentLanguage = Language.English;
               UpdateLanguage();
    }

    public void ClickOnFrench()
    {      
            GamificationManager.instance.l_manager.currentLanguage = Language.French;
            UpdateLanguage();       
    }

    void UpdateLanguage()
    {
        language.text = GamificationManager.instance.l_manager.GetString("Menu Language");
        maudio.text = GamificationManager.instance.l_manager.GetString("Menu Audio");
        achievements.text = GamificationManager.instance.l_manager.GetString("Menu Achievements");
        resume.text = GamificationManager.instance.l_manager.GetString("Menu Resume");
        back.text = GamificationManager.instance.l_manager.GetString("Menu Back");
        back2.text = GamificationManager.instance.l_manager.GetString("Menu Back");
        back3.text = GamificationManager.instance.l_manager.GetString("Menu Back");
        music.text = GamificationManager.instance.l_manager.GetString("Menu Music");
        sound.text = GamificationManager.instance.l_manager.GetString("Menu Sound");
        language2.text = GamificationManager.instance.l_manager.GetString("Menu Language 2");
        german.text = GamificationManager.instance.l_manager.GetString("German");
        english.text = GamificationManager.instance.l_manager.GetString("English");
        french.text = GamificationManager.instance.l_manager.GetString("French");
        deactivate.text = GamificationManager.instance.l_manager.GetString("Deactivate");
        yes.text = GamificationManager.instance.l_manager.GetString("Yes");
        no.text = GamificationManager.instance.l_manager.GetString("No");
        achievement.text = GamificationManager.instance.l_manager.GetString("Menu Achievements") + ": " +
            GamificationManager.instance.FinishedAchievements + "/" + GamificationManager.instance.getNumberOfAchievements();


    }

    private void Update()
    {
        SoundManager.instance.SetMusicVolume(musicSlider.value);
        SoundManager.instance.SetEfxVolume(soundSlider.value);
        highlightSound.volume = SoundManager.instance.efxSource.volume;
        closeSound.volume = SoundManager.instance.efxSource.volume;
        if (musicSlider.value != 0)
            GamificationManager.instance.Headset = true;
        else
            GamificationManager.instance.Headset = false;
    }
}
