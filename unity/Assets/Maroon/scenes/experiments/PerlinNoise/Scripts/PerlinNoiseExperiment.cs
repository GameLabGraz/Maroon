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

        [Space(20)] [Header("EditorControl"), SerializeField]
        private bool force_refresh;

        [SerializeField] private bool animated;
        private MeshFilter meshFilter;
        private float rotation;
        [SerializeField] private bool dirty;

        [SerializeField] private NoiseExperiment[] experiments;
        [SerializeField] private Dropdown type_selection;

        private (Color32 top, Color32 middle, Color32 bottom) colors = (Color.gray, Color.yellow, Color.cyan);

        public void SetTopColor(float color)
        {
            dirty = true;
            colors.top = Color.HSVToRGB(color, 1, 1);
        }

        public void SetBottomColor(float color)
        {
            dirty = true;
            colors.bottom = Color.HSVToRGB(color, 1, 1);
        }

        public void SetMiddleColor(float color)
        {
            dirty = true;
            colors.middle = Color.HSVToRGB(color, 1, 1);
        }


        public Color GetVertexColor(float value, float bottom, float middle, float top)
        {
            if (value > middle)
                return Color.Lerp(colors.middle, colors.top, value.Map(middle, top));
            return Color.Lerp(colors.bottom, colors.middle, value.Map(bottom, middle));
        }

        public void SetDirty() => dirty = true;

        public void ResetSeed() => simplex_noise = new OpenSimplexNoise();

        [Space(10)] [SerializeField, Range(0, 2)]
        float speed = 1;

        private static OpenSimplexNoise simplex_noise = new OpenSimplexNoise();

        public float time { get; private set; }

        public void SelectExperiment(int i)
        {
            if (!experiments.IsValidIndex(i))
                return;

            for (var index = 0; index < experiments.Length; index++)
            {
                var experiment = experiments[index];
                experiment.panel.SetActive(i == index);
            }

            noise_experiment = experiments[i];
            noise_experiment.GenerateMesh(meshFilter.mesh);
            dirty = true;
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
                time += Time.deltaTime * speed;
                dirty = true;
            }

            if (!dirty)
                return;
            noise_experiment.UpdateMesh(meshFilter.sharedMesh);
            dirty = false;
        }

        public static float PerlinNoise2D(float x, float y, float octaves)
        {
            var noise = 0.0;
            int i;
            for (i = 1; i < octaves - float.Epsilon; i++)
                noise += (simplex_noise.Evaluate(x * i, y * i) - 0.5f) / i;
            var last_octave_fraction = octaves - i + 1;
            noise += (simplex_noise.Evaluate(x * i, y * i) - 0.5f) * last_octave_fraction / i;

            return (float) noise;
        }

        public static float PerlinNoise3D(float x, float y, float z, float octaves)
        {
            var noise = 0.0;
            int i;
            for (i = 1; i < octaves - float.Epsilon; i++)
                noise += (simplex_noise.Evaluate(x * i, y * i, z * i) - 0.5f) / i;
            var last_octave_fraction = octaves - i + 1;
            noise += (simplex_noise.Evaluate(x * i, y * i, z * i) - 0.5f) * last_octave_fraction / i;

            return (float) noise;
        }

        public static float PerlinNoise3D(float x, float y, float z) =>
            (float) simplex_noise.Evaluate(x, y, z) - 0.5f;

        public static float PerlinNoise2D(float x, float y) =>
            (float) simplex_noise.Evaluate(x, y) - 0.5f;


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

            if (force_refresh)
            {
                ResetSeed();
                noise_experiment.GenerateMesh(meshFilter.sharedMesh);
                force_refresh = false;
                return;
            }

            try
            {
                noise_experiment.UpdateMesh(meshFilter.sharedMesh);
            }
            catch
            {
                noise_experiment.GenerateMesh(meshFilter.sharedMesh);
            }
        }
    }

    public static class Utils
    {
        public static Vector2 half_vector_2 => new Vector2(0.5f, 0.5f);
        public static Vector3 half_vector_3 => new Vector3(0.5f, 0.5f, 0.5f);

        public static float Clamp01(this float f) => Clamp(f, 0, 1);

        // clamps f to be 0 < f < 1
        public static float Clamp01Exclusive(this float f) => Clamp(f, float.Epsilon, 1f - 1e-7f);

        public static float Clamp(this float f, float min, float max) => Mathf.Max(Mathf.Min(f, max), min);

        public static bool IsValidIndex<T>(this T[] list, int index) => index >= 0 && index < list.Length;
        public static bool IsValidIndex<T>(this IEnumerable<T> list, int index) => index >= 0 && index < list.Count();
        public static bool IsValidIndex<T>(this List<T> list, int index) => index >= 0 && index < list.Count;

        public static float Map(this float value, float min_source, float max_source, float min_target = 0,
            float max_target = 1)
        {
            return (value - min_source) / (max_source - min_source) * (max_target - min_target) + min_target;
        }

        public static bool IsInRange(this int value, int min, int max) => value >= min && value <= max;
    }
}
