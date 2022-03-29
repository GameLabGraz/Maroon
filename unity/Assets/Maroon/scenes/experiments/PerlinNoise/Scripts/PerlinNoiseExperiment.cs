using System;
using UnityEngine;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{

    public interface INoiseExperiment
    {
        void GenerateMesh(Mesh mesh);
        void UpdateMesh(Mesh mesh);
    }
    
    public class PerlinNoiseExperiment : MonoBehaviour
    {
        private INoiseExperiment noise_experiment => noise_experiment_mb as INoiseExperiment;

        [SerializeField] public MonoBehaviour noise_experiment_mb;
        

        [SerializeField, Range(0, 20)] private float rotation_speed = 0;
        [SerializeField] private bool force_refresh;
        private MeshFilter meshFilter;
        private float rotation;
        [HideInInspector] public bool dirty;
        
        

        private static PerlinNoiseExperiment _instance;

        public static PerlinNoiseExperiment Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<PerlinNoiseExperiment>();

                return _instance;
            }
        }


        // Start is called before the first frame update
        void Start()
        {
            noise_experiment.GenerateMesh(meshFilter.sharedMesh);
            SimulationController.Instance.StartSimulation();
        }

        // Update is called once per frame
        void Update()
        {
            if (SimulationController.Instance && SimulationController.Instance.SimulationRunning)
            {
                rotation += Time.deltaTime * rotation_speed;
                meshFilter.transform.localRotation = Quaternion.Euler(Vector3.up * rotation);
                dirty = true;
            }

            if (dirty)
                noise_experiment.UpdateMesh(meshFilter.sharedMesh);
            dirty = false;
        }

        public static float PerlinNoise3D(float x, float y, float z, float octaves)
        {
            var xy = PerlinNoise2D(x, y, octaves);
            var xz = PerlinNoise2D(x, z, octaves);
            var yz = PerlinNoise2D(y, z, octaves);
            var yx = PerlinNoise2D(y, x, octaves);
            var zx = PerlinNoise2D(z, x, octaves);
            var zy = PerlinNoise2D(z, y, octaves);

            return (xy + xz + yz + yx + zx + zy) / 6;
        }

        public static float PerlinNoise2D(float x, float y, float octaves)
        {
            var noise = 0f;
            int i;
            for (i = 1; i < octaves - float.Epsilon; i++)
                noise += (PerlinNoiseIrregular(x * i, y * i) - 0.5f) / i;
            var last_octave_fraction = octaves - i + 1;
            noise += (PerlinNoiseIrregular(x * i, y * i) - 0.5f) * last_octave_fraction / i;

            return noise;
        }

        private static float PerlinNoiseIrregular(float x, float y)
        {
            return Mathf.PerlinNoise(x, y);
            return Mathf.Sin(Mathf.PI * (1 + Mathf.PerlinNoise(x, y)));
        }

        public void OnValidate()
        {
            if (!meshFilter)
                meshFilter = GetComponentInChildren<MeshFilter>();
            if (!meshFilter)
                return;
            
            if(force_refresh)
            {
                noise_experiment.GenerateMesh(meshFilter.sharedMesh);
                force_refresh = false;
                return;
            }

            try
            {
                noise_experiment.UpdateMesh(meshFilter.sharedMesh);
            }
            catch (Exception)
            {
                noise_experiment.GenerateMesh(meshFilter.sharedMesh);
            }
        }
    }

    public static class Utils
    {
        public static Vector2 half_vector => new Vector2(0.5f, 0.5f);
    }
}
