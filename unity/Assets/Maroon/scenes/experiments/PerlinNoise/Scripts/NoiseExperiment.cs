using System;
using Maroon.Physics;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Maroon.UI;
using UnityEngine.Events;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public abstract class NoiseVisualisation : MonoBehaviour
    {
        public GameObject panel;
        public abstract void GenerateMesh(Mesh mesh);
        public abstract void UpdateMesh(Mesh mesh);

        public virtual int GetMaxSize() => 50;
    }

    public class NoiseExperiment : MonoBehaviour
    {
        [SerializeField] private NoiseVisualisation noise_visualisation;

        [SerializeField] private NoiseVisualisation[] noise_visualisations;

        [SerializeField] private bool dirty;

        [SerializeField, Range(0, 20), Header("Controls")]
        private float rotation_speed = 0;

        [SerializeField] private float mouse_sensitivity = 1;
        [SerializeField] private float mouse_wheel_sensitivity = 1;
        [SerializeField] private float rotaion_reset_speed = 0.08f;
        [SerializeField] private float scale_reset_speed = 0.1f;
        [SerializeField] private Vector2 scale_bounds = new Vector2(1, 5);
        [SerializeField] private Dropdown shader_type_dropdown;
        [SerializeField] private Slider size_slider;

        [SerializeField] private Shader[] shaders;

        [SerializeField, Header("Common Configuration")]
        private QuantityInt seed;

        [SerializeField] public QuantityInt size;
        [SerializeField] public QuantityFloat scale;
        [SerializeField] public QuantityFloat octaves;


        [Space(10)] [SerializeField, Range(0, 2)]
        float speed = 1;

        [Header("EditorControl")] [SerializeField]
        private bool force_refresh;

        [SerializeField] private bool animated;

        public float time { get; private set; }

        private MeshFilter meshFilter;
        private float rotation;
        private bool is_rotating;
        private Vector2 current_mouse_position;
        public static Noise Noise3D = new Noise(0);


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

        private void SetShader(int index)
        {
            if (!shaders.IsValidIndex(index))
                return;

            meshFilter.GetComponent<MeshRenderer>().material.shader = shaders[index];
        }

        public void OnSelectVisualisation(int index, string _)
        {
            if (!noise_visualisations.IsValidIndex(index))
                return;
            dirty = true;
            force_refresh = true;

            if (noise_visualisation && noise_visualisation.panel)
                noise_visualisation.panel.SetActive(false);

            noise_visualisation = noise_visualisations[index];

            if (noise_visualisation)
            {
                if (noise_visualisation.panel)
                    noise_visualisation.panel.SetActive(true);

                //keep same relative size value, the slider doesnt change but the number
                var relative_size = (float)size / size.maxValue;
                var max = noise_visualisation.GetMaxSize();
                size.maxValue = max;
                size.Value = (int)(relative_size * size.maxValue);
                size_slider.maxValue = max;
                size_slider.SetSliderValue(size.Value);
                this.EndFrame(() => size_slider.maxValue = max);
            }
        }


        public void SetDirty() => dirty = true;


        // Start is called before the first frame update
        void Start()
        {
            noise_visualisation.GenerateMesh(meshFilter.sharedMesh);
            SimulationController.Instance.StartSimulation();

#if UNITY_EDITOR
            animated = false;
            while (EditorApplication.update.GetInvocationList().Contains((Action)Update))
                EditorApplication.update -= Update;
#endif
        }


        // Update is called once per frame
        void Update()
        {
            HandleInput();

            if ((SimulationController.Instance && SimulationController.Instance.SimulationRunning) || animated)
            {
                if (!is_rotating)
                {
                    var t = meshFilter.transform;
                    var current_rotation = t.rotation;
                    var target_y = current_rotation.eulerAngles.y + Time.deltaTime * rotation_speed;
                    var target_rotation = Quaternion.Lerp(current_rotation, Quaternion.Euler(0, target_y, 0),
                        rotaion_reset_speed * Time.deltaTime);
                    target_rotation = Quaternion.Euler(target_rotation.eulerAngles.x, target_y,
                        target_rotation.eulerAngles.z);
                    t.rotation = target_rotation;

                    t.localScale = Vector3.Lerp(t.localScale, new Vector3(2f, 2f, 2f),
                        scale_reset_speed * Time.deltaTime);
                }

                time += Time.deltaTime * speed;
                //     dirty = true;
            }

            if (force_refresh)
            {
                noise_visualisation.GenerateMesh(meshFilter.sharedMesh);
                force_refresh = false;
                dirty = false;
                return;
            }

            if (!dirty)
                return;

            noise_visualisation.UpdateMesh(meshFilter.sharedMesh);
            dirty = false;
        }

        void HandleInput()
        {
            if (is_rotating)
            {
                var mouse_offset = (Vector2)Input.mousePosition - current_mouse_position;
                mouse_offset *= mouse_sensitivity;
                meshFilter.transform.Rotate(mouse_offset.y, -mouse_offset.x, 0, Space.World);
                current_mouse_position = Input.mousePosition;
            }


            var scale = meshFilter.transform.localScale.x;
            scale += Input.mouseScrollDelta.y * mouse_wheel_sensitivity;
            scale = scale.Clamp(scale_bounds.x, scale_bounds.y);
            meshFilter.transform.localScale = Vector3.one * scale;
        }

        private void SetSeed(int seed) => Noise3D.offset = seed * 1000;

        private void OnValidate()
        {
            noise_visualisations = GetComponents<NoiseVisualisation>();
            if (noise_visualisations.Any() && noise_visualisation == null)
                noise_visualisation = noise_visualisations[0];

#if UNITY_EDITOR
            if (animated)
            {
                if (!EditorApplication.update.GetInvocationList().Contains((Action)Update))
                    EditorApplication.update += Update;
            }
            else
            {
                while (EditorApplication.update.GetInvocationList().Contains((Action)Update))
                    EditorApplication.update -= Update;
            }
#endif

            void InitQuantityListener<T>(UnityEvent<T> onValueChanged)
            {
                onValueChanged.RemoveAllListeners();
                onValueChanged.AddListener(_ => SetDirty());
            }

            InitQuantityListener(seed.onValueChanged);
            InitQuantityListener(size.onValueChanged);
            InitQuantityListener(scale.onValueChanged);
            InitQuantityListener(octaves.onValueChanged);
            InitQuantityListener(shader_type_dropdown.onValueChanged);

            shader_type_dropdown.onValueChanged.AddListener(SetShader);
            seed.onValueChanged.AddListener(SetSeed);

            if (!meshFilter)
                meshFilter = GetComponentInChildren<MeshFilter>();
            if (!meshFilter)
                return;

            if (force_refresh)
            {
                noise_visualisation.GenerateMesh(meshFilter.sharedMesh);
                force_refresh = false;
                return;
            }

            try
            {
                noise_visualisation.UpdateMesh(meshFilter.sharedMesh);
            }
            catch
            {
                noise_visualisation.GenerateMesh(meshFilter.sharedMesh);
            }
        }

        void OnMouseDown()
        {
            is_rotating = true;
            current_mouse_position = Input.mousePosition;
        }

        void OnMouseUp()
        {
            is_rotating = false;
        }
    }
}