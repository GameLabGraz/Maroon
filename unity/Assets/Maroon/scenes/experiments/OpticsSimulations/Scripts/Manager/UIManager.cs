using System.Collections;
using System.Collections.Generic;
using Maroon.Physics;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Util;
using Maroon.UI;
using PrivateAccess;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        
        private LightComponent _selectedLc;      // todo change to private
        private OpticalComponent _selectedOc;

        [Header("Prefabs: Control Panels")]
        [SerializeField] private GameObject aperturePanel;
        [SerializeField] private GameObject eyePanel;
        [SerializeField] private GameObject lensPanel;
        [SerializeField] private GameObject mirrorPanel;
        [SerializeField] private GameObject laserPanel;
        
        [Header("Light Parameters")]
        public QuantityFloat selectedWavelength;
        public QuantityFloat selectedIntensity;

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
        
        public void SelectLightComponent(LightComponent lc)
        {
            _selectedLc = lc;
            selectedIntensity.Value = lc.Intensity;
            selectedWavelength.Value = lc.Wavelength;
        }
        
        public void ActivateOpticalControlPanel(OpticalComponent oc)
        {
            _selectedOc = oc;
            DeactivateAllOpticalControlPanels();
            
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
            oc.UpdateProperties();
        }
        
        private void DeactivateAllOpticalControlPanels()
        {
            aperturePanel.SetActive(false);
            eyePanel.SetActive(false);
            lensPanel.SetActive(false);
            mirrorPanel.SetActive(false);
        }
        
        private void SetApertureControlPanelValues(Aperture aperture)
        {
            aperturePanel.SetActive(true);
            apertureRin.Value = aperture.Rin * Constants.UnitConversion;
            apertureRout.Value = aperture.Rout * Constants.UnitConversion;
        }
        
        private void SetEyeControlPanelValues(Eye eye)
        {
            eyePanel.SetActive(true);
            eyeF.Value = eye.f * Constants.UnitConversion;
        }
        
        private void SetLensControlPanelValues(Lens lens)
        {
            lensPanel.SetActive(true);
            lensR1.Value = lens.R1 * Constants.UnitConversion;
            lensR2.Value = lens.R2 * Constants.UnitConversion;
            lensRc.Value = lens.Rc * Constants.UnitConversion;
            lensD1.Value = lens.d1_TODO * Constants.UnitConversion;
            lensD2.Value = lens.d2 * Constants.UnitConversion;
            lensA.Value = lens.A   * Constants.UnitConversion;
            lensB.Value = lens.B   * Constants.UnitConversion;
        }
        
        private void SetMirrorControlPanelValues(TableObject.OpticalComponent.Mirror mirror)
        {
            mirrorPanel.SetActive(true);
            mirrorR.Value = mirror.R * Constants.UnitConversion;
            mirrorRc.Value = mirror.Rc * Constants.UnitConversion;
        }

        public void UpdateOpticalComponentValues()
        {
            switch (_selectedOc.OpticalType)
            {
                case OpticalType.Aperture:
                    var ap = (Aperture)_selectedOc;
                    ap.Rin = apertureRin.Value / Constants.UnitConversion;
                    ap.Rout = apertureRout.Value / Constants.UnitConversion;
                    break;
                case OpticalType.Eye:
                    var ey = (Eye)_selectedOc;
                    ey.f = eyeF.Value / Constants.UnitConversion;
                    break;
                case OpticalType.Lens:
                    var le = (Lens)_selectedOc;
                    le.R1 = lensR1.Value / Constants.UnitConversion;
                    le.R2 = lensR2.Value / Constants.UnitConversion;
                    le.Rc = lensRc.Value / Constants.UnitConversion;
                    le.d1_TODO = lensD1.Value / Constants.UnitConversion;
                    le.d2 = lensD2.Value / Constants.UnitConversion;
                    le.A = lensA.Value / Constants.UnitConversion;
                    le.B = lensB.Value / Constants.UnitConversion;
                    break;
                case OpticalType.Mirror:
                    var mi = (TableObject.OpticalComponent.Mirror)_selectedOc;
                    mi.R = mirrorR.Value / Constants.UnitConversion;
                    mi.Rc = mirrorRc.Value / Constants.UnitConversion;
                    break;
            }
        }
    }
}
