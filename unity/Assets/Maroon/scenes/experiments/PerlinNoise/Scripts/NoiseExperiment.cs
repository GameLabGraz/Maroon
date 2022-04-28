using System;
using Maroon.Physics;
using UnityEditor;
using UnityEngine;
using System.Linq;
using AssetUsageDetectorNamespace;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public abstract class NoiseType : MonoBehaviour
    {
        public abstract bool GenerateNoiseMap(Noise noise);
        public abstract bool GetNoiseMapValue(Vector3 index);
        public abstract bool GetNoiseMapValue(Vector3Int index);
        public abstract float GetNoiseMapValue(Vector2 index);
        public abstract float GetNoiseMapValue(Vector2Int index);
    }

    public abstract class NoiseVisualisation : MonoBehaviour
    {
        public GameObject panel;
        public abstract void GenerateMesh(Mesh mesh, NoiseType noiseType);
        public abstract void UpdateMesh(Mesh mesh, NoiseType noiseType);

        public string experiment_name;
    }

    public class NoiseExperiment : MonoBehaviour
    {
        [SerializeField] private NoiseType noise_type;
        [SerializeField] private NoiseVisualisation noise_visualisation;

        [SerializeField] private NoiseType[] noise_types;
        [SerializeField] private NoiseVisualisation[] noise_visualisations;

        [SerializeField] private bool dirty;
        [SerializeField] private bool animated;
        [SerializeField, Range(0, 20)] private float rotation_speed = 0;

        [SerializeField, Header("Common Configuration")]
        private QuantityFloat seed;

        [SerializeField] public QuantityInt size;
        [SerializeField] public QuantityFloat scale;
        [SerializeField] public QuantityFloat octaves;


        [Space(10)] [SerializeField, Range(0, 2)]
        float speed = 1;

        [Header("EditorControl"), SerializeField]
        private bool force_refresh;

        public float time { get; private set; }

        private MeshFilter meshFilter;
        private float rotation;
        private static readonly Noise noise = new Noise(0);


        private static NoiseExperiment _instance;

        public static NoiseExperiment Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<NoiseExperiment>();

                return _instance;
            }
        }


        private (Color32 top, Color32 middle, Color32 bottom) colors = (Color.gray, Color.yellow, Color.cyan);

        public void SetTopColor(float color)
        {
            NoiseExperiment.Instance.SetDirty();
            colors.top = Color.HSVToRGB(color, 1, 1);
        }

        public void SetBottomColor(float color)
        {
            NoiseExperiment.Instance.SetDirty();
            colors.bottom = Color.HSVToRGB(color, 1, 1);
        }

        public void SetMiddleColor(float color)
        {
            NoiseExperiment.Instance.SetDirty();
            colors.middle = Color.HSVToRGB(color, 1, 1);
        }

        public Color GetVertexColor(float value, float bottom, float middle, float top)
        {
            if (value > middle)
                return Color.Lerp(colors.middle, colors.top, value.Map(middle, top));
            return Color.Lerp(colors.bottom, colors.middle, value.Map(bottom, middle));
        }

        public void OnSelectNoiseType(int index, string _)
        {
            if (!noise_types.IsValidIndex(index))
                return;
            dirty = true;
            force_refresh = true;
            noise_type = noise_types[index];
        }

        public void OnSelectVisualisation(int index, string _)
        {
            if (!noise_visualisations.IsValidIndex(index))
                return;
            dirty = true;
            force_refresh = true;
            noise_visualisation = noise_visualisations[index];
        }


        public void SetDirty() => dirty = true;


        // Start is called before the first frame update
        void Start()
        {
            noise_type.GenerateNoiseMap(noise);
            noise_visualisation.GenerateMesh(meshFilter.sharedMesh, noise_type);
            SimulationController.Instance.StartSimulation();

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
            if (force_refresh)
            {
                noise_type.GenerateNoiseMap(noise);
                noise_visualisation.UpdateMesh(meshFilter.sharedMesh, noise_type);
            }
            else if (noise_type.GenerateNoiseMap(noise))
                noise_visualisation.UpdateMesh(meshFilter.sharedMesh, noise_type);

            dirty = false;
        }


        private void OnValidate()
        {
            noise_types = GetComponents<NoiseType>();
            noise_visualisations = GetComponents<NoiseVisualisation>();
            if (noise_types.IsEmpty() || noise_visualisations.IsEmpty())
                return;

            if (noise_type == null && noise_types.Length != 0)
                noise_type = noise_types[0];
            if (noise_visualisation == null && noise_visualisations.Length != 0)
                noise_visualisation = noise_visualisations[0];

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

            seed.onValueChanged.RemoveAllListeners();
            seed.onValueChanged.AddListener(_ => SetDirty());

            if (!meshFilter)
                meshFilter = GetComponentInChildren<MeshFilter>();
            if (!meshFilter)
                return;

            if (force_refresh)
            {
                noise_type.GenerateNoiseMap(noise);
                noise_visualisation.GenerateMesh(meshFilter.sharedMesh, noise_type);
                force_refresh = false;
                return;
            }

            try
            {
                if (noise_type.GenerateNoiseMap(noise))
                    noise_visualisation.UpdateMesh(meshFilter.sharedMesh, noise_type);
            }
            catch
            {
                if (noise_type.GenerateNoiseMap(noise))
                    noise_visualisation.GenerateMesh(meshFilter.sharedMesh, noise_type);
            }
        }
    }
}
