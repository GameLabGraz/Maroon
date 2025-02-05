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
        SpawnNewColumn(ColumnLaboratory, ButtonLaboratory);
    }

    private void OnClickSettings()
    {
        SpawnNewColumn(ColumnSettings, ButtonSettings);
    }

    private void OnClickCredits()
    {
        SpawnNewColumn(ColumnCredits, ButtonCredits);
    }

    private void SpawnNewColumn(GameObject newColumn, GameObject pressedButton)
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(newColumn);
        this.ClearButtonActiveIcons();
        this.SetButtonActiveIcon(pressedButton);
    }

    private void OnClickExit()
    {
        Application.Quit();
    }

    private void ClearButtonActiveIcons()
    {
        GameObject[] buttons = new GameObject[] {
            ButtonLaboratory,
            ButtonSettings,
            ButtonCredits
        };
        Color clr = Color.clear;

        foreach (GameObject button in buttons)
        {
            button.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = clr;
        }
    }

    private void SetButtonActiveIcon(GameObject btn)
    {
        btn.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
    }
}
