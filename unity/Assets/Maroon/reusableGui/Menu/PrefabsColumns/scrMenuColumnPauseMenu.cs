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

    [SerializeField] private GameObject ColumnAudio;

    [SerializeField] private GameObject ColumnLanguage;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Buttons

    [SerializeField] private GameObject ButtonAudio;

    [SerializeField] private GameObject ButtonLanguage;

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
        this.ButtonAudio.GetComponent<Button>().onClick.AddListener(() => this.OnClickAudio());
        this.ButtonLanguage.GetComponent<Button>().onClick.AddListener(() => this.OnClickLanguage());
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
        this.ButtonAudio.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        this.ButtonLanguage.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Button Actions

    private void OnClickAudio()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnAudio);
        this.ButtonAudio.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
        this.ButtonLanguage.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
    }

    private void OnClickLanguage()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnLanguage);
        this.ButtonAudio.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        this.ButtonLanguage.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
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
}
