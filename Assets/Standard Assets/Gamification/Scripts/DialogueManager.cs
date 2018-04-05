using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;




public class DialogueManager : MonoBehaviour {

    private float waitAtEnd = 1.0f;
    public GameObject dBox;
    public Text text;
    public bool dialogActive;
    public float letterPause = 0.01f;
    public AudioClip typeSound;
    private string dialougeKey1 = "Introduction 1";
    private string dialogueKey2 = "Introduction 2";
    private string dialogueKey3 = "Vandegraaf 1";
    private string dialogueKey4 = "Vandegraaf 2";
    
    

    // Use this for initialization
    void Start ()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        GamificationManager.instance.coroutineRunning = false;
        if (sceneName == "VandeGraaffExperiment1")
        {

          //  ShowBox(dialogueKey3);

            
        }
        else if (sceneName == "Laboratory" && !GamificationManager.instance.gameStarted)
        {     
            ShowBox(dialougeKey1);                   
        }
        else if (sceneName == "VandeGraaffExperiment2")
        {
           // ShowBox(dialogueKey4);
        }
        else if(sceneName == "Laboratory" && GamificationManager.instance.gameStarted)
        {
            dialogActive = false;
            dBox.SetActive(false);
            GameObject uiBox = GameObject.FindWithTag("UI");
            uiBox.SetActive(false);
        }
        text.text = "";
    }
	
	// Update is called once per frame
	void Update () 
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

       // if (sceneName == "VandeGraaffExperiment1")
        //{

            //Left Mouse Click
            if (dialogActive && Input.GetMouseButtonDown(0))
            {
                dBox.SetActive(false);
                dialogActive = false;
                waitAtEnd = 0.0f;
            }
     //   }

       

	}

    //this functions shows a new dialogue. 
    //keyword is the keyword of dialogue who should be showed
    public void ShowBox(string keyword)
    {      
        if (GamificationManager.instance.deactivateDialogue)
            return;
        //get the dialogue in the language manager in the current language. Returns nothing if keyword doesnt exist
        string dialogue = GamificationManager.instance.l_manager.GetString(keyword);
        //Creating newlines
        Debug.Log(dialogue);
        // \n doesnt work when you load dialogues from XML-Files, so write NEWLINE if you want a newline and replace it here
        dialogue = dialogue.Replace("NEWLINE ", "\n"); //To create Newlines  
        Debug.Log(dialogue);
        if (!GamificationManager.instance.coroutineRunning)
        {
            dialogActive = true;
            dBox.SetActive(true);
            text.text = "";
            waitAtEnd = 1.0f;       
            SoundManager.instance.PlaySingle(typeSound);
            StartCoroutine(TypeText(dialogue));
        }
       


    }



    //Function to display text letter-by-letter
    IEnumerator TypeText(string dialogue)
    {
        GamificationManager.instance.coroutineRunning = true;
        foreach (char letter in dialogue.ToCharArray())
        {
            if (!dialogActive)
                break;
           
            text.text += letter;
            //Play randomized Type Sound
     
             
            yield return 0;
            yield return new WaitForSeconds(letterPause);
        }
            yield return 0;
            yield return new WaitForSeconds(waitAtEnd);
        if (!GamificationManager.instance.gameStarted)
        {
            dialogActive = false;
            dBox.SetActive(false);
            GamificationManager.instance.coroutineRunning = false;
            ShowBox(dialogueKey2);
            GamificationManager.instance.gameStarted = true;

        }
        else
        {
            dialogActive = false;
            dBox.SetActive(false);
            GamificationManager.instance.coroutineRunning = false;                     
        }
        
        
  
    
    }

}
