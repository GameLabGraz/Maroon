using System;
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
        private GameObject _prevLcPanel;
        private GameObject _prevOcPanel;
        private bool _hideControls;

        [Header("Prefabs: Control Panels")]
        [SerializeField] private GameObject aperturePanel;
        [SerializeField] private GameObject eyePanel;
        [SerializeField] private GameObject lensPanel;
        [SerializeField] private GameObject mirrorPanel;
        [SerializeField] private GameObject laserPointerPanel;
        [SerializeField] private GameObject parallelSourcePanel;
        [SerializeField] private GameObject pointSourcePanel;

        [Header("Light Parameters")]
        // public Quantity<List<float>> selectedWavelength;
        public QuantityFloat rayThickness;
        public QuantityFloat sliderWavelength;
        public QuantityFloat selectedIntensity;
        public QuantityInt   numberOfRaysParallel;
        public QuantityFloat distanceBetweenRays;
        public QuantityInt   numberOfRaysPoint;
        public QuantityFloat rayDistributionAngle;

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
            AllowValueChangeEvent(false);
            DeactivateAllLightControlPanels();
            SetLightControlPanelValues();
            StoreCurrentPosRot(_selectedLc);
            AllowValueChangeEvent(true);
            
            // Set wavelength textfield and slider to match lightComponents wavelength
            WavelengthSliderLogic(lc.Wavelengths);
            selectedIntensity.Value = _selectedLc.Intensity;
            SetLightControlPanelValues();
        }

        private void SetLightControlPanelValues()
        {
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
            if (!_hideControls) laserPointerPanel.SetActive(true);
            _activeLcPanel = laserPointerPanel;
        }
        
        private void SetParallelSourceControlPanelValues(ParallelSource ps)
        {
            distanceBetweenRays.Value = ps.distanceBetweenRays * Constants.InMM;
            numberOfRaysParallel.Value = ps.numberOfRays;
            if (!_hideControls) parallelSourcePanel.SetActive(true);
            _activeLcPanel = parallelSourcePanel;
        }
        
        private void SetPointSourceControlPanelValues(PointSource ps)
        {
            numberOfRaysPoint.Value = ps.numberOfRays;
            rayDistributionAngle.Value = ps.rayDistributionAngle;
            if (!_hideControls) pointSourcePanel.SetActive(true);
            _activeLcPanel = pointSourcePanel;
        }

        // Is triggered on change of selectedIntensity OR numberOfRays OR distBetweenRays
        public void UpdateLightComponentValues()
        {
            _selectedLc.ChangeIntensity(selectedIntensity.Value);
            switch (_selectedLc.LightType)
            {
                case LightType.LaserPointer:
                    // Placeholder for potential future LaserPointer properties
                    break;
                
                case LightType.ParallelSource:
                    var ps = (ParallelSource)_selectedLc;
                    ps.ChangeNumberOfRays(numberOfRaysParallel.Value);
                    ps.distanceBetweenRays = distanceBetweenRays.Value / Constants.InMM;
                    break;
                
                case LightType.PointSource:
                    var pointS = (PointSource)_selectedLc;
                    pointS.ChangeNumberOfRaysAndAngle(numberOfRaysPoint.Value, rayDistributionAngle.Value);
                    break;
            }
        }

        // Triggered by sliderWavelength onChange or when text field input has only one value
        public void UpdateWavelengthTextSingle()
        {
            _selectedLc.ChangeWavelength(new List<float> {sliderWavelength.Value});
            float textNr = (float)Math.Round(sliderWavelength.Value, 2);
            GetWavelengthTextInput().text = textNr.ToString(CultureInfo.InvariantCulture);
            SetWavelengthSlider(true);
        }

        // Triggered only when textfield (wavelengths) is changed by user
        public void ParseWavelengthsArray(string wavelengthText)
        {
            wavelengthText = wavelengthText.Replace(" ", "");
            wavelengthText = wavelengthText.Trim(',');

            string[] splitWl = wavelengthText.Split(',');
            List<float> wls = new List<float>();
            
            foreach (string wl in splitWl)
                if (float.TryParse(wl.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out float f))
                    wls.Add(f);

            WavelengthSliderLogic(wls);
        }

        public void WavelengthSliderLogic(List<float> wls)
        {
            // 1. Set UI slider to first entry of the wl-list (does not matter if we got multiple)
            sliderWavelength.Value = wls.First();

            // 2. Change true light component value
            _selectedLc.ChangeWavelength(wls);
            
            // 3. Activate/Deactivate slider and update wavelength textfield
            if (wls.Count == 1)
            {
                UpdateWavelengthTextSingle();
            }
            else
            {
                SetWavelengthSlider(false);
                
                List<string> roundedNumbers = wls.Select(n => Math.Round(n, 2).ToString(CultureInfo.InvariantCulture)).ToList();
                string textWls = String.Join(",", roundedNumbers);
                GetWavelengthTextInput().text = textWls;
            }
        }

        private void SetWavelengthSlider(bool show)
        {
            if (_activeLcPanel != null)
            {
                var tmp = _activeLcPanel.transform.Find("Content");
                tmp.Find("QuantityViewWavelength").gameObject.SetActive(show);
            }
        }

        private InputField GetWavelengthTextInput()
        {
            return _activeLcPanel.transform.Find("Content").Find("HorizontalLayoutGroupWavelength").Find("TextInputWavelengths").GetComponent<InputField>();
        }
        
        public void SetLcPositionFromUI(Vector3 posCm)
        {
            Vector3 posM = posCm / Constants.InCM;
            Util.Math.CropToTableBounds(ref posM);
            posCm = posM * Constants.InCM;
            _selectedLc.transform.localPosition = posM;
            
            if (lcPos.Value != posCm)
            {
                lcPos.Value = posCm;
            }
        }
        
        public void UpdateLcRotation()
        {
            _selectedLc.transform.right = lcRot.Value.normalized;
        }
        
        // ----------------------------------- Optical Components -----------------------------------

        public void ActivateOpticalControlPanel(OpticalComponent oc)
        {
            _selectedOc = oc;
            AllowValueChangeEvent(false);
            DeactivateAllOpticalControlPanels();
            StoreCurrentPosRot(_selectedOc);

            SetOpticalControlPanelValues();
            AllowValueChangeEvent(true);
            SetOpticalControlPanelValues();
        }

        private void SetOpticalControlPanelValues()
        {
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

        private void AllowValueChangeEvent(bool value)
        {
            apertureRin.alwaysSendValueChangedEvent = value;
            apertureRout.alwaysSendValueChangedEvent = value;
            eyeF.alwaysSendValueChangedEvent = value;
            lensR1.alwaysSendValueChangedEvent = value;
            lensR2.alwaysSendValueChangedEvent = value;
            lensRc.alwaysSendValueChangedEvent = value;
            lensD1.alwaysSendValueChangedEvent = value;
            lensD2.alwaysSendValueChangedEvent = value;
            lensA.alwaysSendValueChangedEvent = value;
            lensB.alwaysSendValueChangedEvent = value;
            mirrorR.alwaysSendValueChangedEvent = value;
            mirrorRc.alwaysSendValueChangedEvent = value;
            sliderWavelength.alwaysSendValueChangedEvent = value;
            selectedIntensity.alwaysSendValueChangedEvent = value;
            numberOfRaysParallel.alwaysSendValueChangedEvent = value;
            distanceBetweenRays.alwaysSendValueChangedEvent = value;
            numberOfRaysPoint.alwaysSendValueChangedEvent = value;
            rayDistributionAngle.alwaysSendValueChangedEvent = value;
            
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
            if (!_hideControls) aperturePanel.SetActive(true);
            _activeOcPanel = aperturePanel;
        }
        
        private void SetEyeControlPanelValues(Eye eye)
        {
            eyeF.Value = eye.f * Constants.InCM;
            if (!_hideControls) eyePanel.SetActive(true);
            _activeOcPanel = eyePanel;
        }
        
        private void SetLensControlPanelValues(Lens lens)
        {
            lensR1.Value = lens.R1 * Constants.InCM;
            lensR2.Value = lens.R2 * Constants.InCM;
            lensRc.Value = lens.Rc * Constants.InCM;
            lensD1.Value = lens.d1 * Constants.InCM;
            lensD2.Value = lens.d2 * Constants.InCM;
            lensA.Value = lens.A;
            lensB.Value = lens.B;
            if (!_hideControls) lensPanel.SetActive(true);
            _activeOcPanel = lensPanel;
        }

        public void SetLensRcValue(float adjustedRc)
        {
            lensRc.alwaysSendValueChangedEvent = false;
            lensRc.Value = adjustedRc * Constants.InCM;
            lensRc.alwaysSendValueChangedEvent = true;
        }
        
        private void SetMirrorControlPanelValues(TableObject.OpticalComponent.Mirror mirror)
        {
            mirrorR.Value = mirror.R * Constants.InCM;
            mirrorRc.Value = mirror.Rc * Constants.InCM;
            if (!_hideControls) mirrorPanel.SetActive(true);
            _activeOcPanel = mirrorPanel;
        }

        public void UpdateOpticalComponentValues()
        {
            if (_selectedOc == null)
                return;
            
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
                    le.d1 = lensD1.Value / Constants.InCM;
                    le.d2 = lensD2.Value / Constants.InCM;
                    le.A = lensA.Value;
                    le.B = lensB.Value;
                    le.TranslateArrows();
                    break;
                
                case OpticalType.Mirror:
                    var mi = (TableObject.OpticalComponent.Mirror)_selectedOc;
                    mi.R = mirrorR.Value / Constants.InCM;
                    mi.Rc = mirrorRc.Value / Constants.InCM;
                    mi.TranslateArrows();
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
        
        public void UpdateOcRotation()
        {
            _selectedOc.transform.right = ocRot.Value.normalized;
        }
        
        // ----------------------------------- General -----------------------------------

        public void RecalculateSelectedLcLightRoute()
        {
            _selectedLc.RecalculateLightRoute();
        }
        
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

        public void HideControlElements()
        {
            if (!_hideControls)
            {
                if (_activeLcPanel != null)
                    _activeLcPanel.SetActive(false);
                if (_activeOcPanel != null)
                    _activeOcPanel.SetActive(false);
            }
            else
            {
                if (_activeLcPanel != null)
                    _activeLcPanel.SetActive(true);
                if (_activeOcPanel != null)
                    _activeOcPanel.SetActive(true);
            }

            _hideControls = !_hideControls;
        }
        
        // ----------------------------------- Mesh Recalculation -----------------------------------

        public void RecalculateSelectedOcMesh()
        {
            if (_selectedOc != null)
                _selectedOc.RecalculateMesh();
        }
    }
}