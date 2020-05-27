using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class scrPauseMenu : MonoBehaviour
{
    // #################################################################################################################
    // Members

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // State
    public static bool IsPaused = false;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Pause Menu items
    [SerializeField] private GameObject Canvas;

    [SerializeField] private GameObject PanelRight;

    [SerializeField] private GameObject PanelRightTitle;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Pause Menu settings
    [SerializeField] private GameObject[] SettingButtons;

    [SerializeField] private GameObject[] SettingPanels;
    
    // #################################################################################################################
    // Methods

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Setup

    void Start()
    {
        // Hide menu and panels
        this.Canvas.SetActive(false);
        this.PanelRight.SetActive(false);
        foreach(GameObject SettingPanel in this.SettingPanels)
        {
            SettingPanel.SetActive(false);            
        }

        // Add button actions
        for(int i = 0; i < this.SettingButtons.Length; i++)
        {
            GameObject button = this.SettingButtons[i];

            // Delegates passed by reference, hacky workaround
            int openPanel = i;
            button.GetComponent<Button>().onClick.AddListener(delegate {this.ToggleSettingsPanel(openPanel);});
        }
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Open/Close Pause Menu

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(scrPauseMenu.IsPaused)
            {
                this.ClosePauseMenu();
            }
            else
            {
                this.OpenPauseMenu();
            }
        }
    }

    private void OpenPauseMenu()
    {
        this.Canvas.SetActive(true);
        Time.timeScale = 0;
        scrPauseMenu.IsPaused = true;
    }

    public void ClosePauseMenu()
    {
        this.Canvas.SetActive(false);
        Time.timeScale = 1;
        scrPauseMenu.IsPaused = false;
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Open/Close Settings

    void ToggleSettingsPanel(int panel_id)
    {
        // Gather info
        GameObject requested_panel = this.SettingPanels[panel_id];
        bool open_requested = !requested_panel.activeSelf;

        // Close all panels
        this.PanelRight.SetActive(false);
        foreach(GameObject SettingPanel in this.SettingPanels)
        {
            SettingPanel.SetActive(false);            
        }

        // Open correct panel if requested
        if(open_requested)
        {
            string title = this.SettingButtons[panel_id].GetComponentInChildren<TextMeshProUGUI>().text;
            this.PanelRightTitle.GetComponentInChildren<TextMeshProUGUI>().text = title;
            requested_panel.SetActive(true);
            this.PanelRight.SetActive(true);
        }
    }
    
}
