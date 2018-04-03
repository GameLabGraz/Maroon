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

    public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Player")) {
            setUI = true;
            uiBox.SetActive(true);
            Debug.Log("Player entered");
			this.insideTriggerSphere = true;
            if (!played_once && !noDialogue)
              dMan.ShowBox(dialogueKey);
            if (Just_Playing_Once)
             played_once = true;
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
                dMan.ShowBox(dialogueKeyClick);

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
