using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class scrMenuColumnPauseMenu : MonoBehaviour
{
    // #################################################################################################################
    // Members

    public static bool IsPaused = false;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Pause Menu items

    private scrMenu Menu;

    [SerializeField] private GameObject ButtonAudio;

    [SerializeField] private GameObject ButtonLanguage;

    [SerializeField] private GameObject ButtonMainMenu;

    [SerializeField] private GameObject ButtonResume;
    
    // #################################################################################################################
    // Methods

    private void Start()
    {
        // TODO: This is ugly and needs to get fixed
        this.Menu = (scrMenu) this.transform.parent.parent.parent.GetComponent(typeof(scrMenu));
        print(this.Menu);

        
        this.ButtonAudio.GetComponent<Button>().onClick.AddListener(() => this.OnClickAudio());
        this.ButtonLanguage.GetComponent<Button>().onClick.AddListener(() => this.OnClickLanguage());
        this.ButtonMainMenu.GetComponent<Button>().onClick.AddListener(() => this.OnClickMainMenu());
        this.ButtonResume.GetComponent<Button>().onClick.AddListener(() => this.OnClickResume());
    }

    void OnEnable()
    {
        Time.timeScale = 0;
    }

    void OnDisable()
    {
        Time.timeScale = 1;
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Button Actions

    private void OnClickAudio()
    {

    }

    private void OnClickLanguage()
    {

    }

    private void OnClickMainMenu()
    {

    }

    private void OnClickResume()
    {
        this.Menu.CloseMenu();
    }
}
