using System;
using System.Linq;
using GEAR.Localization;
using Maroon.reusableGui.Experiment.Scripts.Runtime;
using Maroon.UI;
using UnityEditor;
using UnityEngine;
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
                if (p)
                    p.SetActive(active);
        }
    }

    public class NoiseExperiment : NoiseExperimentBase
    {
        [SerializeField] private NoiseVisualisation[] noise_visualisations;
        [SerializeField] private float mouse_sensitivity = 1;
        [SerializeField] private float mouse_wheel_sensitivity = 1;
        [SerializeField] private float rotaion_reset_speed = 0.08f;
        [SerializeField] private float scale_reset_speed = 0.1f;
        [SerializeField] private bool force_refresh;
        [SerializeField] private bool animated;
        [SerializeField] private Vector2 scale_bounds = new Vector2(1, 5);
        [SerializeField] private RadioButton shader_type_selection;
        [SerializeField] private QuantityPropertyView size_property_view;
        [SerializeField] private Shader[] shaders;
        public UnityEvent onUpdateMesh;

        private Vector2 _current_mouse_position;
        private DialogueManager _dialogue_manager;
        private bool _is_rotating;
        private MeshFilter _mesh_filter;

        private void Awake()
        {
            foreach (var visualisation in noise_visualisations) visualisation.SetPanelActive(false);
        }

        private new void Start()
        {
            DisplayMessageByKey("EnterNoiseExperiment");

            Init();

            if (!_mesh_filter)
                _mesh_filter = GetComponentInChildren<MeshFilter>();
            noise_visualisation.GenerateMesh(_mesh_filter.sharedMesh);

            SimulationController.Instance.StartSimulation();


#if UNITY_EDITOR
            animated = false;
            while (EditorApplication.update.GetInvocationList().Contains((Action)Update))
                EditorApplication.update -= Update;
#endif
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_mesh_filter)
                _mesh_filter = GetComponentInChildren<MeshFilter>();
            if (!_mesh_filter) return;

            HandleInput();

            if ((SimulationController.Instance && SimulationController.Instance.SimulationRunning) || animated)
            {
                if (!_is_rotating)
                {
                    var t = _mesh_filter.transform;
                    var currentRotation = t.rotation;
                    var targetY = currentRotation.eulerAngles.y + Time.deltaTime * rotation_speed;
                    var targetRotation = Quaternion.Lerp(currentRotation, Quaternion.Euler(0, targetY, 0),
                        rotaion_reset_speed * Time.deltaTime);
                    targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetY,
                        targetRotation.eulerAngles.z);
                    t.rotation = targetRotation;

                    t.localScale = Vector3.Lerp(t.localScale, new Vector3(2f, 2f, 2f),
                        scale_reset_speed * Time.deltaTime);
                }
            }

            if (force_refresh)
            {
                noise_visualisation.GenerateMesh(_mesh_filter.sharedMesh);
                onUpdateMesh.Invoke();
                force_refresh = false;
                dirty = false;
                dirtyImmediate = false;
                lastUpdate = DateTime.Now;
                return;
            }

            if (!dirtyImmediate && (!dirty || lastUpdate + dirtyRefreshRate > DateTime.Now))
                return;
            lastUpdate = DateTime.Now;
            noise_visualisation.UpdateMesh(_mesh_filter.sharedMesh);
            onUpdateMesh.Invoke();
            dirty = false;
            dirtyImmediate = false;
        }

        private void OnMouseDown()
        {
            _is_rotating = true;
            _current_mouse_position = Input.mousePosition;
        }

        private void OnMouseUp() => _is_rotating = false;

        private void SetShader(int index)
        {
            if (!shaders.IsValidIndex(index))
                return;

            _mesh_filter.GetComponent<MeshRenderer>().material.shader = shaders[index];
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
            if (!noise_visualisation) 
                return;
            
            noise_visualisation.SetPanelActive(true);

            //keep same relative size value, the slider doesnt change but the number
            var relativeSize = (float)size / size.maxValue;
            var max = noise_visualisation.GetMaxSize();
            size.maxValue = max;
            size.Value = (int)(relativeSize * size.maxValue);
            var sizeSlider = size_property_view.GetComponentInChildren<Slider>();
            sizeSlider.maxValue = max;
            sizeSlider.SetSliderValue(size.Value);
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

            shader_type_selection.onSelect.RemoveAllListeners();
            shader_type_selection.onSelect.AddListener((index, _) =>
            {
                SetShader(index);
                SetDirty();
            });
            seed.onValueChanged.AddListener(SetSeed);

            if (!_mesh_filter)
                _mesh_filter = GetComponentInChildren<MeshFilter>();
            if (!_mesh_filter)
                return;

            lastUpdate = DateTime.Now;
            if (force_refresh)
            {
                noise_visualisation.GenerateMesh(_mesh_filter.sharedMesh);
                force_refresh = false;
                return;
            }

            try
            {
                noise_visualisation.UpdateMesh(_mesh_filter.sharedMesh);
            }
            catch
            {
                noise_visualisation.GenerateMesh(_mesh_filter.sharedMesh);
            }
        }

        private void HandleInput()
        {
            if (_is_rotating)
            {
                var mouseOffset = (Vector2)Input.mousePosition - _current_mouse_position;
                mouseOffset *= mouse_sensitivity;
                _mesh_filter.transform.Rotate(mouseOffset.y, -mouseOffset.x, 0, Space.World);
                _current_mouse_position = Input.mousePosition;
            }


            var localScale = _mesh_filter.transform.localScale.x;
            localScale += Input.mouseScrollDelta.y * mouse_wheel_sensitivity;
            localScale = localScale.Clamp(scale_bounds.x, scale_bounds.y);
            _mesh_filter.transform.localScale = Vector3.one * localScale;
        }

        private static void SetSeed(int newSeed) => noise.offset = newSeed * 1000;

        private void DisplayMessageByKey(string key)
        {
            if (_dialogue_manager == null)
                _dialogue_manager = FindObjectOfType<DialogueManager>();

            if (_dialogue_manager == null)
                return;

            var message = LanguageManager.Instance.GetString(key);

            _dialogue_manager.ShowMessage(message);
        }

    }
}