using MaroonVR;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Valve.VR;
using Valve.VR.InteractionSystem;


public class scrPauseMenu : MonoBehaviour
{
    // #################################################################################################################
    // Members

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // State
    public static bool IsPaused = false;

    private int OpenPanelId = -1;
    private Player player = null;
    private bool isVR = false;
    private bool vrLastPressed = false;
    private bool vrShowHint = true;
    
    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Pause Menu items
    [SerializeField] private GameObject Canvas;

    [SerializeField] private GameObject PanelRight;

    [SerializeField] private GameObject PanelRightTitle;

    [SerializeField] protected SteamVR_Action_Boolean menuAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("onMenu");

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

        isVR = SceneManager.GetActiveScene().name.EndsWith(".vr");
        isVR = true;
        if (isVR && player == null)
        {
            Debug.Log("Pause Menu");
            player = Valve.VR.InteractionSystem.Player.instance;
            isVR = player != null;

            foreach (var hand in player.hands)
            {
                Debug.Log("Show Pause Menu hint");
                ControllerButtonHints.ShowTextHint(hand, menuAction, "Menu");
            }
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

        if (isVR)
        {
            foreach (var hand in player.hands)
            {
                if (!menuAction.GetStateDown(hand.handType)) continue;
                
                if(vrShowHint){
                    foreach (var hintHand in player.hands)
                    {
                        ControllerButtonHints.HideTextHint(hintHand, menuAction);
                    }
                }
                
                vrShowHint = false;
                if(IsPaused) ClosePauseMenu();
                else OpenPauseMenu();
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
