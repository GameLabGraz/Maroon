using UnityEngine;
using System.Collections;

public class door : MonoBehaviour {
	GameObject thedoor;
    private DialogueManager dMan;
    private bool hasPlayer = false;
    private bool doorOpen = false;

    private void Start()
    {
        dMan = FindObjectOfType<DialogueManager>();
        thedoor = GameObject.FindWithTag("SF_Door");
        if (GamificationManager.instance.doorIsOpen)
            thedoor.transform.position = new Vector3(thedoor.transform.position.x, thedoor.transform.position.y + 4, thedoor.transform.position.z);
       
    }

    void OnTriggerEnter ( Collider other  )
    {

        if (other.CompareTag("Player") && !GamificationManager.instance.doorIsOpen)
        {
            hasPlayer = true;
            if (!GamificationManager.instance.doorDialogue && GamificationManager.instance.spokenWithLaunch)
            {
                dMan.ShowBox("DoorOpen");
                GamificationManager.instance.doorDialogue = true;               
            }
           
        }
      
       
}

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !GamificationManager.instance.doorIsOpen && GamificationManager.instance.spokenWithLaunch)
        {
            UIManager.instance.ShowUICollided();
        }
    }


    private void Update()
    {
        if (GamificationManager.instance.spokenWithLaunch && !GamificationManager.instance.doorIsOpen)
        {
            if (!GamificationManager.instance.holdingItem && hasPlayer && !GamificationManager.instance.playerCanPickItem && !doorOpen && Input.GetMouseButtonDown(0))
            {              
                thedoor = GameObject.FindWithTag("SF_Door");
                thedoor.GetComponent<Animation>().Play("open");
                doorOpen = true;
                Debug.Log("open");
                GamificationManager.instance.doorIsOpen = true;
                UIManager.instance.HideUICollided();
            }

            /* else if (!GamificationManager.instance.holdingItem && !GamificationManager.instance.playerCanPickItem && hasPlayer && doorOpen && Input.GetMouseButtonDown(0))
             {
                 thedoor = GameObject.FindWithTag("SF_Door");
                 thedoor.GetComponent<Animation>().Play("close");
                 doorOpen = false;
                 Debug.Log("close");
             }*/
        }
       
    }

    void OnTriggerExit ( Collider other)
    {
        if (other.CompareTag("Player") && !GamificationManager.instance.doorIsOpen)
        {
            hasPlayer = false;
            UIManager.instance.HideUICollided();
        }
}
}

