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

    protected void Start()
    {
        var germanBtn = germanButton.GetComponent<VRHoverButton>();
        if (germanBtn)
        {
            germanBtn.OnButtonOn.AddListener(() => OnSwitchToGerman());
            germanBtn.OnButtonOff.AddListener(() => OnSwitchToEnglish());
        }

        var englishBtn = englishButton.GetComponent<VRHoverButton>();
        if (englishBtn)
        {
            englishBtn.OnButtonOn.AddListener(() => OnSwitchToEnglish());
            englishBtn.OnButtonOff.AddListener(() => OnSwitchToGerman());
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
