using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class MeasurementProbeSelectionUILogic : MonoBehaviour
    {
        [SerializeField] GUIVector3InputLogic uiPosition;
        [SerializeField] GUIVector3InputLogic uiVectorFieldValue;
        [SerializeField] GUIFloatInputLogic uiVectorFieldMagnitude;
        [SerializeField] GUIFloatInputLogic uiPotential;

        private void Start()
        {
            uiPosition.OnEndEdit.AddListener((Vector3 newPos) =>
            {
                var selected = SelectionSystem.Instance.GetSelectedObject();
                if (selected == null) return;
                selected.transform.position = newPos;
            });

            var selected = SelectionSystem.Instance.GetSelectedObject();
            if (selected == null) return;
            var terminal = SelectionSystem.Instance.GetSelectedObject().GetComponent<MeasurementProbeLogic>();
            if (terminal == null) return;
            uiPosition.TrackTransform(terminal.transform);
        }

        private void LateUpdate()
        {
            var selected = SelectionSystem.Instance.GetSelectedObject();
            if (selected == null) return;
            var probe = selected.GetComponent<MeasurementProbeLogic>();
            if (probe == null) return;
            var pos = probe.transform.position;

            var efield = ElectricField.Instance;

            // Update UI-Values
            var fieldValue = efield.GetFieldValue(pos, false);
            uiVectorFieldValue.SetValue(fieldValue);
            uiVectorFieldMagnitude.SetValue(fieldValue.magnitude);

            var potential = efield.GetPotential(pos, false);
            float groundPotential = 0.0f;
            if (probe.groundPin != null && probe.groundPin.isActiveAndEnabled)
            {
                groundPotential = efield.GetPotential(probe.groundPin.transform.position, false);
            }
            uiPotential.SetValue(potential - groundPotential);
        }
    }
}
