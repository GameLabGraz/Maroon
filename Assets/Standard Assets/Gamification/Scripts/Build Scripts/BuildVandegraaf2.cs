﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildVandegraaf2 : MonoBehaviour
{
    private int unfinishedItems = 4; //Number of elements that need to be placed
    public GameObject balloon;
    public GameObject generator;
    public GameObject grounder;
    public GameObject electrode;
    private BoxCollider balloonCollider;
    private BoxCollider generatorCollider;
    private BoxCollider grounderCollider;
    private BoxCollider electrodeCollider;
    private bool generatorEnabled;
    private bool balloonEnabled;
    private bool grounderEnabled;
    private bool electrodeEnabled;
    private DialogueManager dMan;


    // Use this for initialization
    void Start ()
    {
        generatorEnabled =  GamificationManager.instance.Vandegraaf2generatorEnabled;
        balloonEnabled = GamificationManager.instance.Vandegraaf2ballonEnabled;
        grounderEnabled = GamificationManager.instance.Vandegraaf2grounderEnabled;
        electrodeEnabled = GamificationManager.instance.Vandegraaf2electrodeEnabled;
        if (balloonEnabled)
            unfinishedItems--;
        if (generatorEnabled)
            unfinishedItems--;
        if (grounderEnabled)
            unfinishedItems--;
        if (electrodeEnabled)
            unfinishedItems--;
        balloon.SetActive(balloonEnabled);
        generator.SetActive(generatorEnabled);
        grounder.SetActive(grounderEnabled);
        electrode.SetActive(electrodeEnabled);
        balloonCollider = balloon.transform.parent.gameObject.GetComponent<BoxCollider>(); //Get collider from parent
        generatorCollider = generator.transform.parent.gameObject.GetComponent<BoxCollider>();
        grounderCollider = grounder.transform.parent.gameObject.GetComponent<BoxCollider>();
        electrodeCollider = electrode.transform.parent.gameObject.GetComponent<BoxCollider>();
        dMan = FindObjectOfType<DialogueManager>();

    }

    public bool IsOverlapping(Collider current, string id)
    {
        if (id == "generator" || id == "generator 2")
            return GeneratorIsOverlapping(current);
        else if (id == "balloon")
            return BalloonIsOverlapping(current);
        else if (id == "Electrode S")
            return GrounderIsOverlapping(current);
        else if (id == "Electrode L")
            return ElectrodeIsOverlapping(current);
        else
            return false;

    }


    public bool BalloonIsOverlapping(Collider current)
    {
        if (balloonCollider.bounds.Intersects(current.bounds) && !GamificationManager.instance.Vandegraaf2ballonEnabled)
        {
            balloon.SetActive(true);
            GamificationManager.instance.Vandegraaf2ballonEnabled= true;
            unfinishedItems--;
            return true;
        }
        else
            return false;
    }

    public bool GrounderIsOverlapping(Collider current)
    {
        if (grounderCollider.bounds.Intersects(current.bounds) && !GamificationManager.instance.Vandegraaf2grounderEnabled)
        {
            grounder.SetActive(true);
            GamificationManager.instance.Vandegraaf2grounderEnabled = true;
            unfinishedItems--;
            return true;
        }
        else
            return false;
    }

    public bool ElectrodeIsOverlapping(Collider current)
    {
        if (electrodeCollider.bounds.Intersects(current.bounds) && !GamificationManager.instance.Vandegraaf2electrodeEnabled)
        {
            electrode.SetActive(true);
            GamificationManager.instance.Vandegraaf2electrodeEnabled = true;
            unfinishedItems--;
            return true;
        }
        else
            return false;
    }

    //Check if generator is placed at right place and if yes, place it
    public bool GeneratorIsOverlapping(Collider current)
    {
      
        
        if (generatorCollider.bounds.Intersects(current.bounds) && !GamificationManager.instance.Vandegraaf2generatorEnabled)
        {
            generator.SetActive(true);
            GamificationManager.instance.Vandegraaf2generatorEnabled = true;
            unfinishedItems--;
            return true;
        }
        else
            return false;
    }

    // Update is called once per frame
    void Update ()
    {
		if (unfinishedItems == 0 && !GamificationManager.instance.vandegraaf2Complete)
        {
            GamificationManager.instance.DeleteAchievement("Build Vandegraaf2");
            GamificationManager.instance.vandegraaf2Complete = true;
            dMan.ShowBox("Built");
        }
    }
}
