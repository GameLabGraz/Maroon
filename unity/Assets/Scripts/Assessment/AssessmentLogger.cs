using UnityEngine;

public static class AssessmentLogger
{
    public static bool Enable = true;

    public static void Log(string message)
    {
        if(Enable) Debug.Log(message);
    }
}
