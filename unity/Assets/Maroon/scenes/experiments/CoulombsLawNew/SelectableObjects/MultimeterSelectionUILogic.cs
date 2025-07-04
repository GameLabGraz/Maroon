using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class MultimeterSelectionUILogic : MonoBehaviour
    {
        [SerializeField] GUIVector3InputLogic uiPosition;
        [SerializeField] GUIVector3InputLogic uiVectorFieldValue;
        [SerializeField] GUIFloatInputLogic uiVectorFieldMagnitude;
        [SerializeField] GUIFloatInputLogic uiPotentialToGround;
        [SerializeField] GUIFloatInputLogic uiDistanceBetween;
        [SerializeField] GUIFloatInputLogic uiVoltageAcross;

        private void Start()
        {
            uiPosition.OnEndEdit.AddListener((Vector3 newPos) =>
            {
                var selected = SelectionSystem.Instance.GetSelectedObject();
                if (selected == null) return;
                var terminal = SelectionSystem.Instance.GetSelectedObject().GetComponent<MultimeterTerminal>();
                if (terminal == null) return;
                terminal.transform.position = newPos;
            });
        }

        private void LateUpdate()
        {
            var selected = SelectionSystem.Instance.GetSelectedObject();
            if (selected == null) return;
            var terminal = SelectionSystem.Instance.GetSelectedObject().GetComponent<MultimeterTerminal>();
            if (terminal == null) return;

            var positiveTerminal = terminal.multimeterController.positiveTerminal;
            var negativeTerminal = terminal.multimeterController.negativeTerminal;
            var otherTerminal = terminal.isPositiveTerminal ? negativeTerminal : positiveTerminal;

            var efield = ElectricField.Instance;

            // Update UI-Values
            uiPosition.SetValue(terminal.transform.position);

            var fieldValue = efield.GetFieldValue(terminal.transform.position, false);
            uiVectorFieldValue.SetValue(fieldValue);
            uiVectorFieldMagnitude.SetValue(fieldValue.magnitude);

            var potential = efield.GetPotential(terminal.transform.position, false);
            uiPotentialToGround.SetValue(potential);

            uiDistanceBetween.SetValue((terminal.transform.position - otherTerminal.transform.position).magnitude);
            var otherPotential = efield.GetPotential(otherTerminal.transform.position, false);
            uiVoltageAcross.SetValue((potential - otherPotential) * (terminal.isPositiveTerminal ? 1 : -1));
        }
    }
}
