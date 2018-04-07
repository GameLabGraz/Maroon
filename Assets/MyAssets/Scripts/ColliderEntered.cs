using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ColliderEntered : MonoBehaviour {

    private GameObject uiBox;
    public string LevelName;
	//public string DisplayedText;	// starts with "Press [E] " //Isnt needed anymore in the Helpi Version
	private bool insideTriggerSphere = false;
	private GUIStyle textStyle;
    //dialogue starts when colliding
    public string dialogueKey;
    //dialogue starts when left-clicking
    public string dialogueKeyClick;
    private DialogueManager dMan;
    //if this is set true in Editor, Dialogue is only played once
    public bool Just_Playing_Once; //if the dialogue should only be displayed once
    public bool noDialogue; //if there should be no dialogue
    private bool setUI = false;
    private  bool played_once; //true after dialogue has displayed once


   

        public void Awake()
	{
        uiBox = GameObject.FindWithTag("UI");
        this.textStyle = new GUIStyle("label");
		this.textStyle.alignment = TextAnchor.MiddleCenter;
        dMan = FindObjectOfType<DialogueManager>();
    }

    //This is here to disable the UI symbol when the game starts
    public void Start()
    {
        if (!GamificationManager.instance.gameStarted)
        {
            setUI = false;
            uiBox.SetActive(false);
        }
  
    }

    //Add and Delete Achievements
    void HandleAchievements()
    {
        if (LevelName == "Helpi" && !GamificationManager.instance.spokenWithHelpi)
        {
            GamificationManager.instance.DeleteAchievement("Helpi");
            GamificationManager.instance.AddAchievement("Achievement 3", "Build");
            GamificationManager.instance.spokenWithHelpi = true;
            GamificationManager.instance.finishedAchievements++;
        }
        else if ((LevelName == "VandeGraaffExperiment1" || LevelName == "FallingCoil" || LevelName == "Pendulum" //Add any new experiments here!
            || LevelName == "VandeGraafExperiment2" || LevelName == "FaradaysLaw") && !GamificationManager.instance.spokenWithLaunch)
        {
            //User plays in correct order
            if (GamificationManager.instance.isAchievementInList("Build"))
            {
                GamificationManager.instance.DeleteAchievement("Build");
                GamificationManager.instance.AddAchievement("Achievement 2", "Door");
                GamificationManager.instance.spokenWithLaunch = true;
                GamificationManager.instance.spokenWithHelpi = true;
                GamificationManager.instance.finishedAchievements++;
                Debug.Log("Correct");
            }         
            else
            {
                //User goes to experiments or door before talking to helpi
                GamificationManager.instance.DeleteAchievement("Helpi");
                GamificationManager.instance.AddAchievement("Achievement 2", "Door");
                GamificationManager.instance.spokenWithLaunch = true;
                GamificationManager.instance.spokenWithHelpi = true;
                GamificationManager.instance.finishedAchievements++;
                GamificationManager.instance.finishedAchievements++;
                Debug.Log("Incorrect");
            }
        }
        else if (LevelName == "After" && !GamificationManager.instance.spokenWithDoor)
        {
            dMan.ShowBox("Build Together");
            //Achievements
            GamificationManager.instance.DeleteAchievement("Door");
            GamificationManager.instance.AddAchievement("Achievement 4", "Build Vandegraaf1");
            GamificationManager.instance.AddAchievement("Achievement 5", "Build Vandegraaf2");
            GamificationManager.instance.AddAchievement("Achievement 6", "Build Faradayslaw");
            GamificationManager.instance.AddAchievement("Achievement 7", "Build FallingCoil");
            GamificationManager.instance.AddAchievement("Achievement 8", "Build Pendulum");
            GamificationManager.instance.finishedAchievements++;
            GamificationManager.instance.spokenWithDoor = true;

        }
    }

    public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Player"))
        {
            setUI = true;
            uiBox.SetActive(true);
			this.insideTriggerSphere = true;

            //Achievement-Dialogues are only played once
            if (LevelName == "Helpi" && GamificationManager.instance.spokenWithHelpi ||
                ((LevelName == "VandeGraaffExperiment1" || LevelName == "FallingCoil" || LevelName == "Pendulum" //Add any new experiments here!
            || LevelName == "VandeGraafExperiment2" || LevelName == "FaradaysLaw") && GamificationManager.instance.spokenWithLaunch)
            || LevelName == "After" && GamificationManager.instance.spokenWithDoor)            
                return;
            if (!played_once && !noDialogue)
            {
                Debug.Log("Player entered");

                dMan.ShowBox(dialogueKey);

            }

            if (Just_Playing_Once)
             played_once = true;
            HandleAchievements();
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            setUI = true;
            uiBox.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
	{
		if (other.CompareTag ("Player")) {
			Debug.Log("Player exit");
			this.insideTriggerSphere = false;
            setUI = false;
            uiBox.SetActive(false);
        }
	}

	public void Update()
	{
        //Starting experiments with left-click
        if (Input.GetMouseButtonDown(0) && this.insideTriggerSphere)
        {
            if ((!GamificationManager.instance.headset && !noDialogue) || (LevelName == "Door" && !noDialogue))
            {
                //Zurzeit auskommentiert weil Funktion nicht gebraucht wird
               // dMan.ShowBox(dialogueKeyClick);
            }

            if (LevelName == "Headset")
            {
                if (!GamificationManager.instance.headset)
                {
                    GamificationManager.instance.headset = true;
                    SoundManager.instance.SetMusicVolume(1.0f);

                }

                else if (!noDialogue)
                {
                    GamificationManager.instance.headset = false;
                    SoundManager.instance.SetMusicVolume(0.0f);
                    dMan.ShowBox("Headset Away");
                }

            }


            else if (LevelName != "Helpi" && LevelName != "Door" && LevelName != "Headset")
            SceneManager.LoadScene(LevelName);

        }
    }

	public void OnGUI()
	{
		//if (this.insideTriggerSphere) {
			//GUI.Label(new Rect(Screen.width / 2 - 200f, Screen.height / 2 - 100f, 400f, 200f), "Press [E] " + DisplayedText, this.textStyle);
		//}
	}

 
}
