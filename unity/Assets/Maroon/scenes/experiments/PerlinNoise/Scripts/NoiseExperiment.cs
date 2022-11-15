using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using GEAR.Localization;
using Maroon.Physics;
using Maroon.reusableGui.Experiment.Scripts.Runtime;
using Maroon.scenes.experiments.PerlinNoise.Scripts.NoiseVisualisations;
using Maroon.UI;
using UnityEngine.Events;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public abstract class NoiseVisualisation : MonoBehaviour
    {
        public GameObject[] panel;
        public abstract void GenerateMesh(Mesh mesh);
        public abstract void UpdateMesh(Mesh mesh);

        public virtual int GetMaxSize() => 50;

        public void SetPanelActive(bool active)
        {
            foreach (var p in panel)
            {
                if(p) p.SetActive(active);
            }
        }
    }

    public class NoiseExperiment : MonoBehaviour
    {
        [SerializeField] private NoiseVisualisation[] noise_visualisations;


        [SerializeField, Header("Controls")] private float mouse_sensitivity = 1;
        [SerializeField] private float mouse_wheel_sensitivity = 1;
        [SerializeField] private float rotaion_reset_speed = 0.08f;
        [SerializeField] private float scale_reset_speed = 0.1f;
        [SerializeField] private Vector2 scale_bounds = new Vector2(1, 5);
        [SerializeField] private RadioButton shader_type_dropdown;
        [SerializeField] private QuantityPropertyView size_property_view;

        [SerializeField] private Shader[] shaders;


        [Header("EditorControl")] [SerializeField]
        private bool force_refresh;

        [SerializeField] private bool animated;

        private Vector2 _current_mouse_position;
        private DialogueManager _dialogue_manager;


        [SerializeField] protected NoiseVisualisation noise_visualisation;
        [SerializeField, Range(0, 20)] protected float rotation_speed = 0;

        [SerializeField, Header("Common Configuration")]
        public QuantityInt seed;

        [SerializeField] public QuantityInt size;
        [SerializeField] public QuantityFloat scale;
        [SerializeField] public QuantityFloat octaves;

        [Space(10)] [SerializeField, Range(0, 2)]
        protected float speed = 1;

        [SerializeField] protected bool dirty;
        [SerializeField] protected bool dirty_immediate;
        [SerializeField] protected TimeSpan dirty_refresh_rate = new TimeSpan(0, 0, 0, 0, 50);
        private (Color32 top, Color32 middle, Color32 bottom) colors = (Color.gray, Color.yellow, Color.cyan);

        public DateTime last_update;

        protected MeshFilter meshFilter;
        protected bool is_rotating;
        public static readonly Noise noise = new Noise(0);
        public float time { get; protected set; }

        public UnityEvent onUpdateMesh;


        private static NoiseExperiment _instance;

        public static NoiseExperiment Instance
        {
            get
            {
                if (!_instance)
                    _instance = FindObjectOfType<NoiseExperiment>();
                return _instance;
            }
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
            SetDirtyImmediate();
            force_refresh = true;

            if (noise_visualisation)
                noise_visualisation.SetPanelActive(false);

            noise_visualisation = noise_visualisations[index];

            if (noise_visualisation)
            {
                noise_visualisation.SetPanelActive(true);

                //keep same relative size value, the slider doesnt change but the number
                var relative_size = (float)size / size.maxValue;
                var max = noise_visualisation.GetMaxSize();
                size.maxValue = max;
                size.Value = (int)(relative_size * size.maxValue);
                var size_slider = size_property_view.GetComponentInChildren<Slider>();
                size_slider.maxValue = max;
                size_slider.SetSliderValue(size.Value);
            }
        }

        public void SetDirty() => dirty = true;
        public void SetDirtyImmediate() => dirty_immediate = true;


        private void Awake()
        {
            foreach (var visualisation in noise_visualisations)
            {
                visualisation.SetPanelActive(false);
            }
        }

        public void GetNoise(ref float[] data, float width, float y)
        {
            var factor = width / data.Length;
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = noise.GetNoise2D((i - data.Length / 2f) * scale * factor, y, octaves);
            }
        }

        public void GetNoiseSizeDependent(ref List<float> data, float width, float y)
        {
            if (data.Count > size)
                data.RemoveRange(size, data.Count - size);
            var factor = width / (size - 1f);
            for (int i = 0; i < data.Count; i++)
            {
                data[i] = noise.GetNoise2D((i - (size - 1) / 2f) * scale * factor, y, octaves);
            }

            for (int i = data.Count; i < size; i++)
            {
                data.Add(noise.GetNoise2D((i - (size - 1) / 2f) * scale * factor, y, octaves));
            }
        }

        private void Start()
        {
            DisplayMessageByKey("EnterNoiseExperiment");

            Init();

            if (!meshFilter)
                meshFilter = GetComponentInChildren<MeshFilter>();
            noise_visualisation.GenerateMesh(meshFilter.sharedMesh);

            SimulationController.Instance.StartSimulation();


#if UNITY_EDITOR
            animated = false;
            while (EditorApplication.update.GetInvocationList().Contains((Action)Update))
                EditorApplication.update -= Update;
#endif
        }


        private void Init()
        {
            void InitQuantityListener<T>(UnityEvent<T> onValueChanged)
            {
                onValueChanged.RemoveAllListeners();
                onValueChanged.AddListener(_ => SetDirty());
            }

            InitQuantityListener(seed.onValueChanged);
            InitQuantityListener(size.onValueChanged);
            InitQuantityListener(scale.onValueChanged);
            InitQuantityListener(octaves.onValueChanged);

            shader_type_dropdown.OnSelect.RemoveAllListeners();
            shader_type_dropdown.OnSelect.AddListener((index, _) =>
            {
                SetShader(index);
                SetDirty();
            });
            seed.onValueChanged.AddListener(SetSeed);

            if (!meshFilter)
                meshFilter = GetComponentInChildren<MeshFilter>();
            if (!meshFilter)
                return;

            last_update = DateTime.Now;
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

        // Update is called once per frame
        private void Update()
        {
            if (!meshFilter)
                meshFilter = GetComponentInChildren<MeshFilter>();
            if (!meshFilter) return;

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
                onUpdateMesh.Invoke();
                force_refresh = false;
                dirty = false;
                dirty_immediate = false;
                last_update = DateTime.Now;
                return;
            }

            if (!dirty_immediate && (!dirty || last_update + dirty_refresh_rate > DateTime.Now))
                return;
            last_update = DateTime.Now;
            noise_visualisation.UpdateMesh(meshFilter.sharedMesh);
            onUpdateMesh.Invoke();
            dirty = false;
            dirty_immediate = false;
        }

        void HandleInput()
        {
            if (is_rotating)
            {
                var mouse_offset = (Vector2)Input.mousePosition - _current_mouse_position;
                mouse_offset *= mouse_sensitivity;
                meshFilter.transform.Rotate(mouse_offset.y, -mouse_offset.x, 0, Space.World);
                _current_mouse_position = Input.mousePosition;
            }


            var scale = meshFilter.transform.localScale.x;
            scale += Input.mouseScrollDelta.y * mouse_wheel_sensitivity;
            scale = scale.Clamp(scale_bounds.x, scale_bounds.y);
            meshFilter.transform.localScale = Vector3.one * scale;
        }


        public Color GetVertexColor(float value, float bottom, float middle, float top)
        {
            if (value > middle)
                return Color.Lerp(colors.middle, colors.top, value.Map(middle, top));
            return Color.Lerp(colors.bottom, colors.middle, value.Map(bottom, middle));
        }

        private void SetSeed(int seed) => noise.offset = seed * 1000;

        private void DisplayMessageByKey(string key)
        {
            if (_dialogue_manager == null)
                _dialogue_manager = FindObjectOfType<DialogueManager>();

            if (_dialogue_manager == null)
                return;

            var message = LanguageManager.Instance.GetString(key);

            _dialogue_manager.ShowMessage(message);
        }

        void OnMouseDown()
        {
            is_rotating = true;
            _current_mouse_position = Input.mousePosition;
        }

        void OnMouseUp()
        {
            is_rotating = false;
        }

        public float GetThreshold()
        {
            switch (noise_visualisation)
            {
                case NoiseVisualisationVoxel visualisationVoxel:
                    return -visualisationVoxel.threshold + 0.5f;
                case NoiseVisualisationMarchingCubes marchingCubes:
                    return -marchingCubes.threshold + 0.5f;
                default:
                    return 0;
            }
        }
    }
}