using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class scrMenuColumnMainMenu : MonoBehaviour
{
    // #################################################################################################################
    // Members

    private scrMenu Menu;

    private float TimeScaleRestore = 1.0f;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Columns

    [SerializeField] private GameObject ColumnAudio;

    [SerializeField] private GameObject ColumnLanguage;

    [SerializeField] private GameObject ColumnNetwork;

    [SerializeField] private GameObject ColumnCredits;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Buttons

    [SerializeField] private GameObject ButtonAudio;

    [SerializeField] private GameObject ButtonLanguage;

    [SerializeField] private GameObject ButtonNetwork;

    [SerializeField] private GameObject ButtonCredits;

    [SerializeField] private GameObject ButtonExit;
    
    // #################################################################################################################
    // Methods

    private void Start()
    {
        // Link scrMenu
        // TODO: This is ugly and needs to get fixed
        this.Menu = (scrMenu) this.transform.parent.parent.parent.GetComponent(typeof(scrMenu));

        // Link button actions
        this.ButtonAudio.GetComponent<Button>().onClick.AddListener(() => this.OnClickAudio());
        this.ButtonLanguage.GetComponent<Button>().onClick.AddListener(() => this.OnClickLanguage());
        this.ButtonNetwork.GetComponent<Button>().onClick.AddListener(() => this.OnClickNetwork());
        this.ButtonCredits.GetComponent<Button>().onClick.AddListener(() => this.OnClickCredits());
        this.ButtonExit.GetComponent<Button>().onClick.AddListener(() => this.OnClickExit());
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
        this.ClearButtonActiveIcons();
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Button Actions

    private void OnClickAudio()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnAudio);
        this.ClearButtonActiveIcons();
        this.ButtonAudio.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
    }

    private void OnClickLanguage()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnLanguage);
        this.ClearButtonActiveIcons();
        this.ButtonLanguage.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
    }

    private void OnClickNetwork()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnNetwork);
        this.ClearButtonActiveIcons();
        this.ButtonLanguage.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
    }

    private void OnClickCredits()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnCredits);
        this.ClearButtonActiveIcons();
        this.ButtonLanguage.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
    }

    private void OnClickExit()
    {
        Application.Quit();
    }

    private void ClearButtonActiveIcons()
    {
        this.ButtonAudio.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        this.ButtonLanguage.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        this.ButtonNetwork.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        this.ButtonCredits.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
    }
}
