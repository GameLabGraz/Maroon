using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildVandegraaf1 : MonoBehaviour
{
    private int unfinishedItems = 3; //Number of elements that need to be placed
    public GameObject generator;
    public GameObject grounder;
    public GameObject electrode;
    private BoxCollider generatorCollider;
    private BoxCollider grounderCollider;
    private BoxCollider electrodeCollider;
    private bool generatorEnabled;
    private bool grounderEnabled;
    private bool electrodeEnabled;
    private DialogueManager dMan;


    // Use this for initialization
    void Start()
    {
        generatorEnabled = GamificationManager.instance.Vandegraaf1generatorEnabled;
        grounderEnabled = GamificationManager.instance.Vandegraaf1grounderEnabled;
        electrodeEnabled = GamificationManager.instance.Vandegraaf1electrodeEnabled;
        if (generatorEnabled)
            unfinishedItems--;
        if (grounderEnabled)
            unfinishedItems--;
        if (electrodeEnabled)
            unfinishedItems--;
        generator.SetActive(generatorEnabled);
        grounder.SetActive(grounderEnabled);
        electrode.SetActive(electrodeEnabled);
        generatorCollider = generator.transform.parent.gameObject.GetComponent<BoxCollider>();
        grounderCollider = grounder.transform.parent.gameObject.GetComponent<BoxCollider>();
        electrodeCollider = electrode.transform.parent.gameObject.GetComponent<BoxCollider>();
        dMan = FindObjectOfType<DialogueManager>();


    }

    public bool IsOverlapping(Collider current, string id)
    {
        if (id == "generator" || id == "generator 2")
            return GeneratorIsOverlapping(current);   
        else if (id == "Electrode S")
            return GrounderIsOverlapping(current);
        else if (id == "Electrode L")
            return ElectrodeIsOverlapping(current);
        else
            return false;

    }



    public bool GrounderIsOverlapping(Collider current)
    {
        if (grounderCollider.bounds.Intersects(current.bounds) && !GamificationManager.instance.Vandegraaf1grounderEnabled)
        {
            grounder.SetActive(true);
            GamificationManager.instance.Vandegraaf1grounderEnabled = true;
            unfinishedItems--;
            return true;
        }
        else
            return false;
    }

    public bool ElectrodeIsOverlapping(Collider current)
    {
        if (electrodeCollider.bounds.Intersects(current.bounds) && !GamificationManager.instance.Vandegraaf1electrodeEnabled)
        {
            electrode.SetActive(true);
            GamificationManager.instance.Vandegraaf1electrodeEnabled = true;
            unfinishedItems--;
            return true;
        }
        else
            return false;
    }

    //Check if generator is placed at right place and if yes, place it
    public bool GeneratorIsOverlapping(Collider current)
    {


        if (generatorCollider.bounds.Intersects(current.bounds) && !GamificationManager.instance.Vandegraaf1generatorEnabled)
        {
            generator.SetActive(true);
            GamificationManager.instance.Vandegraaf1generatorEnabled = true;
            unfinishedItems--;
            return true;
        }
        else
            return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (unfinishedItems == 0 && !GamificationManager.instance.vandegraaf1Complete)
        {
            GamificationManager.instance.DeleteAchievement("Build Vandegraaf1");
            GamificationManager.instance.vandegraaf1Complete = true;
            dMan.ShowBox("Built");
        }
    }
}
