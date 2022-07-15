using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Maroon.UI;
using TMPro;
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

    public class NoiseExperiment : NoiseExperimentBase
    {
        [SerializeField] private NoiseVisualisation[] noise_visualisations;


        [SerializeField, Header("Controls")] private float mouse_sensitivity = 1;
        [SerializeField] private float mouse_wheel_sensitivity = 1;
        [SerializeField] private float rotaion_reset_speed = 0.08f;
        [SerializeField] private float scale_reset_speed = 0.1f;
        [SerializeField] private Vector2 scale_bounds = new Vector2(1, 5);
        [SerializeField] private Dropdown shader_type_dropdown;
        [SerializeField] private QuantityPropertyView size_property_view;

        [SerializeField] private TextMeshProUGUI debug_text;

        [SerializeField] private Shader[] shaders;


        [Header("EditorControl")] [SerializeField]
        private bool force_refresh;

        [SerializeField] private bool animated;

        private Vector2 current_mouse_position;


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
                var size_slider = size_property_view.GetComponentInChildren<Slider>();
                size_slider.maxValue = max;
                size_slider.SetSliderValue(size.Value);
            }
        }

        private void Awake()
        {
            foreach (var visualisation in noise_visualisations)
            {
                if (visualisation.panel)
                    visualisation.panel.SetActive(false);
            }
        }

        new void Start()
        {
            Init();
            base.Start();

            SimulationController.Instance.StartSimulation();


#if UNITY_EDITOR
            animated = false;
            while (EditorApplication.update.GetInvocationList().Contains((Action)HandleUpdate))
                EditorApplication.update -= HandleUpdate;
#endif
        }

        // Update is called once per frame
        protected override void HandleUpdate()
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
            return;
            //Init();
            noise_visualisations = GetComponents<NoiseVisualisation>();
            if (noise_visualisations.Any() && noise_visualisation == null)
                noise_visualisation = noise_visualisations[0];

#if UNITY_EDITOR
            if (animated)
            {
                if (!EditorApplication.update.GetInvocationList().Contains((Action)HandleUpdate))
                    EditorApplication.update += HandleUpdate;
            }
            else
            {
                while (EditorApplication.update.GetInvocationList().Contains((Action)HandleUpdate))
                    EditorApplication.update -= HandleUpdate;
            }
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