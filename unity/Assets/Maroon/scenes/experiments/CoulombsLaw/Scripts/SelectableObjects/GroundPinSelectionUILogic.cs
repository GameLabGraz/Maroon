using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class GroundPinSelectionUILogic : MonoBehaviour
    {
        [SerializeField] GUIVector3InputLogic uiPosition;

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
            var pin = SelectionSystem.Instance.GetSelectedObject().GetComponent<GroundPinLogic>();
            if (pin == null) return;
            uiPosition.TrackTransform(pin.transform);
        }
    }
}
