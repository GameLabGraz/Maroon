using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class CoulombsLawUILogic : MonoBehaviour
    {
        [SerializeField] private PointCharge prefabPointCharge;
        [SerializeField] private ChargedRod prefabChargedRod;
        [SerializeField] private Transform parentForNewObjects;

        [Header("UI-Element References")]
        [SerializeField] private GuiIconTo3DObjectDrag dragIconParticle;
        [SerializeField] private GuiIconTo3DObjectDrag dragIconChargedRod;

        [SerializeField] private TMPro.TMP_Text selectionLabel;
        [SerializeField] private UnityEngine.UI.Button deleteButton;

        [SerializeField] private GuiFloatInputHandler pointChargeChargeInput;
        [SerializeField] private GuiPositionDisplayHandler pointChargePosDisplay;

        [SerializeField] private GuiFloatInputHandler chargedRodChargeDensityInput;
        [SerializeField] private GuiPositionDisplayHandler chargedRodPosA;
        [SerializeField] private GuiPositionDisplayHandler chargedRodPosB;

        private void SetUISelectionEmpty()
        {
            selectionLabel.text = "Selection: Empty";
            deleteButton.gameObject.SetActive(false);

            pointChargeChargeInput.gameObject.SetActive(false);
            pointChargePosDisplay.gameObject.SetActive(false);

            chargedRodChargeDensityInput.gameObject.SetActive(false);
            chargedRodPosA.gameObject.SetActive(false);
            chargedRodPosB.gameObject.SetActive(false);
        }

        private void Awake()
        {
            pointChargeChargeInput.SetMinMax(-PointCharge.MAX_ABSOLUTE_CHARGE * 1e6f, PointCharge.MAX_ABSOLUTE_CHARGE * 1e6f);

            dragIconParticle.OnDragFinished.AddListener((Vector3 pos) =>
            {
                var newParticle = GameObject.Instantiate(prefabPointCharge, pos, Quaternion.identity, parentForNewObjects);
            });

            dragIconChargedRod.OnDragFinished.AddListener((Vector3 pos) =>
            {
                var newRod = GameObject.Instantiate(prefabChargedRod, pos, Quaternion.identity, parentForNewObjects);
                newRod.SetRodPosition(pos, pos + Vector3.up);
            });

            chargedRodPosA.OnEndEdit.AddListener((Vector3 _unused) =>
            {
                var selectable = SelectionSystem.Instance.GetSelectedObject();
                if (selectable == null) return;
                var rod = selectable.GetComponent<ChargedRod>();
                if (rod == null) return;
                rod.SetRodPosition(chargedRodPosA.GetValue(), chargedRodPosB.GetValue());
            });
            chargedRodPosB.OnEndEdit.AddListener((Vector3 _unused) =>
            {
                var selectable = SelectionSystem.Instance.GetSelectedObject();
                if (selectable == null) return;
                var rod = selectable.GetComponent<ChargedRod>();
                if (rod == null) return;
                rod.SetRodPosition(chargedRodPosA.GetValue(), chargedRodPosB.GetValue());
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
                    pointChargePosDisplay.gameObject.SetActive(true);
                    pointChargePosDisplay.affectedObject = pointCharge.gameObject.transform;
                    return;
                }

                var chargedRod = selectable.GetComponent<ChargedRod>();
                if (chargedRod != null)
                {
                    selectionLabel.text = "Selection: Charged Rod";
                    chargedRodPosA.gameObject.SetActive(true);
                    chargedRodPosA.SetValue(chargedRod.GetStartPos());
                    chargedRodPosB.gameObject.SetActive(true);
                    chargedRodPosB.SetValue(chargedRod.GetEndPos());
                    chargedRodChargeDensityInput.gameObject.SetActive(true);
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
