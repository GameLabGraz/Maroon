using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.Handlers;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Util;
using UnityEngine;
using LaserPointer = Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent.LaserPointer;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager
{
    public class LightComponentManager : MonoBehaviour
    {
        public static LightComponentManager Instance;

        private List<LightComponent> _lightComponents;
        [SerializeField] private GameObject tableLowLeftCorner;
        
        [Header("Prefabs: Light Sources")] 
        [SerializeField] private LaserPointer laserPointer;
        [SerializeField] private ParallelSource parallelSource;
        [SerializeField] private PointSource pointSource;

        public List<LightComponent> LightComponents => _lightComponents;
        

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogError("SHOULD NOT OCCUR - Destroyed LightSourceManager");
                Destroy(gameObject);
            }
            _lightComponents = new List<LightComponent>();
        }

        public LightComponent AddLightComponent(LightComponent lc, Vector3 pos, Vector3? rot = null, List<float> wavelengths = null)
        {
            Vector3 rotation = rot ?? new Vector3(0, 0, 0);
            if (wavelengths == null)
                wavelengths = new List<float> {720f};
            
            var lsClone = Instantiate(lc, tableLowLeftCorner.transform);
            lsClone.transform.localPosition = pos;
            lsClone.transform.right = rotation.normalized;
            lsClone.Wavelengths = wavelengths;
            _lightComponents.Add(lsClone);

            return lsClone;
        }

        public void AddLC(int nr)
        {
            switch (nr)
            {
                case 0:
                    return;
                case 1:
                    AddLightComponent(laserPointer, Constants.BaseLcPosition, wavelengths: laserPointer.Wavelengths);
                    break;
                case 2:
                    AddLightComponent(parallelSource, Constants.BaseLcPosition, wavelengths: parallelSource.Wavelengths);
                    break;
                case 3:
                    AddLightComponent(pointSource, Constants.BaseLcPosition, wavelengths: pointSource.Wavelengths);
                    break;
            }
        }
        
        public void WavelengthMenu(int nr)
        {
            List<float> wls = new List<float>();
            switch (nr)
            {
                case 0:
                    return;
                case 1:
                    wls.Add(650);
                    break;
                case 2:
                    wls.Add(510);
                    break;
                case 3:
                    wls.Add(450);
                    break;
                case 4:
                    wls.Add(390);
                    break;
                case 5:
                    wls.AddRange(new List<float>{450, 510, 650});
                    break;
                case 6:
                    wls.AddRange(new List<float>{390, 440, 490, 540, 590, 640, 720});
                    break;
                case 7:
                    wls.Add(633);
                    break;
                case 8:
                    wls.Add(694.3f);
                    break;
                case 9:
                    wls.AddRange(new List<float>{405, 436, 546, 579});
                    break;
                case 10:
                    wls.Add(589);
                    break;
            }
            UIManager.Instance.WavelengthSliderLogic(wls);
        }

        public void RemoveSelectedLC()
        {
            LightComponent selectedLc = UIManager.Instance.SelectedLc;

            if (selectedLc != null)
            {
                _lightComponents.Remove(selectedLc);
                selectedLc.RemoveFromTable();
                
                UIManager.Instance.SelectedLc = null;
                UIManager.Instance.DeactivateAllLightControlPanels();
            }
        }

        public void RecalculateAllLightRoutes()
        {
            foreach (var lightComponent in _lightComponents)
                lightComponent.RecalculateLightRoute();
        }
        
        public void UnselectAll()
        {
            foreach (var ls in _lightComponents)
                ls.GetComponent<SelectionHandler>().Unselect();
        }

    }
}
