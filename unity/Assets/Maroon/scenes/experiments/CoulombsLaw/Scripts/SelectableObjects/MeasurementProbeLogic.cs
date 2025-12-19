using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class MeasurementProbeLogic : MonoBehaviour
    {
        [SerializeField] private GuiIconTo3DObjectDrag dragIcon;
        public GroundPinLogic groundPin;

        private void Awake()
        {
            GetComponent<SelectableObject>().OnDraggedOutOfBounds.AddListener((SelectableObject unused) => {
                SelectionSystem.SetSelectedObject(null);
                gameObject.SetActive(false);
            });

            dragIcon.OnDragFinished.AddListener((Vector3 pos) => {
                gameObject.SetActive(true);
                transform.position = pos;
            });

            gameObject.SetActive(false);
        }
    }
}
