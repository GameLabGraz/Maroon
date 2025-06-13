using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class MovementGizmoArrow : MonoBehaviour
    {
        public int dimension = 0; // Dimension is mapped [X, Y, Z] <-> [0, 1, 2]
        [SerializeField] MovementGizmoController _gizmoController;

        private void OnMouseDown() { _gizmoController.OnArrowMouseDown(dimension); }
        private void OnMouseDrag() { _gizmoController.OnArrowMouseDrag(dimension); }
        private void OnMouseUp()   { _gizmoController.OnArrowMouseUp(dimension); }
    }
}
