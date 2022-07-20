using UnityEngine;
using TMPro;
using Maroon.CSE.StateMachine;
using System.Collections;
public class Logger : MonoBehaviour
{
    private int logId = 1;


    public void LogStateMachineMessage(string text, Color32 color, bool shouldBeLogged) {
        if (shouldBeLogged) {
            GameObject stateMachineOverviewObject = GameObject.Find("StateMachineOverview");
            GameObject stateMachineOutputLog = Instantiate(GameObject.Find("StateMachineLog"));
            TextMeshProUGUI textmeshObject = stateMachineOutputLog.GetComponent<TextMeshProUGUI>();
            textmeshObject.text = $" {logId}: {text}";
            textmeshObject.color = color;
            stateMachineOutputLog.transform.SetParent(stateMachineOverviewObject.transform);
            stateMachineOutputLog.transform.localScale = Vector3.one;
            logId++;
        }
    }
    public void ResetLogCounter() {
        logId = 1;
    }

}
