using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPendulum : MonoBehaviour
{
    private int unfinishedItems = 1; //Number of elements that need to be placed
    public GameObject weight;
    private BoxCollider weightCollider;
    private bool weightEnabled;

    // Use this for initialization
    void Start()
    {
        weightEnabled = GamificationManager.instance.pendulumweightEnabled;
        if (weightEnabled)
            unfinishedItems--;
        weight.SetActive(weightEnabled);
        weightCollider = weight.transform.parent.gameObject.GetComponent<BoxCollider>();
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


        if (weightCollider.bounds.Intersects(current.bounds) && !GamificationManager.instance.pendulumweightEnabled)
        {
            weight.SetActive(true);
            GamificationManager.instance.pendulumweightEnabled = true;
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
            GamificationManager.instance.finishedAchievements++;
        }
    }
}
