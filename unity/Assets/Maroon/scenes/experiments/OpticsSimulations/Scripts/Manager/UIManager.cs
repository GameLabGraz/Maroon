using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GameLabGraz.UI;
#if UNITY_WEBGL
using Maroon.GlobalEntities; // WebGLReceiver
#endif
using Maroon.Physics.Optics.TableObject;
using Maroon.Physics.Optics.TableObject.LightComponent;
using Maroon.Physics.Optics.TableObject.OpticalComponent;
using Maroon.Physics.Optics.Util;
using TMPro;
using UnityEngine;
using Math = System.Math;

namespace Maroon.Physics.Optics.Manager
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

        [Header("Prefabs: Control Panels")]
        [SerializeField] private GameObject aperturePanel;
        [SerializeField] private GameObject eyePanel;
        [SerializeField] private GameObject lensPanel;
        [SerializeField] private GameObject mirrorPanel;
        [SerializeField] private GameObject laserPointerPanel;
        [SerializeField] private GameObject parallelSourcePanel;
        [SerializeField] private GameObject pointSourcePanel;

        [Header("Preset")]
        [SerializeField] private TMP_Dropdown presetDropdown;

        [Header("Light Parameters")]
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
        [SerializeField] private GameObject lensModelDropdown;
        [SerializeField] private GameObject cauchyModelDropdown;
        [SerializeField] private GameObject focalLengthDisplay;
        
        [Header("Mirror Parameters")]
        [SerializeField] private QuantityFloat mirrorR;
        [SerializeField] private QuantityFloat mirrorRc;

        [Header("Selected LC Position and Rotation")]
        [SerializeField] private QuantityVector3 lcPos;
        [SerializeField] private QuantityVector3 lcRot;
        
        [Header("Selected OC Position and Rotation")]
        [SerializeField] private QuantityVector3 ocPos;
        [SerializeField] private QuantityVector3 ocRot;

        private TMP_Dropdown _cauchyModel;
        private TMP_Dropdown _lensModel;
        private TMP_Text _focalLengthText;
        
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

        private void Start()
        {
            _cauchyModel = cauchyModelDropdown.GetComponent<TMP_Dropdown>();
            _lensModel = lensModelDropdown.GetComponent<TMP_Dropdown>();
            _focalLengthText = focalLengthDisplay.GetComponent<TMP_Text>();
#if UNITY_WEBGL
            WebGlReceiver.Instance.OnIncomingData.AddListener((string _jsonData) => {
                // When OpticsParameters config JSON gets sent via Javascript, set the preset Dropdown to index 0, as that's representing an 'undefined' preset
                presetDropdown.SetValueWithoutNotify(0);
            });
#endif
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
            switch (_selectedLc.LightCategory)
            {
                case LightCategory.LaserPointer:
                    SetLaserPointerControlPanelValues((LaserPointer)_selectedLc);
                    break;
                case LightCategory.ParallelSource:
                    SetParallelSourceControlPanelValues((ParallelSource)_selectedLc);
                    break;
                case LightCategory.PointSource:
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
            laserPointerPanel.SetActive(true);
            _activeLcPanel = laserPointerPanel;
        }
        
        private void SetParallelSourceControlPanelValues(ParallelSource ps)
        {
            distanceBetweenRays.Value = ps.distanceBetweenRays * Constants.InMM;
            numberOfRaysParallel.Value = ps.numberOfRays;
            parallelSourcePanel.SetActive(true);
            _activeLcPanel = parallelSourcePanel;
        }
        
        private void SetPointSourceControlPanelValues(PointSource ps)
        {
            numberOfRaysPoint.Value = ps.numberOfRays;
            rayDistributionAngle.Value = ps.rayDistributionAngle;
            pointSourcePanel.SetActive(true);
            _activeLcPanel = pointSourcePanel;
        }

        // Is triggered on change of selectedIntensity OR numberOfRays OR distBetweenRays
        public void UpdateLightComponentValues()
        {
            _selectedLc.ChangeIntensity(selectedIntensity.Value);
            switch (_selectedLc.LightCategory)
            {
                case LightCategory.LaserPointer:
                    // Placeholder for potential future LaserPointer properties
                    break;
                
                case LightCategory.ParallelSource:
                    var ps = (ParallelSource)_selectedLc;
                    ps.ChangeNumberOfRays(numberOfRaysParallel.Value);
                    ps.distanceBetweenRays = distanceBetweenRays.Value / Constants.InMM;
                    break;
                
                case LightCategory.PointSource:
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
            switch (_selectedOc.OpticalCategory)
            {
                case OpticalCategory.Aperture:
                    SetApertureControlPanelValues((Aperture)_selectedOc);
                    break;
                case OpticalCategory.Eye:
                    SetEyeControlPanelValues((Eye)_selectedOc);
                    break;
                case OpticalCategory.Lens:
                    SetLensControlPanelValues((Lens)_selectedOc);
                    break;
                case OpticalCategory.Mirror:
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
            lensD1.Value = lens.d1 * Constants.InCM;
            lensD2.Value = lens.d2 * Constants.InCM;
            lensA.Value = lens.A;
            lensB.Value = lens.B;
            
            
            lensPanel.SetActive(true);
            _activeOcPanel = lensPanel;
            DisplayLensFocalLength();
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
            mirrorPanel.SetActive(true);
            _activeOcPanel = mirrorPanel;
        }

        public void UpdateOpticalComponentValues()
        {
            if (_selectedOc == null)
                return;
            
            switch (_selectedOc.OpticalCategory)
            {
                case OpticalCategory.Aperture:
                    var ap = (Aperture)_selectedOc;
                    ap.Rin = apertureRin.Value / Constants.InCM;
                    ap.Rout = apertureRout.Value / Constants.InCM;
                    break;
                
                case OpticalCategory.Eye:
                    var ey = (Eye)_selectedOc;
                    ey.f = eyeF.Value / Constants.InCM;
                    break;
                
                case OpticalCategory.Lens:
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
                
                case OpticalCategory.Mirror:
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
        
        // ----------------------------------- Lens Presets -----------------------------------
        public void LensCauchyModel(int nr)
        {
            if (_selectedOc == null || _selectedOc.OpticalCategory != OpticalCategory.Lens)
                return;
            
            switch (nr)
            {
                case 0: return;
                case 1: SetLensCauchyModel(Constants.DenseFlintGlassSF10); break;
                case 2: SetLensCauchyModel(Constants.FusedSilica); break;
                case 3: SetLensCauchyModel(Constants.BorosilicateGlassBK7); break;
                case 4: SetLensCauchyModel(Constants.HardCrownGlassK5); break;
                case 5: SetLensCauchyModel(Constants.BariumCrownGlassBaK4); break;
                case 6: SetLensCauchyModel(Constants.BariumFlintGlassBaF10); break;
            }
        }

        private void SetLensCauchyModel((float, float) ab)
        {
            var le = (Lens)_selectedOc;
            le.A = ab.Item1;
            lensA.Value = ab.Item1;
            le.B = ab.Item2;
            lensB.Value = ab.Item2;
        }

        public void SetCustomCauchyModelDropdown()
        {
            if (_selectedOc == null || _selectedOc.OpticalCategory != OpticalCategory.Lens)
                return;

            var ab = (lensA.Value, lensB.Value);
            int dropdownValue = 0;

            if (CheckNumberRange(ab, Constants.DenseFlintGlassSF10)) dropdownValue = 1;
            else if (CheckNumberRange(ab, Constants.FusedSilica)) dropdownValue = 2;
            else if (CheckNumberRange(ab, Constants.BorosilicateGlassBK7)) dropdownValue = 3;
            else if (CheckNumberRange(ab, Constants.HardCrownGlassK5)) dropdownValue = 4;
            else if (CheckNumberRange(ab, Constants.BariumCrownGlassBaK4)) dropdownValue = 5;
            else if (CheckNumberRange(ab, Constants.BariumFlintGlassBaF10)) dropdownValue = 6;
            
            _cauchyModel.SetValueWithoutNotify(dropdownValue);
        }

        // Recalculated when lens.R1, lens.R2, lens.A, lens.B changes
        public void DisplayLensFocalLength()
        {
            if (_selectedOc == null || _selectedOc.OpticalCategory != OpticalCategory.Lens)
                return;

            var le = (Lens)_selectedOc;
            float wl = sliderWavelength.Value;
            float fCM = le.FocalLength(wl) * Constants.InCM;
            
            _focalLengthText.SetText("f ~ " + fCM.ToString("F2") + " [cm] @" + wl.ToString("F0") + " [nm]");
        }
        
        public void LensType(int nr)
        {
            if (_selectedOc == null || _selectedOc.OpticalCategory != OpticalCategory.Lens)
                return;
            
            var le = (Lens)_selectedOc;
            
            switch (nr)
            {
                case 0: 
                    return;
                case 1:
                    le.SetParameters(new LensParameters
                    {
                        R1 = Constants.Biconvex.Item1,
                        R2 = Constants.Biconvex.Item2,
                        d1 = Constants.Biconvex.Item3,
                        d2 = Constants.Biconvex.Item4,
                        Rc = Constants.LensPrestRc,
                        A = Constants.DenseFlintGlassSF10.Item1,
                        B = Constants.DenseFlintGlassSF10.Item2,
                    });
                    break;
                case 2:
                    le.SetParameters(new LensParameters
                    {
                        R1 = Constants.Planoconvex.Item1,
                        R2 = Constants.Planoconvex.Item2,
                        d1 = Constants.Planoconvex.Item3,
                        d2 = Constants.Planoconvex.Item4,
                        Rc = Constants.LensPrestRc,
                        A = Constants.DenseFlintGlassSF10.Item1,
                        B = Constants.DenseFlintGlassSF10.Item2,
                    });
                    break;
                case 3:
                    le.SetParameters(new LensParameters
                    {
                        R1 = Constants.PositiveMeniscus.Item1,
                        R2 = Constants.PositiveMeniscus.Item2,
                        d1 = Constants.PositiveMeniscus.Item3,
                        d2 = Constants.PositiveMeniscus.Item4,
                        Rc = Constants.LensPrestRc,
                        A = Constants.DenseFlintGlassSF10.Item1,
                        B = Constants.DenseFlintGlassSF10.Item2,
                    });
                    break;
                case 4:
                    le.SetParameters(new LensParameters
                    {
                        R1 = Constants.NegativeMeniscus.Item1,
                        R2 = Constants.NegativeMeniscus.Item2,
                        d1 = Constants.NegativeMeniscus.Item3,
                        d2 = Constants.NegativeMeniscus.Item4,
                        Rc = Constants.LensPrestRc,
                        A = Constants.DenseFlintGlassSF10.Item1,
                        B = Constants.DenseFlintGlassSF10.Item2,
                    });
                    break;
                case 5:
                    le.SetParameters(new LensParameters
                    {
                        R1 = Constants.Planoconcave.Item1,
                        R2 = Constants.Planoconcave.Item2,
                        d1 = Constants.Planoconcave.Item3,
                        d2 = Constants.Planoconcave.Item4,
                        Rc = Constants.LensPrestRc,
                        A = Constants.DenseFlintGlassSF10.Item1,
                        B = Constants.DenseFlintGlassSF10.Item2,
                    });
                    break;
                case 6:
                    le.SetParameters(new LensParameters
                    {
                        R1 = Constants.Biconcave.Item1,
                        R2 = Constants.Biconcave.Item2,
                        d1 = Constants.Biconcave.Item3,
                        d2 = Constants.Biconcave.Item4,
                        Rc = Constants.LensPrestRc,
                        A = Constants.DenseFlintGlassSF10.Item1,
                        B = Constants.DenseFlintGlassSF10.Item2,
                    });
                    break;
                case 7:
                    le.SetParameters(new LensParameters
                    {
                        R1 = Constants.Ball.Item1,
                        R2 = Constants.Ball.Item2,
                        d1 = Constants.Ball.Item3,
                        d2 = Constants.Ball.Item4,
                        Rc = Constants.LensPrestRc,
                        A = Constants.DenseFlintGlassSF10.Item1,
                        B = Constants.DenseFlintGlassSF10.Item2,
                    });
                    break;
            }
            
            AllowValueChangeEvent(false);
            SetLensControlPanelValues(le);
            AllowValueChangeEvent(true);
            SetLensControlPanelValues(le);
        }
        
        public void SetCustomLensModelDropdown()
        {
            if (_selectedOc == null || _selectedOc.OpticalCategory != OpticalCategory.Lens)
                return;
            
            var lensParams = (
                lensR1.Value / Constants.InCM, 
                lensR2.Value / Constants.InCM, 
                lensD1.Value / Constants.InCM, 
                lensD2.Value / Constants.InCM);
            
            int dropdownValue = 0;
            
            if (CheckNumberRange(lensParams, Constants.Biconvex)) dropdownValue = 1;
            else if (CheckNumberRange(lensParams, Constants.Planoconvex)) dropdownValue = 2;
            else if (CheckNumberRange(lensParams, Constants.PositiveMeniscus)) dropdownValue = 3;
            else if (CheckNumberRange(lensParams, Constants.NegativeMeniscus)) dropdownValue = 4;
            else if (CheckNumberRange(lensParams, Constants.Planoconcave)) dropdownValue = 5;
            else if (CheckNumberRange(lensParams, Constants.Biconcave)) dropdownValue = 6;
            else if (CheckNumberRange(lensParams, Constants.Ball)) dropdownValue = 7;
                
            _lensModel.SetValueWithoutNotify(dropdownValue);
        }

        private bool CheckNumberRange((float, float) t1, (float, float) t2)
        {
            float t2Lower1 = t2.Item1 * 0.9999f;
            float t2Lower2 = t2.Item2 * 0.9999f;
            float t2Upper1 = t2.Item1 * 1.0001f;
            float t2Upper2 = t2.Item2 * 1.0001f;

            return t1.Item1 >= t2Lower1 && t1.Item2 >= t2Lower2 && t1.Item1 <= t2Upper1 && t1.Item2 <= t2Upper2;
        }
        
        private bool CheckNumberRange((float, float, float, float) t1, (float, float, float, float) t2)
        {
            float t2Lower1 = t2.Item1 * 0.9999f;
            float t2Lower2 = t2.Item2 * 0.9999f;
            float t2Lower3 = t2.Item3 * 0.9999f;
            float t2Lower4 = t2.Item4 * 0.9999f;
            
            float t2Upper1 = t2.Item1 * 1.0001f;
            float t2Upper2 = t2.Item2 * 1.0001f;
            float t2Upper3 = t2.Item3 * 1.0001f;
            float t2Upper4 = t2.Item4 * 1.0001f;

            // R1 and R2 both positive
            if (t1.Item1 > 0 && t1.Item2 > 0 && t2.Item1 > 0 && t2.Item2 > 0)
                return t1.Item1 >= t2Lower1 && t1.Item2 >= t2Lower2 && t1.Item3 >= t2Lower3 && t1.Item4 >= t2Lower4
                       && t1.Item1 <= t2Upper1 && t1.Item2 <= t2Upper2 && t1.Item3 <= t2Upper3 && t1.Item4 <= t2Upper4;
            
            // R1 and R2 both negative
            if (t1.Item1 < 0 && t1.Item2 < 0 && t2.Item1 < 0 && t2.Item2 < 0)
                return t1.Item1 <= t2Lower1 && t1.Item2 <= t2Lower2 && t1.Item3 >= t2Lower3 && t1.Item4 >= t2Lower4
                       && t1.Item1 >= t2Upper1 && t1.Item2 >= t2Upper2 && t1.Item3 <= t2Upper3 && t1.Item4 <= t2Upper4;
            
            // R1 negative and R2 positive
            if (t1.Item1 < 0 && t2.Item1 < 0)
                return t1.Item1 <= t2Lower1 && t1.Item2 >= t2Lower2 && t1.Item3 >= t2Lower3 && t1.Item4 >= t2Lower4
                       && t1.Item1 >= t2Upper1 && t1.Item2 <= t2Upper2 && t1.Item3 <= t2Upper3 && t1.Item4 <= t2Upper4;
            
            // R1 positive and R2 negative
            if (t1.Item2 < 0&& t2.Item2 < 0)
                return t1.Item1 >= t2Lower1 && t1.Item2 <= t2Lower2 && t1.Item3 >= t2Lower3 && t1.Item4 >= t2Lower4
                       && t1.Item1 <= t2Upper1 && t1.Item2 >= t2Upper2 && t1.Item3 <= t2Upper3 && t1.Item4 <= t2Upper4;

            return false;
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
        // ----------------------------------- Mesh Recalculation -----------------------------------

        public void RecalculateSelectedOcMesh()
        {
            if (_selectedOc != null)
                _selectedOc.RecalculateMesh();
        }
    }
}
