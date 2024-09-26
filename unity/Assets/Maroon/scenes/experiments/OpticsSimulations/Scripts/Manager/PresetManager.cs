using System.Collections.Generic;
using Maroon.Physics.Optics.Camera;
using Maroon.Physics.Optics.TableObject.LightComponent;
using Maroon.Physics.Optics.TableObject.OpticalComponent;
using Maroon.Physics.Optics.Util;
using UnityEngine;

namespace Maroon.Physics.Optics.Manager
{
    public class PresetManager : MonoBehaviour
    {
        public static PresetManager Instance;
        
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
        private CameraControls _camControls;

        private OpticalComponentManager _ocm;
        private LightComponentManager _lcm;
        private UIManager _uim;
        private ExperimentManager _em;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogError("SHOULD NOT OCCUR - Destroyed PresetManager");
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            _ocm = OpticalComponentManager.Instance;
            _lcm = LightComponentManager.Instance;
            _uim = UIManager.Instance;
            _em = ExperimentManager.Instance;
            _camControls = mainCamera.GetComponent<CameraControls>();
            
            LensAndMirror();
        }

        public void TablePresets(int nr)
        {
            switch ((TablePreset) nr)
            {
                case TablePreset.Undefined: return;
                case TablePreset.LensAndMirror: LensAndMirror(); break;
                case TablePreset.FocalLength: FocalLength(); break;
                case TablePreset.StandardEye: StandardEye(); break;
                case TablePreset.NearsightedEye: NearsightedEye(); break;
                case TablePreset.FarsightedEye: FarsightedEye(); break;
                case TablePreset.MagnifyingGlass: MagnifyingGlass(); break;
                case TablePreset.KeplerianTelescope: KeplerianTelescope(); break;
                case TablePreset.GalileanTelescope: GalileanTelescope(); break;
                case TablePreset.NewtonianTelescope: NewtonianTelescope(); break;
                case TablePreset.Microscope: Microscope(); break;
                case TablePreset.LightEmittingDiode: LightEmittingDiode(); break;
                case TablePreset.OpticalFiber: OpticalFiber(); break;
            }
        }

        private void LoadExperimentParameters(OpticsParameters experimentParameters)
        {
            _em.ClearTable();
            _uim.rayThickness.Value = experimentParameters.rayThickness;

            foreach (OpticalComponentParameters componentParameters in experimentParameters.tableObjectParameters)
            {
                if (componentParameters is LightComponentParameters)
                {

                }
                else if (componentParameters is OpticalComponentParameters)
                {

                }
                else
                {
                    Debug.LogError("Unknown OpticalComponentParameters class.");
                }
            }

            _camControls.SetPresetCameras(
                experimentParameters.cameraSettingBaseView,
                experimentParameters.cameraSettingTopView
            );
        }

        #region Old Presets
        private void LensAndMirror()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = Constants.BaseRayThicknessInMM;
            _lcm.AddLightComponent(laserPointer, new Vector3(1.30f, 0, 0.60f), wavelengths: new List<float> {390f, 440f, 490f, 540f, 590f, 640f, 720f});
            _ocm.AddOpticalComponent(lens, new Vector3(1.6f, 0, 0.57f));
            _ocm.AddOpticalComponent(mirror, new Vector3(2.2f, 0, 0.40f), new Vector3(1, 0, -1));
            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.065f, 2.6f, 1.0f), Constants.BaseCamRot, 30),
                new CameraControls.CameraSetting(new Vector3(-0.065f, 3.0f, 2.0f), Constants.TopCamRot, 33)
            );
        }

        private void FocalLength()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = Constants.BaseRayThicknessInMM;
            
            _lcm.AddLightComponent(parallelSource, new Vector3(1.2f, 0, 0.62f));
            var lensObject = _ocm.AddOpticalComponent(lens, new Vector3(1.70f, 0, 0.62f));
            ((Lens)lensObject).SetParameters(R1: Constants.Biconvex.Item1, R2: Constants.Biconvex.Item2, d1: Constants.Biconvex.Item3, d2: Constants.Biconvex.Item4);
            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.065f, 2.6f, 1.0f), Constants.BaseCamRot, 36),
                new CameraControls.CameraSetting(new Vector3(-0.06f,3,2.1f), Constants.TopCamRot, 36)
            );
        }
        
        private void StandardEye()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.15f;
            
            var ps = _lcm.AddLightComponent(parallelSource, new Vector3(1.78f, 0, 0.577f));
            ((ParallelSource)ps).SetParameters(distanceBetweenRays: 0.40f / Constants.InMM, numberOfRays: 20);
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
            
            var ps = _lcm.AddLightComponent(parallelSource, new Vector3(1.78f, 0, 0.577f));
            ((ParallelSource)ps).SetParameters(distanceBetweenRays: 0.40f / Constants.InMM, numberOfRays: 20);
            
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
            
            var ps = _lcm.AddLightComponent(pointSource, new Vector3(1.638f, 0, 0.577f));
            ((PointSource)ps).SetParameters(40, 4);
            
            _ocm.AddOpticalComponent(eye, new Vector3(1.90f, 0, 0.577f));
            
            var lensObject = _ocm.AddOpticalComponent(lens, new Vector3(1.882f, 0, 0.577f + Constants.Epsilon));
            ((Lens)lensObject).SetParameters(R1: 0.053f, R2: 0.10f, d1: 0.0005f, d2: 0.0005f, A: 1.458f, B: 3540);
            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.12f, 2.6f, 1f), Constants.BaseCamRot, 3),
                new CameraControls.CameraSetting(new Vector3(-0.12f,3,2.08f), Constants.TopCamRot, 3)
            );
        }
        
        private void MagnifyingGlass()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.15f;
            
            var ps = _lcm.AddLightComponent(pointSource, new Vector3(1.78f, 0, 0.577f));
            ((PointSource)ps).SetParameters(40, 22);
            
            var lensObject = _ocm.AddOpticalComponent(lens, new Vector3(1.866f, 0, 0.577f + Constants.Epsilon));
            ((Lens)lensObject).SetParameters(R1: 0.08f, R2: -0.08f, d1: 0.002f, d2: 0.002f, A: 1.458f, B: 3540);
            
            _ocm.AddOpticalComponent(eye, new Vector3(1.90f, 0, 0.577f));
            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.12f, 2.6f, 1f), Constants.BaseCamRot, 3),
                new CameraControls.CameraSetting(new Vector3(-0.12f,3,2.08f), Constants.TopCamRot, 3)
            );
        }
        
        private void KeplerianTelescope()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.15f;

            var ps = _lcm.AddLightComponent(parallelSource, new Vector3(1.65f, 0, 0.577f));
            ((ParallelSource)ps).SetParameters(distanceBetweenRays: 1.40f / Constants.InMM, numberOfRays: 20);
            
            var ap = _ocm.AddOpticalComponent(aperture, new Vector3(1.74f, 0, 0.577f));
            ((Aperture)ap).SetParameters(Rin: 0.005f, Rout: 0.015f);
            
            var lensObject1 = _ocm.AddOpticalComponent(lens, new Vector3(1.75f, 0, 0.577f));
            ((Lens)lensObject1).SetParameters(R1: 0.112f, R2: -0.112f, Rc: 0.015f, d1: 0.0015f, d2: 0.0015f, A: 1.7f, B: 0);
            
            var lensObject2 = _ocm.AddOpticalComponent(lens, new Vector3(1.85f, 0, 0.577f));
            ((Lens)lensObject2).SetParameters(R1: 0.028f, R2: -0.028f, Rc: 0.05f, d1: 0.001f, d2: 0.001f, A: 1.7f, B: 0);
            
            _ocm.AddOpticalComponent(eye, new Vector3(1.867f, 0, 0.577f));
            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.1682f, 2.6f, 1f), Constants.BaseCamRot, 3.5f),
                new CameraControls.CameraSetting(new Vector3(-0.1682f,3,2.08f), Constants.TopCamRot, 3.5f)
            );
        }
        
        private void GalileanTelescope()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.15f;

            // Upper part
            var ps = _lcm.AddLightComponent(parallelSource, new Vector3(1.65f, 0, 0.60f));
            ((ParallelSource)ps).SetParameters(distanceBetweenRays: 1.40f / Constants.InMM, numberOfRays: 20);
            
            var ap = _ocm.AddOpticalComponent(aperture, new Vector3(1.783f, 0, 0.60f));
            ((Aperture)ap).SetParameters(Rin: 0.005f, Rout: 0.015f);
            
            var lensObject1 = _ocm.AddOpticalComponent(lens, new Vector3(1.788f, 0, 0.60f));
            ((Lens)lensObject1).SetParameters(R1: 0.112f, R2: -0.112f, Rc: 0.015f, d1: 0.0015f, d2: 0.0015f, A: 1.7f, B: 0);
            
            var lensObject2 = _ocm.AddOpticalComponent(lens, new Vector3(1.848f, 0, 0.60f));
            ((Lens)lensObject2).SetParameters(R1: -0.028f, R2: 0.028f, Rc: 0.005f, d1: 0.00005f, d2: 0.00005f, A: 1.7f, B: 0);
            
            _ocm.AddOpticalComponent(eye, new Vector3(1.865f, 0, 0.60f));
            
            // Lower part (4 cm apart)
            var ps1 = _lcm.AddLightComponent(parallelSource, new Vector3(1.65f, 0, 0.56f));
            ((ParallelSource)ps1).SetParameters(distanceBetweenRays: 1.40f / Constants.InMM, numberOfRays: 20);
            
            var ap2 = _ocm.AddOpticalComponent(aperture, new Vector3(1.783f, 0, 0.56f));
            ((Aperture)ap2).SetParameters(Rin: 0.005f, Rout: 0.015f);
            
            var lensObject3 = _ocm.AddOpticalComponent(lens, new Vector3(1.788f, 0, 0.56f));
            ((Lens)lensObject3).SetParameters(R1: 0.112f, R2: -0.112f, Rc: 0.015f, d1: 0.0015f, d2: 0.0015f, A: 1.7f, B: 0);
            
            var lensObject4 = _ocm.AddOpticalComponent(lens, new Vector3(1.848f, 0, 0.56f));
            ((Lens)lensObject4).SetParameters(R1: -0.028f, R2: 0.028f, Rc: 0.005f, d1: 0.00005f, d2: 0.00005f, A: 1.7f, B: 0);
            
            _ocm.AddOpticalComponent(eye, new Vector3(1.865f, 0, 0.56f));
            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.168f, 2.6f, 1f), Constants.BaseCamRot, 3.5f),
                new CameraControls.CameraSetting(new Vector3(-0.168f,3,2.08f), Constants.TopCamRot, 3.5f)
            );
        }
        
        private void NewtonianTelescope()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.15f;

            var ps1 = _lcm.AddLightComponent(parallelSource, new Vector3(1.65f, 0, 0.55f));
            ((ParallelSource)ps1).SetParameters(distanceBetweenRays: 1.40f / Constants.InMM, numberOfRays: 20);
            
            var ps2 = _lcm.AddLightComponent(parallelSource, new Vector3(1.65f, 0, 0.55f + 0.028f));
            ((ParallelSource)ps2).SetParameters(distanceBetweenRays: 1.40f / Constants.InMM, numberOfRays: 20);
            
            var ps3 = _lcm.AddLightComponent(parallelSource, new Vector3(1.65f, 0, 0.55f + 2 * 0.028f));
            ((ParallelSource)ps3).SetParameters(distanceBetweenRays: 1.40f / Constants.InMM, numberOfRays: 20);
            
            var ap1 = _ocm.AddOpticalComponent(aperture, new Vector3(1.735f, 0, 0.56f));
            ((Aperture)ap1).SetParameters(Rin: 0.019f, Rout: 0.10f);
            
            var mirrorObject1 = _ocm.AddOpticalComponent(mirror, new Vector3(1.856f, 0, 0.56f));
            ((TableObject.OpticalComponent.Mirror)mirrorObject1).SetParameters(-0.20f, 0.02f);
            
            var mirrorObject2 = _ocm.AddOpticalComponent(mirror, new Vector3(1.77f, 0, 0.56f), new Vector3(1, 0, 1));
            ((TableObject.OpticalComponent.Mirror)mirrorObject2).SetParameters(0.5f, 0.006f);
            
            var ap2 = _ocm.AddOpticalComponent(aperture, new Vector3(1.77f, 0, 0.5595f), new Vector3(1, 0, 1));
            ((Aperture)ap2).SetParameters(Rin: 0, Rout: 0.006f);
            
            var lensObject = _ocm.AddOpticalComponent(lens, new Vector3(1.77f, 0, 0.592f), new Vector3(0, 0, 1));
            ((Lens)lensObject).SetParameters(R1: 0.024f, R2: -0.024f, Rc: 0.006855f, d1: 0.001f, d2: 0.001f, A: 1.63f, B: 0);
            
            _ocm.AddOpticalComponent(eye, new Vector3(1.77f, 0, 0.611f), new Vector3(0, 0, 1));

            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.1825f, 2.6f, 1f), Constants.BaseCamRot, 3.5f),
                new CameraControls.CameraSetting(new Vector3(-0.1825f,3,2.08f), Constants.TopCamRot, 3.5f)
            );
        }
        
        private void Microscope()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.08f;

            var ps = _lcm.AddLightComponent(pointSource, new Vector3(1.78f, 0, 0.577f));
            ((PointSource)ps).SetParameters(40, 22);
            
            var ap = _ocm.AddOpticalComponent(aperture, new Vector3(1.809f, 0, 0.577f));
            ((Aperture)ap).SetParameters(Rin: 0.002f, Rout: 0.05f);
            
            var lensObject1 = _ocm.AddOpticalComponent(lens, new Vector3(1.81f, 0, 0.577f));
            ((Lens)lensObject1).SetParameters(R1: 0.028f, R2: -0.028f, Rc: 0.52678f, d1: 0.0005f, d2: 0.0005f, A: 1.7f, B: 0);
            
            var lensObject2 = _ocm.AddOpticalComponent(lens, new Vector3(1.89f, 0, 0.577f));
            ((Lens)lensObject2).SetParameters(R1: 0.028f, R2: -0.028f, Rc: 0.52678f, d1: 0.0005f, d2: 0.0005f, A: 1.7f, B: 0);
            
            _ocm.AddOpticalComponent(eye, new Vector3(1.907f, 0, 0.577f));
            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.12f, 2.6f, 1f), Constants.BaseCamRot, 3f),
                new CameraControls.CameraSetting(new Vector3(-0.12f,3,2.08f), Constants.TopCamRot, 3f)
            );
        }
        
        private void LightEmittingDiode()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.30f;

            var ps = _lcm.AddLightComponent(pointSource, new Vector3(1.82f, 0, 0.56f));
            ((PointSource)ps).SetParameters(40, 360);
            ((PointSource)ps).Component.GetComponent<MeshRenderer>().enabled = false;
            
            var lensObject = _ocm.AddOpticalComponent(lens, new Vector3(1.82f, 0, 0.57f), new Vector3(0, 0, -1));
            ((Lens)lensObject).SetParameters(R1: 0.03f, R2: 0.5f, Rc: 0.03f, d1: 0.04f, d2: 0.04f, A: 1.41f, B: 0);
            
            var mirrorObject = _ocm.AddOpticalComponent(mirror, new Vector3(1.82f, 0, 0.55f), new Vector3(0, 0, 1));
            ((TableObject.OpticalComponent.Mirror)mirrorObject).SetParameters(0.01f, 0.01f);
            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.17f, 2.6f, 1f), Constants.BaseCamRot, 7f),
                new CameraControls.CameraSetting(new Vector3(-0.17f,3,2.08f), Constants.TopCamRot, 7f)
            );
        }
        
        private void OpticalFiber()
        {
            _em.ClearTable();
            _uim.rayThickness.Value = 0.15f;
            
            var ps = _lcm.AddLightComponent(parallelSource, new Vector3(1.75f, 0, 0.577f));
            ((ParallelSource)ps).SetParameters(distanceBetweenRays: 0.40f / Constants.InMM, numberOfRays: 20);
            
            var lensObject1 = _ocm.AddOpticalComponent(lens, new Vector3(1.80f, 0, 0.577f));
            ((Lens)lensObject1).SetParameters(R1: 0.03f, R2: -0.03f, Rc: 0.009367f, d1: 0.0015f, d2: 0.0015f, A: 1.7f, B: 0);
            
            var lensObject2 = _ocm.AddOpticalComponent(lens, new Vector3(1.872f, 0, 0.577f));
            ((Lens)lensObject2).SetParameters(R1: 0.5f, R2: -0.5f, Rc: 0.002f, d1: 0.05f, d2: 0.05f, A: 1.7f, B: 0);
            
            _camControls.SetPresetCameras(
                new CameraControls.CameraSetting(new Vector3(-0.124f, 2.6f, 1f), Constants.BaseCamRot, 3),
                new CameraControls.CameraSetting(new Vector3(-0.124f,3,2.08f), Constants.TopCamRot, 3)
            );
        }
        
        private enum TablePreset
        {
            Undefined,
            LensAndMirror,
            FocalLength,
            StandardEye,
            NearsightedEye,
            FarsightedEye,
            MagnifyingGlass,
            KeplerianTelescope,
            GalileanTelescope,
            NewtonianTelescope,
            Microscope,
            LightEmittingDiode,
            OpticalFiber
        }
        #endregion
    }
}