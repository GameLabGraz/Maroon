using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GuiPendulum : MonoBehaviour {

    public Text text_info;


    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Laboratory");
        }

        string dialogue;  
        dialogue = GamificationManager.instance.l_manager.GetString("Info Pendulum");
        dialogue = dialogue.Replace("NEWLINE ", "\n");
        text_info.text = dialogue;
    }
}
