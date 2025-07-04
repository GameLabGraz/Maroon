using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class MultimeterTerminal : MonoBehaviour
    {
        public MultimeterController multimeterController;
        public bool isPositiveTerminal;

        private void Start()
        {
            GetComponent<SelectableObject>().OnDraggedOutOfBounds.AddListener((SelectableObject unused) => {
                SelectionSystem.SetSelectedObject(null);
                multimeterController.positiveTerminal.gameObject.SetActive(false);
                multimeterController.negativeTerminal.gameObject.SetActive(false);
            });
        }
    }
}
