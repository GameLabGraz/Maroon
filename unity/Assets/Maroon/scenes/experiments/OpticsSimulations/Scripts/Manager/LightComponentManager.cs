using System;
using System.Collections.Generic;
using Maroon.Physics.Optics.TableObject.Handlers;
using Maroon.Physics.Optics.TableObject.LightComponent;
using Maroon.Physics.Optics.Util;
using UnityEngine;
// using LaserPointer = Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent.LaserPointer;

namespace Maroon.Physics.Optics.Manager
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

        private readonly List<float>[] _wavelengthSettings =
        {
            new(),
            new(){650},
            new(){510},
            new(){450},
            new(){390},
            new(){450, 510, 650},
            new(){390, 440, 490, 540, 590, 640, 720},
            new(){633},
            new(){694.3f},
            new(){405, 436, 546, 579},
            new(){589}
        };

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
            Vector3 rotation = rot ?? Vector3.zero;
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
            switch ((LightCategory) nr)
            {
                case LightCategory.Undefined:
                    return;
                case LightCategory.LaserPointer:
                    AddLightComponent(laserPointer, Constants.BaseLcPosition, wavelengths: laserPointer.Wavelengths);
                    break;
                case LightCategory.ParallelSource:
                    AddLightComponent(parallelSource, Constants.BaseLcPosition, wavelengths: parallelSource.Wavelengths);
                    break;
                case LightCategory.PointSource:
                    AddLightComponent(pointSource, Constants.BaseLcPosition, wavelengths: pointSource.Wavelengths);
                    break;
            }
        }
        
        public void WavelengthMenu(int nr)
        {
            if (nr == 0) return;
            UIManager.Instance.WavelengthSliderLogic(_wavelengthSettings[nr]);
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
