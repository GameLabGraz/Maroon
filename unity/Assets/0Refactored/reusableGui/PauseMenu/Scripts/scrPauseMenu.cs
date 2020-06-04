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

    private int OpenPanelId = -1;

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

    private void Start()
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
            int openPanelId = i;
            button.GetComponent<Button>().onClick.AddListener(delegate {this.ToggleSettingsPanel(openPanelId);});
        }
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Open/Close Pause Menu

    private void Update()
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

    private void ClosePauseMenu()
    {
        this.Canvas.SetActive(false);
        Time.timeScale = 1;
        scrPauseMenu.IsPaused = false;
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Open/Close Settings

    private void ToggleSettingsPanel(int panel_id)
    {
        // Gather info
        GameObject requested_panel = this.SettingPanels[panel_id];
        bool open_requested = !requested_panel.activeSelf;

        // Close all panels
        this.PanelRight.SetActive(false);
        foreach(GameObject SettingPanel in this.SettingPanels)
        {
            this.OpenPanelId = -1;
            SettingPanel.SetActive(false);            
        }

        // Open correct panel if requested
        if(open_requested)
        {
            this.OpenPanelId = panel_id;
            this.UpdateRightPanelName();
            requested_panel.SetActive(true);
            this.PanelRight.SetActive(true);
        }
    }

    public void UpdateRightPanelName()
    {
        string title = this.SettingButtons[this.OpenPanelId].GetComponentInChildren<TextMeshProUGUI>().text;
        this.PanelRightTitle.GetComponentInChildren<TextMeshProUGUI>().text = title;
    }
    
}
