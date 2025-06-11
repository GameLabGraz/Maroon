using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class GuiPositionDisplayHandler : MonoBehaviour
    {
        public Transform affectedObject = null;

        [SerializeField] private PC_InputParser_Float_TMP xCoordinateInput;
        [SerializeField] private PC_InputParser_Float_TMP yCoordinateInput;
        [SerializeField] private PC_InputParser_Float_TMP zCoordinateInput;

        private void OnCoordinateValueChanged(float value, int dimension)
        {
            if (affectedObject == null) return;
            var newPos = affectedObject.position;
            newPos[dimension] = value;
            affectedObject.transform.position = newPos;
        }

        private void UpdateInputBounds()
        {
            var bounds = SimulationBox.Instance.Bounds;
            xCoordinateInput.minimum = bounds.min.x;
            xCoordinateInput.maximum = bounds.max.x;
            yCoordinateInput.minimum = bounds.min.y;
            yCoordinateInput.maximum = bounds.max.y;
            zCoordinateInput.minimum = bounds.min.z;
            zCoordinateInput.maximum = bounds.max.z;
        }

        private void Awake()
        {
            SimulationBox.Instance.OnBoundsChanged.AddListener((Bounds _unused) => { UpdateInputBounds(); });
            UpdateInputBounds();

            // Register UI callbacks
            xCoordinateInput?.onValueChangedFloat.AddListener((value) => OnCoordinateValueChanged(value, 0));
            yCoordinateInput?.onValueChangedFloat.AddListener((value) => OnCoordinateValueChanged(value, 1));
            zCoordinateInput?.onValueChangedFloat.AddListener((value) => OnCoordinateValueChanged(value, 2));
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
    }
}
