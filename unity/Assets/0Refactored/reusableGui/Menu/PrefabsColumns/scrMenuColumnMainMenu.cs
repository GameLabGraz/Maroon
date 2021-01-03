using UnityEngine;
using UnityEngine.UI;

public class scrMenuColumnMainMenu : MonoBehaviour
{
    // #################################################################################################################
    // Members

    private scrMenu Menu;

    private float TimeScaleRestore = 1.0f;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Columns

    [SerializeField] private GameObject ColumnLaboratory;

    [SerializeField] private GameObject ColumnAudio;

    [SerializeField] private GameObject ColumnLanguage;

    [SerializeField] private GameObject ColumnCredits;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Buttons

    [SerializeField] private GameObject ButtonLaboratory;

    [SerializeField] private GameObject ButtonAudio;

    [SerializeField] private GameObject ButtonLanguage;

    [SerializeField] private GameObject ButtonCredits;

    [SerializeField] private GameObject ButtonExit;
    
    // #################################################################################################################
    // Methods

    private void Start()
    {
        // Link scrMenu
        // TODO: This is ugly and needs to get fixed
        this.Menu = (scrMenu) this.transform.parent.parent.parent.GetComponent(typeof(scrMenu));

        // Hide exit button on WebGL
        if(Maroon.PlatformManager.Instance.CurrentPlatform == Maroon.Platform.WebGL)
        {
            this.ButtonExit.SetActive(false);
        }

        // Link button actions
        this.ButtonLaboratory.GetComponent<Button>().onClick.AddListener(() => this.OnClickLaboratory());
        this.ButtonAudio.GetComponent<Button>().onClick.AddListener(() => this.OnClickAudio());
        this.ButtonLanguage.GetComponent<Button>().onClick.AddListener(() => this.OnClickLanguage());
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

    private void OnClickLaboratory()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnLaboratory);
        this.ClearButtonActiveIcons();
        this.SetButtonActiveIcon(this.ButtonLaboratory);
    }

    private void OnClickAudio()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnAudio);
        this.ClearButtonActiveIcons();
        this.SetButtonActiveIcon(this.ButtonAudio);
    }

    private void OnClickLanguage()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnLanguage);
        this.ClearButtonActiveIcons();
        this.SetButtonActiveIcon(this.ButtonLanguage);
    }

    private void OnClickCredits()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnCredits);
        this.ClearButtonActiveIcons();
        this.SetButtonActiveIcon(this.ButtonCredits);
    }

    private void OnClickExit()
    {
        Application.Quit();
    }

    private void ClearButtonActiveIcons()
    {
        Color clr = Color.clear;
        this.ButtonLaboratory.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = clr;
        this.ButtonAudio.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = clr;
        this.ButtonLanguage.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = clr;
        this.ButtonCredits.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = clr;
    }

    private void SetButtonActiveIcon(GameObject btn)
    {
        btn.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
    }
}
