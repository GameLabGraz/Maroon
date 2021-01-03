using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class scrMenuColumnMainMenu : MonoBehaviour
{
    // #################################################################################################################
    // Members

    private scrMenu Menu;

    private float TimeScaleRestore = 1.0f;

    [SerializeField] private Maroon.CustomSceneAsset targetLabScenePC;

    [SerializeField] private Maroon.CustomSceneAsset targetLabSceneVR;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Columns

    [SerializeField] private GameObject ColumnAudio;

    [SerializeField] private GameObject ColumnLanguage;

    [SerializeField] private GameObject ColumnCredits;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Buttons

    [SerializeField] private GameObject ButtonLab;

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
        Debug.Log(Maroon.PlatformManager.Instance.CurrentPlatform);
        if(Maroon.PlatformManager.Instance.CurrentPlatform == Maroon.Platform.WebGL)
        {
            this.ButtonExit.SetActive(false);
        }

        // Link button actions
        this.ButtonLab.GetComponent<Button>().onClick.AddListener(() => this.OnClickLab());
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

    private void OnClickLab()
    {
        if(Maroon.PlatformManager.Instance.CurrentPlatformIsVR)
        {
            SceneManager.LoadScene(this.targetLabSceneVR);
        }

        else
        {
            SceneManager.LoadScene(this.targetLabScenePC);
        }
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
        this.ButtonAudio.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = clr;
        this.ButtonLanguage.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = clr;
        this.ButtonCredits.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = clr;
    }

    private void SetButtonActiveIcon(GameObject btn)
    {
        btn.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
    }
}
