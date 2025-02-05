using Maroon.GlobalEntities;
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

    [SerializeField] private GameObject ColumnSettings;

    [SerializeField] private GameObject ColumnCredits;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Buttons

    [SerializeField] private GameObject ButtonLaboratory;

    [SerializeField] private GameObject ButtonSettings;

    [SerializeField] private GameObject ButtonCredits;

    [SerializeField] private GameObject ButtonExit;
    
    // #################################################################################################################
    // Methods

    private void Start()
    {
        // Link scrMenu
        this.Menu = FindObjectOfType<scrMenu>();

        // Hide exit button on WebGL
        if(PlatformManager.Instance.CurrentPlatform == Platform.WebGL)
        {
            this.ButtonExit.SetActive(false);
        }

        // Link button actions
        this.ButtonLaboratory.GetComponent<Button>().onClick.AddListener(() => this.OnClickLaboratory());
        this.ButtonSettings.GetComponent<Button>().onClick.AddListener(() => this.OnClickSettings());
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

    public void OnClickLaboratory()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnLaboratory);
        this.ClearButtonActiveIcons();
        this.SetButtonActiveIcon(this.ButtonLaboratory);
    }

    private void OnClickSettings()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnSettings);
        this.ClearButtonActiveIcons();
        this.SetButtonActiveIcon(this.ButtonSettings);
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
        this.ButtonSettings.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = clr;
        this.ButtonCredits.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = clr;
    }

    private void SetButtonActiveIcon(GameObject btn)
    {
        btn.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
    }
}
