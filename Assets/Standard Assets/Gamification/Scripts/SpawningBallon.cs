using UnityEngine;
using System.Collections;

public class SpawningBallon : MonoBehaviour
{
    private DialogueManager dMan;
    public GameObject uiBox;
    private bool setUI = false;
    private bool hasPlayer = false;
    public GameObject ballonPrefab;

    private void Start()
    {
        dMan = FindObjectOfType<DialogueManager>();

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Should display");
            hasPlayer = true;
      
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Spawning Ballon
        if (other.CompareTag("Player") && !GamificationManager.instance.HoldingItem && hasPlayer && !GamificationManager.instance.PlayerCanPickItem && Input.GetMouseButtonDown(0)
            && !GamificationManager.instance.OneBalloonSpawned)
        {
            Debug.Log("ballon");
            Instantiate(ballonPrefab, new Vector3(GamificationManager.instance.Player_position.x-1, GamificationManager.instance.Player_position.y+1, GamificationManager.instance.Player_position.z),
             Quaternion.identity);
            GamificationManager.instance.OneBalloonSpawned = true;
        }
        if(other.CompareTag("Player"))
        {
            if (GamificationManager.instance.OneBalloonSpawned)
            {
                setUI = false;
                uiBox.SetActive(false);
            }
            else
            {
                setUI = true;
                uiBox.SetActive(true);
            }
        }

       


    }

    private void Update()
    {
       
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasPlayer = false;
            GamificationManager.instance.OneBalloonSpawned = false;
            if (setUI)
            {
                setUI = false;
                uiBox.SetActive(false);
            }
        }
    }
}

