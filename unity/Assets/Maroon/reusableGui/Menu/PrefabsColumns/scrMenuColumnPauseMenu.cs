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

        // Enable WebGL button only for PC or Editor, non-VR Build
        Platform currentPlatform = PlatformManager.Instance.CurrentPlatform;
        if(!((currentPlatform == Platform.PC || currentPlatform == Platform.Editor) &&
           (PlatformManager.Instance.CurrentPlatformIsVR == false)))
        {
            this.ButtonNetwork.GetComponent<Button>().interactable = false;
        }
    }

    void OnEnable()
    {
        this.TimeScaleRestore = Time.timeScale;
        if (Maroon.NetworkManager.Instance == null)
        {
            Time.timeScale = 0;
            return;
        }

        if(Maroon.NetworkManager.Instance.AllowNetworkPause())
            Time.timeScale = 0;
        if(Maroon.NetworkManager.Instance.IsInControl)
            Maroon.NetworkManager.Instance.onLoseControl.Invoke();
    }

    void OnDisable()
    {
        Time.timeScale = this.TimeScaleRestore;
        this.TimeScaleRestore = 1.0f;
        this.ButtonAudio.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        this.ButtonLanguage.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        this.ButtonNetwork.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        if (Maroon.NetworkManager.Instance == null)
            return;
        if(Maroon.NetworkManager.Instance.IsInControl)
            Maroon.NetworkManager.Instance.onGetControl.Invoke();
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Button Actions

    private void OnClickAudio()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnAudio);
        this.ButtonAudio.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
        this.ButtonLanguage.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        this.ButtonNetwork.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
    }

    private void OnClickLanguage()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnLanguage);
        this.ButtonAudio.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        this.ButtonLanguage.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
        this.ButtonNetwork.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
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
    
    private void OnClickNetwork()
    {
        this.Menu.RemoveAllMenuColumnsButFirst();
        this.Menu.AddMenuColumn(this.ColumnNetwork);
        this.ButtonAudio.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        this.ButtonLanguage.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        this.ButtonNetwork.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
    }
}
