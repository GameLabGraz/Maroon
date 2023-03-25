using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class UIController : MonoBehaviour {
        [SerializeField] 
        private GameObject deviceOptionsPanel;

        public static UIController Instance { get; private set; }
        
        void Start() {
            if(Instance == null) {
                Instance = this;
            }
            else {
                Debug.LogWarning("New instance of UIController detected");
            }

            HideDeviceOptions();
        }

        void Update() {

        }

        public static void ShowDeviceOptions() {
            Instance.deviceOptionsPanel.SetActive(true);
        }
        public static void HideDeviceOptions() {
            Instance.deviceOptionsPanel.SetActive(false);
        }
    }
}