using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class scrMenuColumnSettings : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button ButtonAudio;
    [SerializeField] private Button ButtonLanguage;
    [SerializeField] private Button ButtonControls;

    [Header("Columns")]
    [SerializeField] private GameObject ColumnAudio;
    [SerializeField] private GameObject ColumnLanguage;
    [SerializeField] private GameObject ColumnControls;

    private scrMenu menu;

    private void Start()
    {
        // Link scrMenu
        menu = FindObjectOfType<scrMenu>();

        // Link button actions
        ButtonAudio.onClick.AddListener(() => OnClickAudio());
        ButtonLanguage.onClick.AddListener(() => OnClickLanguage());
        ButtonControls.onClick.AddListener(() => OnClickControls());
    }

    private void OnClickAudio()
    {
        ClickSpecificSettingsButton(ColumnAudio, ButtonAudio);
    }

    private void OnClickLanguage()
    {
        ClickSpecificSettingsButton(ColumnLanguage, ButtonLanguage);
    }

    private void OnClickControls()
    {
        ClickSpecificSettingsButton(ColumnControls, ButtonControls);
    }

    private void ClickSpecificSettingsButton(GameObject specificColumn, Button pressedButton)
    {
        menu.RemoveAllMenuColumnsButTwo();
        menu.AddMenuColumn(specificColumn);
        ClearButtonActiveIcons();
        SetButtonActiveIcon(pressedButton);
    }

    private void ClearButtonActiveIcons()
    {
        List<Button> buttons = new List<Button> { 
            ButtonAudio, 
            ButtonLanguage, 
            ButtonControls 
        };
        Color clr = Color.clear;

        foreach (Button button in buttons)
        {
            button.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = clr;
        }
    }

    private void SetButtonActiveIcon(Button btn)
    {
        btn.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
    }
}
