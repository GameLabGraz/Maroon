using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class MultimeterController : MonoBehaviour
    {
        [SerializeField] private GuiIconTo3DObjectDrag uiDragIcon;

        public MultimeterTerminal positiveTerminal;
        public MultimeterTerminal negativeTerminal;

        void Start()
        {
            uiDragIcon.OnDragFinished.AddListener((Vector3 position) =>
            {
                SelectionSystem.Instance.SetSelectedObject(null);
                positiveTerminal.gameObject.SetActive(true);
                negativeTerminal.gameObject.SetActive(true);
                positiveTerminal.transform.position = position + 0.1f * Vector3.left;
                negativeTerminal.transform.position = position + 0.1f * Vector3.right;
            });
        }
    }
}
