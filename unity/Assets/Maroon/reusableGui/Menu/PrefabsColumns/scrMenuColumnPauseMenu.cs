using Maroon.GlobalEntities;
using UnityEngine;
using UnityEngine.UI;


public class scrMenuColumnPauseMenu : MonoBehaviour
{
    // #################################################################################################################
    // Members

    private scrMenu Menu;

    private float TimeScaleRestore = 1.0f;

    [SerializeField] private Maroon.CustomSceneAsset targetMainMenuScenePC;

    [SerializeField] private Maroon.CustomSceneAsset targetMainMenuSceneVR;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Columns

    [SerializeField] private GameObject ColumnSettings;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Buttons

    [SerializeField] private GameObject ButtonSettings;

    [SerializeField] private GameObject ButtonMainMenu;

    [SerializeField] private GameObject ButtonResume;
    
    // #################################################################################################################
    // Methods

    private void Start()
    {
        // Link scrMenu
        // TODO: This is ugly and needs to get fixed
        this.Menu = (scrMenu) this.transform.parent.parent.parent.GetComponent(typeof(scrMenu));

        // Link button actions
        this.ButtonSettings.GetComponent<Button>().onClick.AddListener(() => this.OnClickSettings());
        this.ButtonMainMenu.GetComponent<Button>().onClick.AddListener(() => this.OnClickMainMenu());
        this.ButtonResume.GetComponent<Button>().onClick.AddListener(() => this.OnClickResume());
    }

    void OnEnable()
    {
        this.TimeScaleRestore = Time.timeScale;
        Time.timeScale = 0;
    }

    void OnDisable()
    {
        Time.timeScale = this.TimeScaleRestore;
        this.TimeScaleRestore = 1.0f;
        ClearButtonActiveIcons();
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Button Actions

    private void OnClickSettings()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnSettings);
        ClearButtonActiveIcons();
        SetButtonActiveIcon(ButtonSettings);
    }

    private void OnClickMainMenu()
    {
        if(PlatformManager.Instance.CurrentPlatformIsVR)
        {
            SceneManager.Instance.LoadSceneRequest(this.targetMainMenuSceneVR);
        }
        
        else
        {
            SceneManager.Instance.LoadSceneRequest(this.targetMainMenuScenePC);
        }
        this.Menu.CloseMenu();
    }

    private void OnClickResume()
    {
        this.Menu.CloseMenu();
    }


    private void ClearButtonActiveIcons()
    {
        GameObject[] buttons = new GameObject[] {
            ButtonSettings
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
