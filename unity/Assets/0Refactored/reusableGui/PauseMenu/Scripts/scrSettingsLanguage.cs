using GEAR.Localization;
using UnityEngine;

public class scrSettingsLanguage : MonoBehaviour
{
    public void OnClickGerman()
    {
        LanguageManager.Instance.CurrentLanguage = SystemLanguage.German;
        UpdateLocalizedText();
        print("d");
    }

    public void OnClickEnglish()
    {
        LanguageManager.Instance.CurrentLanguage = SystemLanguage.English;
        UpdateLocalizedText();
        print("e");
    }

    private void UpdateLocalizedText()
    {
        // Update text in all localized elements
        foreach(var text in GameObject.FindObjectsOfType<LocalizedText>())
        {
            text.UpdateLocalizedText();
        }

        // Copy new localized text to right panel title
        this.GetComponentInParent<scrPauseMenu>().UpdateRightPanelName();
    }
}
