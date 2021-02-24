using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

public class StudyTracker : MonoBehaviour
{
    public string UserId;

    public float TimeInDetailMode;

    public List<string> ChallengeResults;

    public TMP_InputField MyInputField;

    public GameObject UserIdUi;

    public void UserNameEntered()
    {
        UserId = MyInputField.text;
        UserIdUi.SetActive(false);
    }

    public void DumpFile()
    {
        Debug.Log("Dump It");
        
        string destination = "./StudyResults_" + UserId + ".txt";

        StreamWriter writer = new StreamWriter(destination, true);
        writer.WriteLine("New Log: " + System.DateTime.Now);
        writer.WriteLine("User ID: " + UserId);
        writer.WriteLine("Time in detail mode: " + TimeInDetailMode);
        writer.WriteLine("Challenge result dump:");
        foreach (var line in ChallengeResults)
        {
            writer.WriteLine("  " + line);
        }
        writer.Close();
    }

    private void OnDisable()
    {
        DumpFile();
    }
}
