using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    [RequireComponent(typeof(GuiGenericValueHandler))]
    public class GuiPositionDisplayHandler : MonoBehaviour
    {
        [SerializeField] private Vector3 value = Vector3.zero;
        [SerializeField] private bool clampValuesToSimulationBox = true;

        // UI-References
        private GuiSubWindowHandler parentSubwindow;
        private PC_InputParser_Float_TMP xCoordinateInput;
        private PC_InputParser_Float_TMP yCoordinateInput;
        private PC_InputParser_Float_TMP zCoordinateInput;

        // Public members
        // If this is set, the text-inputs will track the objects position (Queried once every LateUpdate)
        public Transform affectedObject = null;
        // This Event is only triggered on text-field edits, not when the tracked/affected object moves
        public UnityEngine.Events.UnityEvent<Vector3> OnEndEdit;

        private void Awake() 
        {
            // Create UI elements by instanciating prefab
            var contentPanel = GetComponent<GuiGenericValueHandler>().GetEmptyContentPanel();
            var inputPrefab = Resources.Load("GuiPositionDisplayPrefab");
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

            xCoordinateInput?.onValueChangedFloat.AddListener((value) => OnCoordinateValueChanged(value, 0));
            yCoordinateInput?.onValueChangedFloat.AddListener((value) => OnCoordinateValueChanged(value, 1));
            zCoordinateInput?.onValueChangedFloat.AddListener((value) => OnCoordinateValueChanged(value, 2));

            SetValue(value);
        }

        private void OnCoordinateValueChanged(float coordValue, int dimension)
        {
            if (affectedObject != null)
            {
                value = affectedObject.transform.position;
            }

            value[dimension] = coordValue;
            if (affectedObject != null)
            {
                affectedObject.transform.position = value;
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
                xCoordinateInput.minimum = bounds.min.x;
                xCoordinateInput.maximum = bounds.max.x;
                yCoordinateInput.minimum = bounds.min.y;
                yCoordinateInput.maximum = bounds.max.y;
                zCoordinateInput.minimum = bounds.min.z;
                zCoordinateInput.maximum = bounds.max.z;
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
            if (affectedObject == null) return;
            var systemPos = affectedObject.position;
            // Note(MartinR): SetValue checks if the new value is different from the current textfield-value, so it
            //      doesn't cause any problems while the text-field is edited even if we call SetValue each frame
            xCoordinateInput.SetValue(systemPos.x);
            yCoordinateInput.SetValue(systemPos.y);
            zCoordinateInput.SetValue(systemPos.z);
        }

        public Vector3 GetValue()
        {
            if (affectedObject != null) return affectedObject.transform.position;
            return value;
        }

        public void SetValue(Vector3 newValue)
        {
            value = newValue;
            if (affectedObject != null)
            {
                affectedObject.transform.position = value;
            }

            // Note(MartinR): This check is required so UpdateInputBounds can be called from other gameobjects in Awake
            if (xCoordinateInput == null || yCoordinateInput == null || zCoordinateInput == null) return;

            xCoordinateInput.SetValue(value.x);
            yCoordinateInput.SetValue(value.y);
            zCoordinateInput.SetValue(value.z);
        }
    }
}
