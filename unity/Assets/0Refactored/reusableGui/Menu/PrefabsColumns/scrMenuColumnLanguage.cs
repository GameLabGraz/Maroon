using GEAR.Localization;
using UnityEngine;
using UnityEngine.UI;

public class scrMenuColumnLanguage : MonoBehaviour
{
    [SerializeField] private GameObject ButtonEnglish;

    [SerializeField] private GameObject ButtonGerman;

    [SerializeField] private Texture TextureIconBase;

    [SerializeField] private Texture TextureIconSelected;


    private void Start()
    {
        this.ButtonGerman.GetComponent<Button>().onClick.AddListener(() => this.OnClickGerman());
        this.ButtonEnglish.GetComponent<Button>().onClick.AddListener(() => this.OnClickEnglish());
        this.UpdateLocalizedText();
        this.UpdateActiveButton();
    }

    private void OnClickGerman()
    {
        LanguageManager.Instance.CurrentLanguage = SystemLanguage.German;
        this.UpdateLocalizedText();
        this.UpdateActiveButton();
    }

    private void OnClickEnglish()
    {
        LanguageManager.Instance.CurrentLanguage = SystemLanguage.English;
        this.UpdateLocalizedText();
        this.UpdateActiveButton();
    }

    private void UpdateLocalizedText()
    {
        // Update text in all localized elements
        foreach(var text in GameObject.FindObjectsOfType<LocalizedText>())
        {
            text.UpdateLocalizedText();
        }
    }

    private void UpdateActiveButton()
    {
        var icon_english = this.ButtonEnglish.transform.Find("IconContainer").Find("Icon").GetComponent<RawImage>();
        var icon_german = this.ButtonGerman.transform.Find("IconContainer").Find("Icon").GetComponent<RawImage>();

        icon_english.texture = this.TextureIconBase;
        icon_german.texture = this.TextureIconBase;

        if(LanguageManager.Instance.CurrentLanguage == SystemLanguage.English)
        {
            icon_english.texture = this.TextureIconSelected;
        }

        else if (LanguageManager.Instance.CurrentLanguage == SystemLanguage.German)
        {
            icon_german.texture = this.TextureIconSelected;
        }
    }
}
