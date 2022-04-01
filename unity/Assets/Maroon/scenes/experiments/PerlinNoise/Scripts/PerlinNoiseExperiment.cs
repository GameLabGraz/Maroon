using System;
using System.Collections.Generic;
using System.Linq;
using Maroon.UI;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{

    public abstract class NoiseExperiment : MonoBehaviour
    {
        public GameObject panel;
        public abstract void GenerateMesh(Mesh mesh);
        public abstract void UpdateMesh(Mesh mesh);

        public string experiment_name;
    }
    
    public class PerlinNoiseExperiment : MonoBehaviour
    {

        [SerializeField] public NoiseExperiment noise_experiment;
        

        [SerializeField, Range(0, 20)] private float rotation_speed = 0;
        [Space(20)][Header("EditorControl"),SerializeField] private bool force_refresh;
        [SerializeField] private bool animated;
        private MeshFilter meshFilter;
        private float rotation;
        [SerializeField]private bool dirty;

        [SerializeField] private NoiseExperiment[] experiments;
        [SerializeField] private Dropdown type_selection;

        public void SetDirty() => dirty = true;

        public void SelectExperiment(int i)
        {
            if (experiments.IsValidIndex(i))
            {
                for (var index = 0; index < experiments.Length; index++)
                {
                    var experiment = experiments[index];
                    experiment.panel.SetActive(i == index);
                }

                noise_experiment = experiments[i];
                noise_experiment.GenerateMesh(meshFilter.mesh);
                dirty = true;
            }
        }

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
            SelectExperiment(0);
            
            
#if UNITY_EDITOR
            animated = false;
            while (EditorApplication.update.GetInvocationList().Contains((Action) Update))
                EditorApplication.update -= Update;
            #endif
        }

        // Update is called once per frame
        void Update()
        {
            if (SimulationController.Instance && SimulationController.Instance.SimulationRunning || animated)
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
         //   x += Mathf.Sin(y * 10f) * 0.2f;
         //   y += Mathf.Sin(x * 10f) * 0.2f;
          //  x = Mathf.PerlinNoise(x, y);
           // y = Mathf.PerlinNoise(y, x);
            return Mathf.PerlinNoise(x, y);
            
            
            x += Mathf.Sin(y);
            y += Mathf.Sin(x);
          //  return Mathf.PerlinNoise(x, y);
          return Mathf.Sin(Mathf.PI * (1 + Mathf.PerlinNoise(x, y)));
        }

        public void OnValidate()
        {
            #if UNITY_EDITOR
            if (animated)
            {
                if (!EditorApplication.update.GetInvocationList().Contains((Action) Update))
                    EditorApplication.update += Update;
            }
            else
            {
                while (EditorApplication.update.GetInvocationList().Contains((Action) Update))
                    EditorApplication.update -= Update;
            }
            #endif

            type_selection.options = experiments.Select(e => new TMP_Dropdown.OptionData(e.experiment_name)).ToList();
            type_selection.onValueChanged.RemoveAllListeners();
            type_selection.onValueChanged.AddListener(SelectExperiment);
            
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
        
        public static float Clamp01(this float f) => Clamp(f, 0, 1);

        // clamps f to be 0 < f < 1
        public static float Clamp01Exclusive(this float f) => Clamp(f, float.Epsilon, 1f - 1e-7f);

        public static float Clamp(this float f, float min, float max) => Mathf.Max(Mathf.Min(f, max), min);

        public static bool IsValidIndex<T>(this T[] list, int index) => index >= 0 && index < list.Length;
        public static bool IsValidIndex<T>(this IEnumerable<T> list, int index) => index >= 0 && index < list.Count();
        public static bool IsValidIndex<T>(this List<T> list, int index) => index >= 0 && index < list.Count;

        public static float Map(this float value, float from_source, float to_source, float from_target = 0,
            float to_target = 1)
        {
            return (value - from_source) / (to_source - from_source) * (to_target - from_target) + from_target;
        }

    }
}
