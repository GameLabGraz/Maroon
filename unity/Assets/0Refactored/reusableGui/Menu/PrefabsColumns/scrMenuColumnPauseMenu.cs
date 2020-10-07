using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class scrMenuColumnPauseMenu : MonoBehaviour
{
    // #################################################################################################################
    // Members

    private scrMenu Menu;

    private float TimeScaleRestore = 1.0f;

    [SerializeField] private Utilities.SceneField targetMainMenuScenePC;

    [SerializeField] private Utilities.SceneField targetMainMenuSceneVR;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Columns

    [SerializeField] private GameObject ColumnAudio;

    [SerializeField] private GameObject ColumnLanguage;
    
    [SerializeField] private GameObject ColumnNetwork;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Buttons

    [SerializeField] private GameObject ButtonAudio;

    [SerializeField] private GameObject ButtonLanguage;

    [SerializeField] private GameObject ButtonMainMenu;

    [SerializeField] private GameObject ButtonResume;
    
    [SerializeField] private GameObject ButtonNetwork;
    
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
        this.ButtonNetwork.GetComponent<Button>().onClick.AddListener(() => this.OnClickNetwork());
    }

    void OnEnable()
    {
        this.TimeScaleRestore = Time.timeScale;
        if (MaroonNetworkManager.Instance == null)
        {
            Time.timeScale = 0;
            return;
        }

        if(MaroonNetworkManager.Instance.AllowNetworkPause())
            Time.timeScale = 0;
        if(MaroonNetworkManager.Instance.IsInControl)
            MaroonNetworkManager.Instance.onLoseControl.Invoke();
    }

    void OnDisable()
    {
        Time.timeScale = this.TimeScaleRestore;
        this.TimeScaleRestore = 1.0f;
        this.ButtonAudio.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        this.ButtonLanguage.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        if (MaroonNetworkManager.Instance == null)
            return;
        if(MaroonNetworkManager.Instance.IsInControl)
            MaroonNetworkManager.Instance.onGetControl.Invoke();
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
        if(TargetPlatformDetector.isVRPlatform)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(this.targetMainMenuSceneVR);
        }
        
        else
        {
            //UnityEngine.SceneManagement.SceneManager.LoadScene(this.targetMainMenuScenePC);
            MaroonNetworkManager.Instance.EnterScene(this.targetMainMenuScenePC);
        }
        this.Menu.CloseMenu();
    }

    private void OnClickResume()
    {
        this.Menu.CloseMenu();
    }
    
    private void OnClickNetwork()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnNetwork);
    }
}
