using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPendulum : MonoBehaviour
{
    private int unfinishedItems = 1; //Number of elements that need to be placed
    public GameObject weight;
    private BoxCollider weightCollider;
    private bool weightEnabled;
    private DialogueManager dMan;

    // Use this for initialization
    void Start()
    {
        weightEnabled = GamificationManager.instance.PendulumweightEnabled;
        if (weightEnabled)
            unfinishedItems--;
        weight.SetActive(weightEnabled);
        weightCollider = weight.transform.parent.gameObject.GetComponent<BoxCollider>();
        dMan = FindObjectOfType<DialogueManager>();
    }

    public bool IsOverlapping(Collider current, string id)
    {
        if (id == "weight")
            return weightIsOverlapping(current);
        else
            return false;

    }



    //Check if weight is placed at right place and if yes, place it
    public bool weightIsOverlapping(Collider current)
    {


        if (weightCollider.bounds.Intersects(current.bounds) && !GamificationManager.instance.PendulumweightEnabled)
        {
            weight.SetActive(true);
            GamificationManager.instance.PendulumweightEnabled = true;
            unfinishedItems--;
            return true;
        }
        else
            return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (unfinishedItems == 0 && !GamificationManager.instance.pendulumComplete)
        {
            GamificationManager.instance.DeleteAchievement("Build Pendulum");
            GamificationManager.instance.pendulumComplete = true;
            dMan.ShowBox("Built");
        }
    }
}
