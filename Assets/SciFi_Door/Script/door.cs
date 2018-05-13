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
        if (GamificationManager.instance.DoorIsOpen)
            thedoor.transform.position = new Vector3(thedoor.transform.position.x, thedoor.transform.position.y + 4, thedoor.transform.position.z);
       
    }

    void OnTriggerEnter ( Collider other  )
    {

        if (other.CompareTag("Player") && !GamificationManager.instance.DoorIsOpen)
        {
            hasPlayer = true;
            if (!GamificationManager.instance.DoorDialogue && GamificationManager.instance.SpokenWithLaunch)
            {
                dMan.ShowBox("DoorOpen");
                GamificationManager.instance.DoorDialogue = true;               
            }
           
        }
      
       
}

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !GamificationManager.instance.DoorIsOpen && GamificationManager.instance.SpokenWithLaunch)
        {
            UIManager.instance.ShowUICollided();
        }
    }


    private void Update()
    {
        if (GamificationManager.instance.SpokenWithLaunch && !GamificationManager.instance.DoorIsOpen)
        {
            if (!GamificationManager.instance.HoldingItem && hasPlayer && !GamificationManager.instance.PlayerCanPickItem && !doorOpen && Input.GetMouseButtonDown(0))
            {              
                thedoor = GameObject.FindWithTag("SF_Door");
                thedoor.GetComponent<Animation>().Play("open");
                doorOpen = true;
                Debug.Log("open");
                GamificationManager.instance.DoorIsOpen = true;
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
        if (other.CompareTag("Player") && !GamificationManager.instance.DoorIsOpen)
        {
            hasPlayer = false;
            UIManager.instance.HideUICollided();
        }
}
}

