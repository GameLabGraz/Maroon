using System.Collections;
using System.Collections.Generic;
using Maroon.Physics;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Util;
using Maroon.UI;
using PrivateAccess;
using UnityEngine;
using LaserPointer = Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent.LaserPointer;
using LightType = Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent.LightType;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        private LightComponent _selectedLc;
        private OpticalComponent _selectedOc;

        [Header("Prefabs: Control Panels")]
        [SerializeField] private GameObject aperturePanel;
        [SerializeField] private GameObject eyePanel;
        [SerializeField] private GameObject lensPanel;
        [SerializeField] private GameObject mirrorPanel;
        [SerializeField] private GameObject laserPointerPanel;
        [SerializeField] private GameObject parallelSourcePanel;
        [SerializeField] private GameObject pointSourcePanel;
        
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
            selectedWavelength.Value = lc.Wavelength;
            
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
            lc.RecalculateLightRoute(); // todo check if call necessary 
        }
        
        private void DeactivateAllLightControlPanels()
        {
            laserPointerPanel.SetActive(false);
            parallelSourcePanel.SetActive(false);
            pointSourcePanel.SetActive(false);
        }
        
        private void SetLaserPointerControlPanelValues(LaserPointer lp)
        {
            laserPointerPanel.SetActive(true);
        }
        
        private void SetParallelSourceControlPanelValues(ParallelSource ps)
        {
            parallelSourcePanel.SetActive(true);
            distanceBetweenRays.Value = ps.distanceBetweenRays * Constants.InMM;
            numberOfRays.Value = ps.numberOfRays;
        }
        
        private void SetPointSourceControlPanelValues(PointSource ps)
        {
            pointSourcePanel.SetActive(true);
            numberOfRays.Value = ps.numberOfRays;
        }

        public void UpdateLightComponentValues()
        {
            _selectedLc.ChangeWavelengthAndIntensity(selectedWavelength.Value, selectedIntensity.Value);
            
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
        
        // ----------------------------------- Optical Components -----------------------------------

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
            apertureRin.Value = aperture.Rin * Constants.InCM;
            apertureRout.Value = aperture.Rout * Constants.InCM;
        }
        
        private void SetEyeControlPanelValues(Eye eye)
        {
            eyePanel.SetActive(true);
            eyeF.Value = eye.f * Constants.InCM;
        }
        
        private void SetLensControlPanelValues(Lens lens)
        {
            lensPanel.SetActive(true);
            lensR1.Value = lens.R1 * Constants.InCM;
            lensR2.Value = lens.R2 * Constants.InCM;
            lensRc.Value = lens.Rc * Constants.InCM;
            lensD1.Value = lens.d1_TODO * Constants.InCM;
            lensD2.Value = lens.d2 * Constants.InCM;
            lensA.Value = lens.A   * Constants.InCM;
            lensB.Value = lens.B   * Constants.InCM;
        }
        
        private void SetMirrorControlPanelValues(TableObject.OpticalComponent.Mirror mirror)
        {
            mirrorPanel.SetActive(true);
            mirrorR.Value = mirror.R * Constants.InCM;
            mirrorRc.Value = mirror.Rc * Constants.InCM;
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
    }
}
