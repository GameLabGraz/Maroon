using UnityEngine;
using System.Collections;

public class door : MonoBehaviour {
	GameObject thedoor;
    private DialogueManager dMan;
    public GameObject uiBox;
    private bool setUI = false;
    private bool hasPlayer = false;
    private bool doorOpen = false;

    private void Start()
    {
        dMan = FindObjectOfType<DialogueManager>();
    }

    void OnTriggerEnter ( Collider obj  )
    {
        hasPlayer = true;
        if (!GamificationManager.instance.doorDialogue)
        {
            dMan.ShowBox("DoorOpen");
            GamificationManager.instance.doorDialogue = true;
        }
        if (!setUI)
        {
            setUI = true;
            uiBox.SetActive(true);
        }
      
       
}

   

    private void Update()
    {
        if (!GamificationManager.instance.holdingItem && hasPlayer &&  !GamificationManager.instance.playerCanPickItem && !doorOpen && Input.GetMouseButtonDown(0))
        {
            thedoor = GameObject.FindWithTag("SF_Door");
            thedoor.GetComponent<Animation>().Play("open");
            doorOpen = true;
            Debug.Log("open");
        }

        else if (!GamificationManager.instance.holdingItem &&  !GamificationManager.instance.playerCanPickItem &&hasPlayer && doorOpen && Input.GetMouseButtonDown(0))
        {
            thedoor = GameObject.FindWithTag("SF_Door");
            thedoor.GetComponent<Animation>().Play("close");
            doorOpen = false;
            Debug.Log("close");
        }
    }

    void OnTriggerExit ( Collider obj  ){
        hasPlayer = false;
        if (setUI)
        {
            setUI = false;
            uiBox.SetActive(false);
        }
}
}

