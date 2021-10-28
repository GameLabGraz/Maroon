using System;
using GEAR.Localization;
using GEAR.VRInteraction;
using UnityEngine;
using UnityEngine.Events;

public class LanguageChangeHelper : MonoBehaviour
{
    [SerializeField] private UnityEvent onSwitchToGerman;
    [SerializeField] private UnityEvent onSwitchToEnglish;

    [SerializeField] private GameObject germanButton;
    [SerializeField] private GameObject englishButton;
    
    private bool _ignoreEvents = false;

    protected void Start()
    {
        var germanBtn = germanButton.GetComponent<VRHoverButton>();
        var englishBtn = englishButton.GetComponent<VRHoverButton>();
        if (germanBtn && englishBtn)
        {
            if (LanguageManager.Instance.CurrentLanguage == SystemLanguage.German)
            {
                germanBtn.ForceButtonState(true);
            }
            else
            {
                englishBtn.ForceButtonState(true);
            }
            
            
            germanBtn.OnButtonOn.AddListener(() =>
            {
                if (_ignoreEvents) return;
                _ignoreEvents = true;
                OnSwitchToGerman();
                englishBtn.ForceButtonState(false);
                _ignoreEvents = false;
            });
            germanBtn.OnButtonOff.AddListener(() =>
            {
                if (_ignoreEvents) return;
                _ignoreEvents = true;
                OnSwitchToEnglish();
                englishBtn.ForceButtonState(true);
                _ignoreEvents = false;
            });
            
            englishBtn.OnButtonOn.AddListener(() =>
            {
                if (_ignoreEvents) return;
                _ignoreEvents = true;
                OnSwitchToEnglish();
                germanBtn.ForceButtonState(false);
                _ignoreEvents = false;
            });
            englishBtn.OnButtonOff.AddListener(() =>
            {
                if (_ignoreEvents) return;
                _ignoreEvents = true;
                OnSwitchToGerman();
                germanBtn.ForceButtonState(true);
                _ignoreEvents = false;
            });
        }
    }

    public void OnSwitchToGerman()
    {
        LanguageManager.Instance.CurrentLanguage = SystemLanguage.German;
        onSwitchToGerman.Invoke();
    }

    public void OnSwitchToEnglish()
    {
        LanguageManager.Instance.CurrentLanguage = SystemLanguage.English;
        onSwitchToEnglish.Invoke();
    }
}
