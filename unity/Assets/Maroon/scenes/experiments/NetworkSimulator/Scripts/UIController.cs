using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class UIController : MonoBehaviour {
        [SerializeField] 
        private GameObject deviceOptionsPanel;

        public static UIController Instance { get; private set; }
        // Start is called before the first frame update
        void Start() {
            if(Instance == null) {
                Instance = this;
            }
            else {
                Debug.LogWarning("New instance of UIController detected");
            }

            HideDeviceOptions();
        }

        // Update is called once per frame
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