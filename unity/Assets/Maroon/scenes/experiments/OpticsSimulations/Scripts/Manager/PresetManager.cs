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
        
        
        public void LensAndMirror()
        {
            OpticalComponentManager ocm = OpticalComponentManager.Instance;
            LightComponentManager lcm = LightComponentManager.Instance;
            
            ExperimentManager.Instance.ClearTable();
            
            ocm.AddOpticalComponent(lens, new Vector3(1.687f, 0, 0.687f));
            ocm.AddOpticalComponent(mirror, new Vector3(2.2729f, 0, 0.4801f), new Vector3(0, 90, 0));

            float wl = 390;
            for (int i = 0; i < 7; i++)
            {
                laserPointer.Wavelength = wl;
                lcm.AddLightComponent(laserPointer, new Vector3(1.22f, 0, 0.788f));
                wl += 50;
            }
            
            
        }
    }
}