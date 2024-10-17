using System.Collections.Generic;
using Maroon.Physics.Optics.TableObject.Handlers;
using Maroon.Physics.Optics.TableObject.OpticalComponent;
using Maroon.Physics.Optics.Util;
using UnityEngine;

namespace Maroon.Physics.Optics.Manager
{
    public class OpticalComponentManager : MonoBehaviour
    {
        public static OpticalComponentManager Instance;

        private List<OpticalComponent> _opticalComponents;
        [SerializeField] private GameObject tableLowLeftCorner;

        public GameObject TableLowLeftCorner => tableLowLeftCorner;

        [Header("Prefabs: Optical Components")] 
        [SerializeField] private Aperture aperture;
        [SerializeField] private Eye eye;
        [SerializeField] private Lens lens;
        [SerializeField] private Mirror mirror;
        [SerializeField] private Wall wall;

        public List<OpticalComponent> OpticalComponents => _opticalComponents;
        

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogError("SHOULD NOT OCCUR - Destroyed OpticalComponentManager");
                Destroy(gameObject);
            }
            _opticalComponents = new List<OpticalComponent>();
        }

        private void Start()
        {
            AddWalls();
        }

        private void AddWalls()
        {
            // Bottom wall
            var wBot = Instantiate(wall, tableLowLeftCorner.transform);
            wBot.SetParameters(new WallParameters
            {
                r0 = new Vector3(2.0f, -Constants.TableObjectHeight, 1.0f),
                n = Vector3.up,
            });
            _opticalComponents.Add(wBot);
            
            // Top wall
            var wTop = Instantiate(wall, tableLowLeftCorner.transform);
            wTop.SetParameters(new WallParameters
            {
                r0 = new Vector3(2.0f, 2.0f, 1.0f),
                n = Vector3.down,
            });
            _opticalComponents.Add(wTop);
            
            // Left wall
            var wL = Instantiate(wall, tableLowLeftCorner.transform);
            wL.SetParameters(new WallParameters
            {
                r0 = new Vector3(-0.5f, 1.0f, 1.0f),
                n = Vector3.right,
            });
            _opticalComponents.Add(wL);
            
            // Right wall
            var wR = Instantiate(wall, tableLowLeftCorner.transform);
            wR.SetParameters(new WallParameters
            {
                r0 = new Vector3(4.5f, 1.0f, 1.0f),
                n = Vector3.left,
            });
            _opticalComponents.Add(wR);
            
            // Back wall
            var wBack = Instantiate(wall, tableLowLeftCorner.transform);
            wBack.SetParameters(new WallParameters
            {
                r0 = new Vector3(2.0f, 1.0f, 2.5f),
                n = Vector3.forward,
            });
            _opticalComponents.Add(wBack);
            
            // Front wall
            var wFront = Instantiate(wall, tableLowLeftCorner.transform);
            wFront.SetParameters(new WallParameters
            {
                r0 = new Vector3(2.0f, 1.0f, -0.5f),
                n = Vector3.back,
            });
            _opticalComponents.Add(wFront);
        }

        public OpticalComponent AddOpticalComponent(OpticalComponent oc, Vector3 pos, Vector3? rot = null)
        {
            Vector3 rotation = rot ?? Vector3.zero;
            
            var ocClone = Instantiate(oc, tableLowLeftCorner.transform);
            ocClone.transform.localPosition = pos;
            ocClone.transform.right = rotation.normalized;
            ocClone.UpdateProperties();
            _opticalComponents.Add(ocClone);

            return ocClone;
        }
        
        public void AddOC(int nr)
        {
            switch ((OpticalCategory) nr)
            {
                case OpticalCategory.Undefined:
                    return;
                case OpticalCategory.Lens:
                    AddOpticalComponent(lens, Constants.BaseOcPosition);
                    break;
                case OpticalCategory.Mirror:
                    AddOpticalComponent(mirror, Constants.BaseOcPosition);
                    break;
                case OpticalCategory.Eye:
                    AddOpticalComponent(eye, Constants.BaseOcPosition);
                    break;
                case OpticalCategory.Aperture:
                    AddOpticalComponent(aperture, Constants.BaseOcPosition);
                    break;
            }
        }

        public void RemoveSelectedOC()
        {
            OpticalComponent selectedOc = UIManager.Instance.SelectedOc;

            if (selectedOc != null)
            {
                _opticalComponents.Remove(selectedOc);
                selectedOc.RemoveFromTable();
                
                LightComponentManager.Instance.RecalculateAllLightRoutes();
                UIManager.Instance.SelectedOc = null;
                UIManager.Instance.DeactivateAllOpticalControlPanels();
            }
        }

        public void UnselectAll()
        {
            foreach (var oc in _opticalComponents)
            {
                if (oc.OpticalCategory != OpticalCategory.Wall)
                    oc.GetComponent<SelectionHandler>().Unselect();
            }
        }
        
        public OpticalComponent GetFirstHitComponent(Vector3 rayOrigin, Vector3 rayDirection)
        {
            float dmin = Mathf.Infinity;
            OpticalComponent firstOc = _opticalComponents[0];
            foreach (var oc in _opticalComponents)
            {
                float d = oc.GetRelevantDistance(rayOrigin, rayDirection);
                
                if (Math.IsValidDistance(d) && d < dmin)
                {
                    dmin = d;
                    firstOc = oc;
                }
            }
            return firstOc;
        }
        
    }
}
