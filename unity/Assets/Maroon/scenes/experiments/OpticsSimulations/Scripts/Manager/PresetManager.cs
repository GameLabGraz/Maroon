using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent;
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
            OpticalComponentManager ocm = OpticalComponentManager.Instance;
            LightComponentManager lcm = LightComponentManager.Instance;
            ExperimentManager.Instance.ClearTable();
            
            ocm.AddOpticalComponent(lens, new Vector3(1.70f, 0, 0.70f));
            ocm.AddOpticalComponent(mirror, new Vector3(2.30f, 0, 0.57f), Vector3.back);
            lcm.AddLightComponent(laserPointer, new Vector3(1.2f, 0, 0.75f), new List<float> {390f, 440f, 490f, 540f, 590f, 640f, 720f});
        }

        private void FocalLength()
        {
            OpticalComponentManager ocm = OpticalComponentManager.Instance;
            LightComponentManager lcm = LightComponentManager.Instance;
            ExperimentManager.Instance.ClearTable();
            
            var lensObject = ocm.AddOpticalComponent(lens, new Vector3(2.00f, 0, 1.00f));
            ((Lens)lensObject).SetParameters(R1: 0.30f, R2: -0.30f, d1: 0.015f, d2: 0.015f);
            lcm.AddLightComponent(parallelSource, new Vector3(1.00f, 0, 1.00f));
        }
        
        private void NormalEye()
        {
            
        }
        
        private void NearsightedEye()
        {
            
        }
        
        private void FarsightedEye()
        {
            
        }
        
        private void UnderwaterVision()
        {
            
        }
        
        private void MagnifyingGlass()
        {
            
        }
        
        private void KeplerianTelescope()
        {
            
        }
        
        private void GalileanTelescope()
        {
            
        }
        
        private void NewtonianTelescope()
        {
            
        }
        
        private void Microscope()
        {
            
        }
        
        private void LightEmittingDiode()
        {
            
        }
        
        private void OpticalFiber()
        {
            
        }
    }
}