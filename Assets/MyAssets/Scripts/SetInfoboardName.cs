using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetInfoboardName : MonoBehaviour {

    public Text enterText;
    public string dialogueKey;


	// Update is called once per frame
	void Update ()
    {
        string dialogue = GamificationManager.instance.l_manager.GetString(dialogueKey);
        enterText.text = dialogue;
    }
}
