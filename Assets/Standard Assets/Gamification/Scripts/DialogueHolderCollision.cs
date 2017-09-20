using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHolderCollision : MonoBehaviour
{

    public string dialogue;
    private DialogueManager dMan;

    // Use this for initialization
    void Start()
    {
        dMan = FindObjectOfType<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Player")
        {
            dMan.ShowBox(dialogue);
        }
    }


}
