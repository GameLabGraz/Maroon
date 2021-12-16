﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.UI;
using TMPro;
using System.Threading;

namespace StateMachine {
    public class Logger : MonoBehaviour
    {
        private int logId = 1;
        public void LogStateMachineMessage(string text) {
            GameObject stateMachineOverviewObject = GameObject.Find("StateMachineOverview");
            GameObject stateMachineOutputLog = Instantiate(GameObject.Find("StateMachineLog"));
            TextMeshProUGUI textmeshObject = stateMachineOutputLog.GetComponent(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
            textmeshObject.text = " " + logId + ": " + text;
            stateMachineOutputLog.transform.SetParent(stateMachineOverviewObject.transform);
            stateMachineOutputLog.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            logId++;
        }
    }
}