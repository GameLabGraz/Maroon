using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ColliderEntered : MonoBehaviour {

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
    private  bool played_once; //true after dialogue has displayed once


   

        public void Awake()
	{
        this.textStyle = new GUIStyle("label");
		this.textStyle.alignment = TextAnchor.MiddleCenter;
        dMan = FindObjectOfType<DialogueManager>();
    }



    //Add and Delete Achievements
    void HandleAchievements()
    {
        if (LevelName == "Helpi" && !GamificationManager.instance.SpokenWithHelpi)
        {
            GamificationManager.instance.DeleteAchievement("Helpi");
            GamificationManager.instance.AddAchievement("Achievement 3", "Build");
            GamificationManager.instance.SpokenWithHelpi = true;
        }
        else if ((LevelName == "VandeGraaffExperiment1" || LevelName == "FallingCoil" || LevelName == "Pendulum" //Add any new experiments here!
            || LevelName == "VandeGraaffExperiment2" || LevelName == "FaradaysLaw") && !GamificationManager.instance.SpokenWithLaunch)
        {
            //User plays in correct order
            if (GamificationManager.instance.isAchievementInList("Build"))
            {
                GamificationManager.instance.DeleteAchievement("Build");
                GamificationManager.instance.AddAchievement("Achievement 2", "Door");
                GamificationManager.instance.SpokenWithLaunch = true;
                GamificationManager.instance.SpokenWithHelpi = true;
                Debug.Log("Correct");
            }         
            else
            {
                //User goes to experiments or door before talking to helpi
                GamificationManager.instance.DeleteAchievement("Helpi");
                GamificationManager.instance.DeleteAchievement("Build");
                GamificationManager.instance.AddAchievement("Achievement 2", "Door");
                GamificationManager.instance.SpokenWithLaunch = true;
                GamificationManager.instance.SpokenWithHelpi = true;
                Debug.Log("Incorrect");
            }
        }
        else if (LevelName == "After" && !GamificationManager.instance.SpokenWithDoor)
        {
            dMan.ShowBox("Build Together");
            //Achievements
            GamificationManager.instance.DeleteAchievement("Door");
            GamificationManager.instance.AddAchievement("Achievement 8", "Build Pendulum");
            GamificationManager.instance.AddAchievement("Achievement 4", "Build Vandegraaf1");
            GamificationManager.instance.AddAchievement("Achievement 5", "Build Vandegraaf2");
            GamificationManager.instance.AddAchievement("Achievement 6", "Build Faradayslaw");
            GamificationManager.instance.AddAchievement("Achievement 7", "Build FallingCoil");
            GamificationManager.instance.SpokenWithDoor = true;

        }
    }

    public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Player"))
        {
            UIManager.instance.ShowUICollided();
			this.insideTriggerSphere = true;

            //Achievement-Dialogues are only played once
            if (LevelName == "Helpi" && GamificationManager.instance.SpokenWithHelpi ||
                ((LevelName == "VandeGraaffExperiment1" || LevelName == "FallingCoil" || LevelName == "Pendulum" //Add any new experiments here!
            || LevelName == "VandeGraaffExperiment2" || LevelName == "FaradaysLaw") && GamificationManager.instance.SpokenWithLaunch)
            || LevelName == "After" && GamificationManager.instance.SpokenWithDoor)            
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
            UIManager.instance.ShowUICollided();
        }
    }

    public void OnTriggerExit(Collider other)
	{
		if (other.CompareTag ("Player")) {
			Debug.Log("Player exit");
			this.insideTriggerSphere = false;
            UIManager.instance.HideUICollided();
        }
	}

	public void Update()
	{
        //Starting experiments with left-click
        if (Input.GetMouseButtonDown(0) && this.insideTriggerSphere)
        {
            if ((!GamificationManager.instance.Headset && LevelName == "Headset" && !noDialogue) || (LevelName == "Leave" && !noDialogue))
            {
                //Zurzeit auskommentiert weil Funktion nicht gebraucht wird
                dMan.ShowBox(dialogueKeyClick);
            }

            if (LevelName == "Headset")
            {
                if (!GamificationManager.instance.Headset)
                {
                    GamificationManager.instance.Headset = true;
                    SoundManager.instance.SetMusicVolume(1.0f);

                }

                else if (!noDialogue)
                {
                    GamificationManager.instance.Headset = false;
                    SoundManager.instance.SetMusicVolume(0.0f);
                    dMan.ShowBox("Headset Away");
                }

            }
                ChangeScene();
        }
    }

    //Change scene to new experiment if it is build correctly
    public void ChangeScene()
    {

        //Check if experiments are built
        if (LevelName == "VandeGraaffExperiment2")
        {
            if (GamificationManager.instance.vandegraaf2Complete)
                SceneManager.LoadScene(LevelName);
        }
        else if (LevelName == "VandeGraaffExperiment1")
        {
            if (GamificationManager.instance.vandegraaf1Complete)
                SceneManager.LoadScene(LevelName);
        }
        else if (LevelName == "Pendulum")
        {
            if (GamificationManager.instance.pendulumComplete)
                SceneManager.LoadScene(LevelName);
        }
        else if (LevelName == "FaradaysLaw")
        {
            if (GamificationManager.instance.faradayslawComplete)
                SceneManager.LoadScene(LevelName);
        }
        else if (LevelName == "FallingCoil")
        {
            if (GamificationManager.instance.fallingcoilComplete)
             SceneManager.LoadScene(LevelName);
        }
        else if (LevelName =="Whiteboard" || LevelName == "Laptop")
            SceneManager.LoadScene(LevelName);

    }

    public void OnGUI()
	{
		//if (this.insideTriggerSphere) {
			//GUI.Label(new Rect(Screen.width / 2 - 200f, Screen.height / 2 - 100f, 400f, 200f), "Press [E] " + DisplayedText, this.textStyle);
		//}
	}

 
}
