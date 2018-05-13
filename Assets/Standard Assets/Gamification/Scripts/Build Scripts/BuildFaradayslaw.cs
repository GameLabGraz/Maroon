using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildFaradayslaw : MonoBehaviour
{
    private int unfinishedItems = 2; //Number of elements that need to be placed
    public GameObject magnet;
    public GameObject ring;
    private BoxCollider magnetCollider;
    private BoxCollider ringCollider;
    private bool magnetEnabled;
    private bool ringEnabled;
    private DialogueManager dMan;


    // Use this for initialization
    void Start()
    {
        magnetEnabled = GamificationManager.instance.FaradayslawmagnetEnabled;
        ringEnabled = GamificationManager.instance.FaradayslawringEnabled;
        if (magnetEnabled)
            unfinishedItems--;
        if (ringEnabled)
            unfinishedItems--;
        magnet.SetActive(magnetEnabled);
        ring.SetActive(ringEnabled);
        magnetCollider = magnet.transform.parent.gameObject.GetComponent<BoxCollider>();
        ringCollider = ring.transform.parent.gameObject.GetComponent<BoxCollider>();
        dMan = FindObjectOfType<DialogueManager>();
    }

    public bool IsOverlapping(Collider current, string id)
    {
        if (id == "magnet")
            return magnetIsOverlapping(current);
        else if (id == "coil M")
            return ringIsOverlapping(current);
        else
            return false;

    }



    public bool ringIsOverlapping(Collider current)
    {
        if (ringCollider.bounds.Intersects(current.bounds) && !GamificationManager.instance.FaradayslawringEnabled)
        {
            ring.SetActive(true);
            GamificationManager.instance.FaradayslawringEnabled = true;
            unfinishedItems--;
            return true;
        }
        else
            return false;
    }


    //Check if magnet is placed at right place and if yes, place it
    public bool magnetIsOverlapping(Collider current)
    {


        if (magnetCollider.bounds.Intersects(current.bounds) && !GamificationManager.instance.FaradayslawmagnetEnabled)
        {
            magnet.SetActive(true);
            GamificationManager.instance.FaradayslawmagnetEnabled = true;
            unfinishedItems--;
            return true;
        }
        else
            return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (unfinishedItems == 0 && !GamificationManager.instance.faradayslawComplete)
        {
            GamificationManager.instance.DeleteAchievement("Build Faradayslaw");
            GamificationManager.instance.faradayslawComplete = true;
            dMan.ShowBox("Built");
        }
    }
}
