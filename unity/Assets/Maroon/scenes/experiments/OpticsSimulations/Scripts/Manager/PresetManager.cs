using System;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Camera;
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
        [SerializeField] private GameObject mainCamera;
        private UnityEngine.Camera _cam;
        private CameraControls _camControls;

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

            _cam = mainCamera.GetComponent<UnityEngine.Camera>();
            _camControls = mainCamera.GetComponent<CameraControls>();
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
            _lcm.AddLightComponent(laserPointer, new Vector3(1.30f, 0, 0.62f), wavelengths: new List<float> {390f, 440f, 490f, 540f, 590f, 640f, 720f});
            _ocm.AddOpticalComponent(lens, new Vector3(1.6f, 0, 0.57f));
            _ocm.AddOpticalComponent(mirror, new Vector3(2.2f, 0, 0.40f), Vector3.back);
            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.065f, 2.6f, 1.0f), Constants.BaseCamRot, 30),
                new CameraControls.CameraSetting(new Vector3(0, 3.0f, 2.0f), Constants.TopCamRot, 33)
            );
        }

        private void FocalLength()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = Constants.BaseRayThicknessInMM;
            
            _lcm.AddLightComponent(parallelSource, new Vector3(1.2f, 0, 0.62f));
            var lensObject = _ocm.AddOpticalComponent(lens, new Vector3(1.70f, 0, 0.62f));
            ((Lens)lensObject).SetParameters(R1: 0.30f, R2: -0.30f, d1: 0.015f, d2: 0.015f);
            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.065f, 2.6f, 1.0f), Constants.BaseCamRot, 36),
                new CameraControls.CameraSetting(new Vector3(-0.06f,3,2.1f), Constants.TopCamRot, 36)
            );
        }
        
        private void NormalEye()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.15f;
            
            var ps = _lcm.AddLightComponent(parallelSource, new Vector3(1.76f, 0, 0.577f));
            ((ParallelSource)ps).SetParameters(distanceBetweenRays: 0.50f / Constants.InMM, numberOfRays: 31);
            _ocm.AddOpticalComponent(eye, new Vector3(1.90f, 0, 0.577f));
            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.12f, 2.6f, 1f), Constants.BaseCamRot, 3),
                new CameraControls.CameraSetting(new Vector3(-0.12f,3,2.08f), Constants.TopCamRot, 3)
            );
        }
        
        private void NearsightedEye()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.15f;
            
            var ps = _lcm.AddLightComponent(parallelSource, new Vector3(1.76f, 0, 0.577f));
            ((ParallelSource)ps).SetParameters(distanceBetweenRays: 0.50f / Constants.InMM, numberOfRays: 31);
            
            var eyeObject = _ocm.AddOpticalComponent(eye, new Vector3(1.90f, 0, 0.577f));
            ((Eye)eyeObject).SetParameters(0.022f);
            
            var lensObject = _ocm.AddOpticalComponent(lens, new Vector3(1.882f, 0, 0.577f + Constants.Epsilon));
            ((Lens)lensObject).SetParameters(R1: 0.05f, R2: 0.024f, d1: 0.001f, d2: 0.001f, A: 1.458f, B: 3540);
            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.12f, 2.6f, 1f), Constants.BaseCamRot, 3),
                new CameraControls.CameraSetting(new Vector3(-0.12f,3,2.08f), Constants.TopCamRot, 3)
            );
        }
        
        private void FarsightedEye()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.15f;
            
            var ps = _lcm.AddLightComponent(parallelSource, new Vector3(1.76f, 0, 0.577f));
            ((ParallelSource)ps).SetParameters(distanceBetweenRays: 0.50f / Constants.InMM, numberOfRays: 31);
            
            var eyeObject = _ocm.AddOpticalComponent(eye, new Vector3(1.90f, 0, 0.577f));
            ((Eye)eyeObject).SetParameters(0.026f);
            
            var lensObject = _ocm.AddOpticalComponent(lens, new Vector3(1.882f, 0, 0.577f + Constants.Epsilon));
            ((Lens)lensObject).SetParameters(R1: 0.053f, R2: 0.10f, d1: 0.0005f, d2: 0.0005f, A: 1.458f, B: 3540);
            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.12f, 2.6f, 1f), Constants.BaseCamRot, 3),
                new CameraControls.CameraSetting(new Vector3(-0.12f,3,2.08f), Constants.TopCamRot, 3)
            );
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
            _uim.rayThickness.Value = 0.10f;
            
            var ps = _lcm.AddLightComponent(parallelSource, new Vector3(1.65f, 0, 0.577f));
            ((ParallelSource)ps).SetParameters(distanceBetweenRays: 0.80f / Constants.InMM, numberOfRays: 40);
            
            var ap = _ocm.AddOpticalComponent(aperture, new Vector3(1.75f, 0, 0.577f));
            ((Aperture)ap).SetParameters(Rin: 0.005f, Rout: 0.05f);
            
            var lensObject1 = _ocm.AddOpticalComponent(lens, new Vector3(1.77f, 0, 0.577f));
            ((Lens)lensObject1).SetParameters(R1: 0.112f, R2: -0.112f, Rc: 0.015f, d1: 0.0015f, d2: 0.0015f, A: 1.7f, B: 0);
            
            var lensObject2 = _ocm.AddOpticalComponent(lens, new Vector3(1.87f, 0, 0.577f));
            ((Lens)lensObject2).SetParameters(R1: 0.028f, R2: -0.028f, Rc: 0.05f, d1: 0.001f, d2: 0.001f, A: 1.7f, B: 0);
            
            _ocm.AddOpticalComponent(eye, new Vector3(1.887f, 0, 0.577f));
            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.14f, 2.6f, 1f), Constants.BaseCamRot, 5),
                new CameraControls.CameraSetting(new Vector3(-0.14f,3,2.08f), Constants.TopCamRot, 5)
            );
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
        
        
    }
}