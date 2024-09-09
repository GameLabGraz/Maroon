using System;
using System.Collections.Generic;
using Maroon.Physics;
using Maroon.scenes.experiments.PerlinNoise.Scripts.NoiseVisualisations;
using UnityEngine;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public class NoiseExperimentBase : MonoBehaviour
    {
        [SerializeField] protected NoiseVisualisation noise_visualisation;
        [SerializeField, Range(0, 20)] protected float rotation_speed = 0;
        public static readonly Noise noise = new Noise(0);

        [SerializeField, Header("Common Configuration")]
        public QuantityInt seed;

        [SerializeField] public QuantityInt size;
        [SerializeField] public QuantityFloat scale;
        [SerializeField] public QuantityFloat octaves;
        [SerializeField] private Gradient gradient;

        [Space(10)] [SerializeField]
        protected float speed = 1;

        protected bool dirty;
        protected bool dirtyImmediate;
        protected TimeSpan dirtyRefreshRate = new TimeSpan(0, 0, 0, 0, 50);

        public DateTime lastUpdate;

        private static NoiseExperimentBase _instance;

        public static NoiseExperimentBase Instance
        {
            get
            {
                if (!_instance)
                    _instance = FindObjectOfType<NoiseExperimentBase>();
                return _instance;
            }
        }

        protected MeshFilter meshFilter;
        protected bool is_rotating;
        private float _time;

        protected void Start()
        {
            if (!meshFilter)
                meshFilter = GetComponentInChildren<MeshFilter>();
            noise_visualisation.GenerateMesh(meshFilter.sharedMesh);
        }

        private void Update()
        {
            HandleUpdate();
        }


        protected virtual void HandleUpdate()
        {
            _time += Time.deltaTime * speed;
            noise.offset = _time;
            if (!meshFilter)
                meshFilter = GetComponentInChildren<MeshFilter>();
            noise_visualisation.UpdateMesh(meshFilter.sharedMesh);
            meshFilter.transform.Rotate(Vector3.up, Time.deltaTime * rotation_speed, Space.World);
        }

        public void SetDirty() => dirty = true;
        public void SetDirtyImmediate() => dirtyImmediate = true;


        public Color GetVertexColor(float value)
        {
            return gradient.Evaluate(value);
        }
        
        
        public void GetNoise(ref float[] data, float width, float y)
        {
            var factor = width / data.Length;
            for (var i = 0; i < data.Length; i++)
                data[i] = noise.GetNoise2D((i - data.Length / 2f) * scale * factor, y, octaves);
        }

        public void GetNoiseSizeDependent(ref List<float> data, float width, float y)
        {
            if (data.Count > size)
                data.RemoveRange(size, data.Count - size);
            var factor = width / (size - 1f);
            for (var i = 0; i < data.Count; i++)
                data[i] = noise.GetNoise2D((i - (size - 1) / 2f) * scale * factor, y, octaves);

            for (var i = data.Count; i < size; i++)
                data.Add(noise.GetNoise2D((i - (size - 1) / 2f) * scale * factor, y, octaves));
        }

        
        public float GetThreshold()
        {
            switch (noise_visualisation)
            {
                case NoiseVisualisationVoxel visualisationVoxel:
                    return -visualisationVoxel.Threshold + 0.5f;
                case NoiseVisualisationMarchingCubes marchingCubes:
                    return -marchingCubes.Threshold + 0.5f;
                default:
                    return 0;
            }
        }
    }
}