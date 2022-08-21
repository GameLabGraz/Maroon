using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CatalystInteractableTimerController : MonoBehaviour
{
    private float _timeInteracted = 0.0f;
    private string _userId = "";

    private bool _addToTime = false;


    public void SetUserID(string userID)
    {
        _userId = userID;
    }
    
    public void AddToInteractionTime(float time)
    {
        _timeInteracted += time;
    }

    public void StartAddToInteractionTime()
    {
        _addToTime = true;
    }

    public void StopAddToInteractionTime()
    {
        _addToTime = false;
    }
    
    public void WriteDataToFile()
    {
        using StreamWriter file = new StreamWriter("timing_data.csv", true);
        file.WriteLine($"{_userId},{_timeInteracted.ToString(CultureInfo.InvariantCulture)}");
    }

    private void FixedUpdate()
    {
        if (_addToTime)
            _timeInteracted += Time.deltaTime;
    }
}
