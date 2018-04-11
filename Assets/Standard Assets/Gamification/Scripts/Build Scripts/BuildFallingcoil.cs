using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildFallingcoil : MonoBehaviour
{
    private int unfinishedItems = 2; //Number of elements that need to be placed
    public GameObject magnet;
    public GameObject ring;
    private BoxCollider magnetCollider;
    private BoxCollider ringCollider;
    private bool magnetEnabled;
    private bool ringEnabled;

    // Use this for initialization
    void Start()
    {
        magnetEnabled = GamificationManager.instance.fallingcoilmagnetEnabled;
        ringEnabled = GamificationManager.instance.fallingcoilringEnabled;
        if (magnetEnabled)
            unfinishedItems--;
        if (ringEnabled)
            unfinishedItems--;
        magnet.SetActive(magnetEnabled);
        ring.SetActive(ringEnabled);
        magnetCollider = magnet.transform.parent.gameObject.GetComponent<BoxCollider>();
        ringCollider = ring.transform.parent.gameObject.GetComponent<BoxCollider>();
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
        if (ringCollider.bounds.Intersects(current.bounds) && !GamificationManager.instance.fallingcoilringEnabled)
        {
            ring.SetActive(true);
            GamificationManager.instance.fallingcoilringEnabled = true;
            unfinishedItems--;
            return true;
        }
        else
            return false;
    }

  
    //Check if magnet is placed at right place and if yes, place it
    public bool magnetIsOverlapping(Collider current)
    {


        if (magnetCollider.bounds.Intersects(current.bounds) && !GamificationManager.instance.fallingcoilmagnetEnabled)
        {
            magnet.SetActive(true);
            GamificationManager.instance.fallingcoilmagnetEnabled = true;
            unfinishedItems--;
            return true;
        }
        else
            return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (unfinishedItems == 0 && !GamificationManager.instance.fallingcoilComplete)
        {
            GamificationManager.instance.DeleteAchievement("Build FallingCoil");
            GamificationManager.instance.fallingcoilComplete = true;
            GamificationManager.instance.finishedAchievements++;
        }
    }
}
