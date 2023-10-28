using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager
{
    public class ExperimentManager : MonoBehaviour
    {
        public static ExperimentManager Instance;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogError("SHOULD NOT OCCUR - Destroyed LightSourceManager");
                Destroy(gameObject);
            }
        }
        
        private void Update()
        {
            // Main Update loop
            
            // Light Source Branch
            if (UIManager.Instance.SelectedLc != null)
            {
                var lc = UIManager.Instance.SelectedLc;
                
                lc.ChangeWavelengthAndIntensity(
                    UIManager.Instance.selectedWavelength.Value, 
                    UIManager.Instance.selectedIntensity.Value);
                
                if (lc.transform.hasChanged)
                {
                    lc.Origin = lc.transform.localPosition;
                    lc.RecalculateLightRoute();
                    lc.transform.hasChanged = false;
                }
            }

            // Optical Component Branch
            if (UIManager.Instance.SelectedOc != null)
            {
                var oc = UIManager.Instance.SelectedOc;
                UIManager.Instance.UpdateOpticalComponentValues();
                
                if (oc.transform.hasChanged)
                {
                    oc.UpdateProperties();
                    oc.transform.hasChanged = false;
                }
                LightComponentManager.Instance.CheckOpticalComponentHit(oc);
            }
        }
    }
}
