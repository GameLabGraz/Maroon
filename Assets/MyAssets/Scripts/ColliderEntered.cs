using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ColliderEntered : MonoBehaviour {

	public string LevelName;
	public string DisplayedText;	// starts with "Press [E] " //Isnt needed anymore in the Helpi Version
	private bool insideTriggerSphere = false;
	private GUIStyle textStyle;
    //dialogue starts when colliding
    public string dialogueKey;
    //dialogue starts when left-clicking
    public string dialogueKeyClick;
    private DialogueManager dMan;
    //if this is set true in Editor, Dialogue is only played once
    public bool DeleteAfterFirstTime;


    public void Start()
	{
		this.textStyle = new GUIStyle("label");
		this.textStyle.alignment = TextAnchor.MiddleCenter;
        dMan = FindObjectOfType<DialogueManager>();
    }

	public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Player")) {
			Debug.Log("Player entered");
			this.insideTriggerSphere = true;
            dMan.ShowBox(dialogueKey);
            if (DeleteAfterFirstTime)
                Destroy(this);
        }
	}

	public void OnTriggerExit(Collider other)
	{
		if (other.CompareTag ("Player")) {
			Debug.Log("Player exit");
			this.insideTriggerSphere = false;
		}
	}

	public void Update()
	{
        //Starting experiments with left-click
		if (Input.GetMouseButtonDown(0) && this.insideTriggerSphere)
        {
            if (!GamificationManager.instance.headset)
                dMan.ShowBox(dialogueKeyClick);
		
            if (LevelName == "Headset")
            {
                if (!GamificationManager.instance.headset)
                {
                    GamificationManager.instance.headset = true;
                    SoundManager.instance.SetMusicVolume(1.0f);
                   
                }
                             
                else
                {
                    GamificationManager.instance.headset = false;
                    SoundManager.instance.SetMusicVolume(0.0f);
                    dMan.ShowBox("Headset Away");
                }
                                
            }
            
              
            else if (LevelName != "helpi" && LevelName != "door" && LevelName != "Headset")
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
