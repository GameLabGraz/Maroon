﻿using System;
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
    }

    public class NoiseExperiment : MonoBehaviour
    {
        [SerializeField] private NoiseVisualisation noise_visualisation;

        [SerializeField] private NoiseVisualisation[] noise_visualisations;

        [SerializeField] private bool dirty;
        [SerializeField] private bool animated;
        [SerializeField, Range(0, 20)] private float rotation_speed = 0;
        [SerializeField] private float mouse_sensitivity = 1;
        [SerializeField] private Dropdown shader_type_dropdown;

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
        private bool is_rotating;
        private Vector2 current_mouse_position;
        public static readonly Noise Noise3D = new Noise(0);


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
            if(shader_type_dropdown.value == 0) return Color.gray;
            if (value > middle)
                return Color.Lerp(colors.middle, colors.top, value.Map(middle, top));
            return Color.Lerp(colors.bottom, colors.middle, value.Map(bottom, middle));
        }

        public void ResetRotation() => meshFilter.transform.rotation = Quaternion.identity;

        public void OnSelectVisualisation(int index, string _)
        {
            if (!noise_visualisations.IsValidIndex(index))
                return;
            dirty = true;
            force_refresh = true;

            if (noise_visualisation && noise_visualisation.panel)
                noise_visualisation.panel.SetActive(false);

            noise_visualisation = noise_visualisations[index];

            if (noise_visualisation && noise_visualisation.panel)
                noise_visualisation.panel.SetActive(true);
        }


        public void SetDirty() => dirty = true;


        // Start is called before the first frame update
        void Start()
        {
            noise_visualisation.GenerateMesh(meshFilter.sharedMesh);
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
            HandleInput();
            
            if ((SimulationController.Instance && SimulationController.Instance.SimulationRunning) || animated)
            {
                meshFilter.transform.Rotate(Vector3.up, Time.deltaTime * rotation_speed, Space.World);
                time += Time.deltaTime * speed;
                dirty = true;
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
        }


        private void OnValidate()
        {
            noise_visualisations = GetComponents<NoiseVisualisation>();
            if (noise_visualisations.Any() && noise_visualisation == null)
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
