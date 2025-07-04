using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    [ExecuteAlways]
    [RequireComponent(typeof(GuiGenericValueHandler))]
    public class GuiVector3InputHandler : MonoBehaviour
    {
        [SerializeField] private Vector3 value = Vector3.zero;
        [SerializeField] private bool isInteractable = true;
        [SerializeField] private bool clampValuesToSimulationBox = true;
        [SerializeField] private bool displayRelativeToSimulationBox = false;

        // UI-References
        private GuiSubWindowHandler parentSubwindow;
        private PC_InputParser_Float_TMP xCoordinateInput;
        private PC_InputParser_Float_TMP yCoordinateInput;
        private PC_InputParser_Float_TMP zCoordinateInput;

        // Public members
        // If this is set, the text-inputs will track the objects position (Queried once every LateUpdate)
        private Transform _trackedTransform = null;
        // This Event is only triggered on text-field edits, not when the tracked/affected object moves
        public UnityEngine.Events.UnityEvent<Vector3> OnEndEdit;

        private void Awake() 
        {
            // Create UI elements by instanciating prefab
            var contentPanel = GetComponent<GuiGenericValueHandler>().GetEmptyContentPanel();
            var inputPrefab = Resources.Load("GuiVector3InputPrefab");
            var inputObject = (GameObject) GameObject.Instantiate(inputPrefab, contentPanel.transform);

            parentSubwindow = GuiSubWindowHandler.FindParentSubWindow(gameObject);
            xCoordinateInput = GuiSubWindowHandler.FindChildObjectByNameRecursive(
                GuiSubWindowHandler.FindChildObjectByNameRecursive(contentPanel, "XPanel"),
                "ValueInputField").GetComponent<PC_InputParser_Float_TMP>();
            yCoordinateInput = GuiSubWindowHandler.FindChildObjectByNameRecursive(
                GuiSubWindowHandler.FindChildObjectByNameRecursive(contentPanel, "YPanel"),
                "ValueInputField").GetComponent<PC_InputParser_Float_TMP>();
            zCoordinateInput = GuiSubWindowHandler.FindChildObjectByNameRecursive(
                GuiSubWindowHandler.FindChildObjectByNameRecursive(contentPanel, "ZPanel"),
                "ValueInputField").GetComponent<PC_InputParser_Float_TMP>();

            if (xCoordinateInput == null || yCoordinateInput == null || zCoordinateInput == null)
            {
                Debug.LogWarning("With correct use of prefab all child objects should be found");
                return;
            }
        }

        private void Start()
        {
            // Register callbacks
            SimulationBox.Instance.OnBoundsChanged.AddListener((Bounds _unused) => { UpdateInputBounds(); });
            UpdateInputBounds();

            SetValue(value);

            if (xCoordinateInput == null || yCoordinateInput == null || zCoordinateInput == null) return;
            xCoordinateInput.GetInputField().readOnly = !isInteractable;
            yCoordinateInput.GetInputField().readOnly = !isInteractable;
            zCoordinateInput.GetInputField().readOnly = !isInteractable;

            xCoordinateInput?.onValueChangedFloat.AddListener((value) => OnCoordinateValueChanged(value, 0));
            yCoordinateInput?.onValueChangedFloat.AddListener((value) => OnCoordinateValueChanged(value, 1));
            zCoordinateInput?.onValueChangedFloat.AddListener((value) => OnCoordinateValueChanged(value, 2));
        }

        private void OnCoordinateValueChanged(float coordValue, int dimension)
        {
            if (_trackedTransform != null)
            {
                value = _trackedTransform.transform.position;
            }

            Vector3 offset = displayRelativeToSimulationBox ? SimulationBox.Instance.Bounds.center : Vector3.zero;
            value[dimension] = coordValue + offset[dimension];
            if (_trackedTransform != null)
            {
                _trackedTransform.transform.position = value;
            }

            OnEndEdit.Invoke(value);
        }

        private void UpdateInputBounds()
        {
            // Note(MartinR): This check is required so UpdateInputBounds can be called from other gameobjects in Awake
            if (xCoordinateInput == null || yCoordinateInput == null || zCoordinateInput == null) return;

            var bounds = SimulationBox.Instance.Bounds;
            if (clampValuesToSimulationBox)
            {
                Vector3 offset = displayRelativeToSimulationBox ? SimulationBox.Instance.Bounds.center : Vector3.zero;
                xCoordinateInput.minimum = bounds.min.x - offset.x;
                xCoordinateInput.maximum = bounds.max.x - offset.x;
                yCoordinateInput.minimum = bounds.min.y - offset.y;
                yCoordinateInput.maximum = bounds.max.y - offset.y;
                zCoordinateInput.minimum = bounds.min.z - offset.z;
                zCoordinateInput.maximum = bounds.max.z - offset.z;
            }
            else
            {
                xCoordinateInput.minimum = float.NegativeInfinity;
                yCoordinateInput.minimum = float.NegativeInfinity;
                zCoordinateInput.minimum = float.NegativeInfinity;
                xCoordinateInput.maximum = float.PositiveInfinity;
                yCoordinateInput.maximum = float.PositiveInfinity;
                zCoordinateInput.maximum = float.PositiveInfinity;
            }
        }

        private void LateUpdate()
        {
            // Update text fields
            if (_trackedTransform == null) return;
            SetValue(_trackedTransform.position);
        }

        public Vector3 GetValue()
        {
            if (_trackedTransform != null) return _trackedTransform.transform.position;
            return value;
        }

        public void SetValue(Vector3 newValue)
        {
            value = newValue;
            if (_trackedTransform != null)
            {
                _trackedTransform.transform.position = value;
            }

            // Note(MartinR): This check is required so UpdateInputBounds can be called from other gameobjects in Awake
            if (xCoordinateInput == null || yCoordinateInput == null || zCoordinateInput == null) return;

            Vector3 displayPos = value;
            if (displayRelativeToSimulationBox)
            {
                displayPos -= SimulationBox.Instance.Bounds.center;
            }
            xCoordinateInput.SetValue(displayPos.x);
            yCoordinateInput.SetValue(displayPos.y);
            zCoordinateInput.SetValue(displayPos.z);
        }

        public void TrackTransform(Transform transform)
        {
            SetValue(transform.position);
            _trackedTransform = transform;
        }
    }
}
