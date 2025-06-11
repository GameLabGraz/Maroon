using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class CoulombsLawUILogic : MonoBehaviour
    {
        [SerializeField] private GuiIconTo3DObjectDrag dragIconParticle;
        [SerializeField] private PointCharge prefabPointCharge;
        [SerializeField] private Transform parentForNewObjects;

        [SerializeField] private GuiFloatInputHandler pointChargeChargeInput;
        [SerializeField] private TMPro.TMP_Text selectionLabel;
        [SerializeField] private UnityEngine.UI.Button deleteButton;
        [SerializeField] private GuiPositionDisplayHandler positionDisplay;

        private void SetUISelectionEmpty()
        {
            selectionLabel.text = "Selection: Empty";
            pointChargeChargeInput.gameObject.SetActive(false);
            deleteButton.gameObject.SetActive(false);
            positionDisplay.gameObject.SetActive(false);
        }

        private void Awake()
        {
            pointChargeChargeInput.SetMinMax(-PointCharge.MAX_ABSOLUTE_CHARGE * 1e6f, PointCharge.MAX_ABSOLUTE_CHARGE * 1e6f);

            dragIconParticle.OnDragFinished.AddListener((Vector3 pos) =>
            {
                var newParticle = GameObject.Instantiate(prefabPointCharge, pos, Quaternion.identity, parentForNewObjects);
            });

            SelectionSystem.Instance.OnSelectionChanged.AddListener((SelectableObject selectable) =>
            {
                SetUISelectionEmpty();
                if (selectable == null) return;
                deleteButton.gameObject.SetActive(true);

                // Handle selection for different object types
                var pointCharge = selectable.GetComponent<PointCharge>();
                if (pointCharge != null)
                {
                    selectionLabel.text = "Selection: PointCharge";
                    pointChargeChargeInput.gameObject.SetActive(true);
                    pointChargeChargeInput.SetValue(pointCharge.GetCharge() * 1e6f); // In MicroCoulomb
                    positionDisplay.gameObject.SetActive(true);
                    positionDisplay.affectedObject = pointCharge.gameObject.transform;
                    return;
                }
            });

            pointChargeChargeInput.OnValueChanged.AddListener((float newCharge) =>
            {
                newCharge = newCharge * 1e-6f; // Conversion from microCoulomb to Coulomb
                var selected = SelectionSystem.Instance.GetSelectedObject();
                if (selected == null) return;
                var pointCharge = selected.GetComponent<PointCharge>();
                if (pointCharge == null) return;
                pointCharge.SetCharge(newCharge);
            });

            deleteButton.onClick.AddListener(() =>
            {
                var selected = SelectionSystem.Instance.GetSelectedObject();
                if (selected == null) return;
                GameObject.Destroy(selected.gameObject);
            });

            SetUISelectionEmpty();
        }
    }
}
