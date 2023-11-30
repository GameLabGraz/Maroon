using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GameLabGraz.UI;
using Maroon.Physics;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Util;
using Maroon.Tools.Calculator;
using Maroon.UI;
using PrivateAccess;
using TMPro;
using UnityEngine;
using LaserPointer = Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent.LaserPointer;
using LightType = Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent.LightType;
using Math = System.Math;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        private LightComponent _selectedLc;
        private OpticalComponent _selectedOc;
        private GameObject _activeLcPanel;
        private GameObject _activeOcPanel;
        private bool _singleWavelength = true;

        [Header("Prefabs: Control Panels")]
        [SerializeField] private GameObject aperturePanel;
        [SerializeField] private GameObject eyePanel;
        [SerializeField] private GameObject lensPanel;
        [SerializeField] private GameObject mirrorPanel;
        [SerializeField] private GameObject laserPointerPanel;
        [SerializeField] private GameObject parallelSourcePanel;
        [SerializeField] private GameObject pointSourcePanel;
        [SerializeField] private GameObject wavelengths;
        
        
        [Header("Light Parameters")]
        public QuantityFloat selectedWavelength;
        public QuantityFloat selectedIntensity;
        public QuantityInt numberOfRays;
        public QuantityFloat distanceBetweenRays;

        [Header("Aperture Parameters")]
        [SerializeField] private QuantityFloat apertureRin;
        [SerializeField] private QuantityFloat apertureRout;
        
        [Header("Eye Parameters")]
        [SerializeField] private QuantityFloat eyeF;

        [Header("Lens Parameters")]
        [SerializeField] private QuantityFloat lensR1;
        [SerializeField] private QuantityFloat lensR2;
        [SerializeField] private QuantityFloat lensRc;
        [SerializeField] private QuantityFloat lensD1;
        [SerializeField] private QuantityFloat lensD2;
        [SerializeField] private QuantityFloat lensA;
        [SerializeField] private QuantityFloat lensB;
        
        [Header("Mirror Parameters")]
        [SerializeField] private QuantityFloat mirrorR;
        [SerializeField] private QuantityFloat mirrorRc;

        [Header("Selected LC Position and Rotation")]
        [SerializeField] private QuantityVector3 lcPos;
        [SerializeField] private QuantityVector3 lcRot;
        
        [Header("Selected OC Position and Rotation")]
        [SerializeField] private QuantityVector3 ocPos;
        [SerializeField] private QuantityVector3 ocRot;

        public LightComponent SelectedLc
        {
            get => _selectedLc;
            set => _selectedLc = value;
        }
        public OpticalComponent SelectedOc
        {
            get => _selectedOc;
            set => _selectedOc = value;
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogError("SHOULD NOT OCCUR - Destroyed UIManager");
                Destroy(gameObject);
            }
        }
        
        // ----------------------------------- Light Components -----------------------------------
        
        public void ActivateLightControlPanel(LightComponent lc)
        {
            _selectedLc = lc;
            selectedIntensity.Value = lc.Intensity;
            selectedWavelength.Value = lc.Wavelengths.First();       // todo
            
            DeactivateAllLightControlPanels();
            
            switch (_selectedLc.LightType)
            {
                case LightType.LaserPointer:
                    SetLaserPointerControlPanelValues((LaserPointer)_selectedLc);
                    break;
                case LightType.ParallelSource:
                    SetParallelSourceControlPanelValues((ParallelSource)_selectedLc);
                    break;
                case LightType.PointSource:
                    SetPointSourceControlPanelValues((PointSource)_selectedLc);
                    break;
            }

            StoreCurrentPosRot(_selectedLc);
            lc.RecalculateLightRoute(); // todo check if call necessary 
        }
        
        public void DeactivateAllLightControlPanels()
        {
            laserPointerPanel.SetActive(false);
            parallelSourcePanel.SetActive(false);
            pointSourcePanel.SetActive(false);
            _activeLcPanel = null;
        }
        
        private void SetLaserPointerControlPanelValues(LaserPointer lp)
        {
            laserPointerPanel.SetActive(true);
            _activeLcPanel = laserPointerPanel;
        }
        
        private void SetParallelSourceControlPanelValues(ParallelSource ps)
        {
            distanceBetweenRays.Value = ps.distanceBetweenRays * Constants.InMM;
            numberOfRays.Value = ps.numberOfRays;
            parallelSourcePanel.SetActive(true);
            _activeLcPanel = parallelSourcePanel;
        }
        
        private void SetPointSourceControlPanelValues(PointSource ps)
        {
            numberOfRays.Value = ps.numberOfRays;
            pointSourcePanel.SetActive(true);
            _activeLcPanel = pointSourcePanel;
        }

        public void UpdateLightComponentValues()
        {
            if (_singleWavelength)
            {
                _selectedLc.ChangeWavelengthAndIntensity(new List<float>{selectedWavelength.Value}, selectedIntensity.Value);
                wavelengths.GetComponent<InputField>().text = selectedWavelength.Value.ToString("f2");
            }
            
            switch (_selectedLc.LightType)
            {
                case LightType.LaserPointer:
                    // Placeholder for potential future LaserPointer properties
                    break;
                
                case LightType.ParallelSource:
                    var ps = (ParallelSource)_selectedLc;
                    ps.ChangeNumberOfRays(numberOfRays.Value);
                    ps.distanceBetweenRays = distanceBetweenRays.Value / Constants.InMM;
                    break;
                
                case LightType.PointSource:
                    var pointS = (PointSource)_selectedLc;
                    pointS.ChangeNumberOfRays(numberOfRays.Value);
                    break;
            }
        }

        public void ParseWavelengthsArray(string wls)
        {
            wls = wls.Replace(" ", "");
            wls = wls.Trim(',');

            string[] splitWl = wls.Split(',');
            List<float> wavelengths = new List<float>();
            
            foreach (string wl in splitWl)
                if (float.TryParse(wl.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out float f))
                    wavelengths.Add(f);

            if (_selectedLc != null)
                _selectedLc.ChangeWavelengthAndIntensity(wavelengths, selectedIntensity.Value);

            if (wavelengths.Count > 1)
            {
                _singleWavelength = false;
                SetWavelengthSlider(false);
            }
            else
            {
                _singleWavelength = true;
                SetWavelengthSlider(true);
            }
        }

        private void SetWavelengthSlider(bool show)
        {
            if (_activeLcPanel != null)
            {
                var tmp = _activeLcPanel.transform.Find("Content");
                tmp.Find("TextWavelength").gameObject.SetActive(show);
                tmp.Find("QuantityViewWavelength").gameObject.SetActive(show);
            }
        }
        
        public void SetLcPositionFromUI(Vector3 posCm)
        {
            Vector3 posM = posCm / Constants.InCM;
            Util.Math.CropToTableBounds(ref posM);
            posCm = posM * Constants.InCM;
            _selectedLc.transform.localPosition = posM;
            
            if (lcPos.Value != posCm)
            {
                // lcPos.SystemSetsQuantity(posCm);
                lcPos.Value = posCm;
            }
        }
        
        // TODO Probably remove later
        public void OnNewValSystem(Vector3 test)
        {
            // if needed: the text in the quantity view UI could be changed manually
            // Debug.Log("new val system: " + test.ToString("f3"));
        }
        
        // public void SetLcRotationFromUI(Vector3 rot)
        // {
        //     Vector3 rotNormal = rot.normalized;
        //
        //     Vector3 rotNormalRounded = new Vector3(
        //         (float)Math.Round(rotNormal.x, 3),
        //         (float)Math.Round(rotNormal.y, 3),
        //         (float)Math.Round(rotNormal.z, 3)
        //     );
        //     
        //     if (lcRot.Value != rotNormalRounded)
        //         lcRot.Value = rotNormalRounded;
        // }

        public void UpdateLcRotation()
        {
            _selectedLc.transform.right = lcRot.Value.normalized;
        }
        
        // ----------------------------------- Optical Components -----------------------------------

        public void ActivateOpticalControlPanel(OpticalComponent oc)
        {
            _selectedOc = oc;
            DeactivateAllOpticalControlPanels();
            StoreCurrentPosRot(_selectedOc);
            
            switch (_selectedOc.OpticalType)
            {
                case OpticalType.Aperture:
                    SetApertureControlPanelValues((Aperture)_selectedOc);
                    break;
                case OpticalType.Eye:
                    SetEyeControlPanelValues((Eye)_selectedOc);
                    break;
                case OpticalType.Lens:
                    SetLensControlPanelValues((Lens)_selectedOc);
                    break;
                case OpticalType.Mirror:
                    SetMirrorControlPanelValues((TableObject.OpticalComponent.Mirror)_selectedOc);
                    break;
            }
             
        }
        
        public void DeactivateAllOpticalControlPanels()
        {
            aperturePanel.SetActive(false);
            eyePanel.SetActive(false);
            lensPanel.SetActive(false);
            mirrorPanel.SetActive(false);
            _activeOcPanel = null;
        }
        
        private void SetApertureControlPanelValues(Aperture aperture)
        {
            apertureRin.Value = aperture.Rin * Constants.InCM;
            apertureRout.Value = aperture.Rout * Constants.InCM;
            aperturePanel.SetActive(true);
            _activeOcPanel = aperturePanel;
        }
        
        private void SetEyeControlPanelValues(Eye eye)
        {
            eyeF.Value = eye.f * Constants.InCM;
            eyePanel.SetActive(true);
            _activeOcPanel = eyePanel;
        }
        
        private void SetLensControlPanelValues(Lens lens)
        {
            lensR1.Value = lens.R1 * Constants.InCM;
            lensR2.Value = lens.R2 * Constants.InCM;
            lensRc.Value = lens.Rc * Constants.InCM;
            lensD1.Value = lens.d1_TODO * Constants.InCM;
            lensD2.Value = lens.d2 * Constants.InCM;
            lensA.Value = lens.A   * Constants.InCM;
            lensB.Value = lens.B   * Constants.InCM;
            lensPanel.SetActive(true);
            _activeOcPanel = lensPanel;
        }
        
        private void SetMirrorControlPanelValues(TableObject.OpticalComponent.Mirror mirror)
        {
            mirrorR.Value = mirror.R * Constants.InCM;
            mirrorRc.Value = mirror.Rc * Constants.InCM;
            mirrorPanel.SetActive(true);
            _activeOcPanel = mirrorPanel;
        }

        public void UpdateOpticalComponentValues()
        {
            switch (_selectedOc.OpticalType)
            {
                case OpticalType.Aperture:
                    var ap = (Aperture)_selectedOc;
                    ap.Rin = apertureRin.Value / Constants.InCM;
                    ap.Rout = apertureRout.Value / Constants.InCM;
                    break;
                
                case OpticalType.Eye:
                    var ey = (Eye)_selectedOc;
                    ey.f = eyeF.Value / Constants.InCM;
                    break;
                
                case OpticalType.Lens:
                    var le = (Lens)_selectedOc;
                    le.R1 = lensR1.Value / Constants.InCM;
                    le.R2 = lensR2.Value / Constants.InCM;
                    le.Rc = lensRc.Value / Constants.InCM;
                    le.d1_TODO = lensD1.Value / Constants.InCM;
                    le.d2 = lensD2.Value / Constants.InCM;
                    le.A = lensA.Value / Constants.InCM;
                    le.B = lensB.Value / Constants.InCM;
                    break;
                
                case OpticalType.Mirror:
                    var mi = (TableObject.OpticalComponent.Mirror)_selectedOc;
                    mi.R = mirrorR.Value / Constants.InCM;
                    mi.Rc = mirrorRc.Value / Constants.InCM;
                    break;
            }
        }
        
        public void SetOcPositionFromUI(Vector3 posCm)
        {
            Vector3 posM = posCm / Constants.InCM;
            Util.Math.CropToTableBounds(ref posM);
            posCm = posM * Constants.InCM;
            _selectedOc.transform.localPosition = posM;
            
            if (ocPos.Value != posCm)
                ocPos.Value = posCm;
        }
        
        // public void SetOcRotationFromUI(Vector3 rot)
        // {
        //     Vector3 rotNormal = rot.normalized;
        //
        //     Vector3 rotNormalRounded = new Vector3(
        //         (float)Math.Round(rotNormal.x, 3),
        //         (float)Math.Round(rotNormal.y, 3),
        //         (float)Math.Round(rotNormal.z, 3)
        //     );
        //     
        //     if (ocRot.Value != rotNormalRounded)
        //         ocRot.Value = rotNormalRounded;
        // }
        
        public void UpdateOcRotation()
        {
            _selectedOc.transform.right = ocRot.Value.normalized;
        }
        
        // ----------------------------------- General -----------------------------------

        

        public void StoreCurrentPosRot(TableObject.TableObject to)
        {
            var pos = to.transform.localPosition;
            var rot = to.transform.right.normalized;
            
            Vector3 posUI = new Vector3(
                (float)Math.Round(pos.x * Constants.InCM, 2), 
                (float)Math.Round(pos.y * Constants.InCM, 2), 
                (float)Math.Round(pos.z * Constants.InCM, 2)
            );
            Vector3 rotUI = new Vector3(
                (float)Math.Round(rot.x, 3), 
                (float)Math.Round(rot.y, 3), 
                (float)Math.Round(rot.z, 3)
            );
            
            if (to.ComponentType == ComponentType.LightSource)
            {
                lcPos.Value = posUI;
                lcRot.Value = rotUI;
            }
            else if (to.ComponentType == ComponentType.OpticalComponent)
            {
                ocPos.Value = posUI;
                ocRot.Value = rotUI;
            }
            
        }
    }
}
