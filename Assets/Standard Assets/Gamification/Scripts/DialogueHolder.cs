using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHolder : MonoBehaviour {

    public string dialogueKey;
    private DialogueManager dMan;
    private bool messageRead = false;

	// Use this for initialization
	void Start ()
    {
        dMan = FindObjectOfType<DialogueManager>();
	}
	
	// Update is called once per frame
	void Update ()
    {
      
    }



    private void OnMouseEnter()
    {
        if (!messageRead)
        {
            if (!GamificationManager.instance.CoroutineRunning)
                messageRead = true;
            dMan.ShowBox(dialogueKey);
          
        }
    }
}
