using System;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Util;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager
{
    public class PresetManager : MonoBehaviour
    {
        
        [Header("Prefabs: Light Sources")]
        [SerializeField] private LaserPointer laserPointer;
        [SerializeField] private ParallelSource parallelSource;
        [SerializeField] private PointSource pointSource;
        
        [Header("Prefabs: Optical Components")]
        [SerializeField] private Aperture aperture;
        [SerializeField] private Eye eye;
        [SerializeField] private Lens lens;
        [SerializeField] private TableObject.OpticalComponent.Mirror mirror;
        
        [Header("Camera")]
        [SerializeField] private UnityEngine.Camera cam;

        private OpticalComponentManager _ocm;
        private LightComponentManager _lcm;
        private UIManager _uim;
        private ExperimentManager _em;

        public void Start()
        {
            _ocm = OpticalComponentManager.Instance;
            _lcm = LightComponentManager.Instance;
            _uim = UIManager.Instance;
            _em = ExperimentManager.Instance;
        }

        public void TablePresets(int nr)
        {
            switch (nr)
            {
                case 0: return;
                case 1: LensAndMirror(); break;
                case 2: FocalLength(); break;
                case 3: NormalEye(); break;
                case 4: NearsightedEye(); break;
                case 5: FarsightedEye(); break;
                case 6: UnderwaterVision(); break;
                case 7: MagnifyingGlass(); break;
                case 8: KeplerianTelescope(); break;
                case 9: GalileanTelescope(); break;
                case 10: NewtonianTelescope(); break;
                case 11: Microscope(); break;
                case 12: LightEmittingDiode(); break;
                case 13: OpticalFiber(); break;
            }
        }
        
        private void LensAndMirror()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = Constants.BaseRayThicknessInMM;
            
            _lcm.AddLightComponent(laserPointer, new Vector3(1.20f, 0, 0.62f), wavelengths: new List<float> {390f, 440f, 490f, 540f, 590f, 640f, 720f});
            _ocm.AddOpticalComponent(lens, new Vector3(1.6f, 0, 0.57f));
            _ocm.AddOpticalComponent(mirror, new Vector3(2.2f, 0, 0.40f), Vector3.back);

            SetCamera(fov: 36);
        }

        private void FocalLength()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = Constants.BaseRayThicknessInMM;
            
            _lcm.AddLightComponent(parallelSource, new Vector3(1.2f, 0, 0.62f));
            var lensObject = _ocm.AddOpticalComponent(lens, new Vector3(1.70f, 0, 0.62f));
            ((Lens)lensObject).SetParameters(R1: 0.30f, R2: -0.30f, d1: 0.015f, d2: 0.015f);
            
            SetCamera(fov: 36);
        }
        
        private void NormalEye()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.15f;
            
            var ps = _lcm.AddLightComponent(parallelSource, new Vector3(1.76f, 0, 0.577f));
            ((ParallelSource)ps).SetParameters(distanceBetweenRays: 0.50f / Constants.InMM, numberOfRays: 31);
            _ocm.AddOpticalComponent(eye, new Vector3(1.90f, 0, 0.577f));
            
            SetCamera(x: -0.12f, fov: 3);
        }
        
        private void NearsightedEye()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.3f;
            
            
        }
        
        private void FarsightedEye()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.3f;
            
            
        }
        
        private void UnderwaterVision()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.3f;


        }
        
        private void MagnifyingGlass()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.3f;


        }
        
        private void KeplerianTelescope()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.3f;


        }
        
        private void GalileanTelescope()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.3f;


        }
        
        private void NewtonianTelescope()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.3f;


        }
        
        private void Microscope()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.3f;


        }
        
        private void LightEmittingDiode()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = Constants.BaseRayThicknessInMM;
            
            
        }
        
        private void OpticalFiber()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = Constants.BaseRayThicknessInMM;
            
            
        }
        
        private void SetCamera(
            float x = Constants.BaseCameraX, 
            float z = Constants.BaseCameraZ, 
            float fov = Constants.BaseCameraFOV)
        {
            cam.transform.position = new Vector3(x, Constants.BaseCameraY, z);
            cam.fieldOfView = fov;
        }
    }
}