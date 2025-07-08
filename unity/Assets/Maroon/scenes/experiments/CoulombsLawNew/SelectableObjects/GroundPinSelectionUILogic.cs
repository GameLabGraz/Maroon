using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class GroundPinSelectionUILogic : MonoBehaviour
    {
        [SerializeField] GUIVector3InputLogic uiPosition;
        [SerializeField] GUIBoolInputLogic uiApplyToVisualizationToggle;

        private void Start()
        {
            uiPosition.OnEndEdit.AddListener((Vector3 newPos) =>
            {
                var selected = SelectionSystem.Instance.GetSelectedObject();
                if (selected == null) return;
                selected.transform.position = newPos;
            });

            uiApplyToVisualizationToggle.OnValueChanged.AddListener((bool newValue) => 
            {
                var selected = SelectionSystem.Instance.GetSelectedObject();
                if (selected == null) return;
                var pin = SelectionSystem.Instance.GetSelectedObject().GetComponent<GroundPinLogic>();
                if (pin == null) return;
                pin.applyToVisualization = newValue;
            });

            var selected = SelectionSystem.Instance.GetSelectedObject();
            if (selected == null) return;
            var pin = SelectionSystem.Instance.GetSelectedObject().GetComponent<GroundPinLogic>();
            if (pin == null) return;
            uiApplyToVisualizationToggle.SetValue(pin.applyToVisualization);
            uiPosition.TrackTransform(pin.transform);
        }
    }
}
