using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class CoulombsLawUILogic : MonoBehaviour
    {
        [SerializeField] private PointCharge prefabPointCharge;
        [SerializeField] private ChargedRod prefabChargedRod;
        [SerializeField] private ChargedPlane prefabChargedPlane;
        [SerializeField] private Transform parentForNewObjects;

        [Header("UI-Element References")]
        [SerializeField] private GuiIconTo3DObjectDrag dragIconParticle;
        [SerializeField] private GuiIconTo3DObjectDrag dragIconChargedRod;
        [SerializeField] private GuiIconTo3DObjectDrag dragIconChargedPlane;

        [SerializeField] private TMPro.TMP_Text selectionLabel;
        [SerializeField] private UnityEngine.UI.Button deleteButton;

        [SerializeField] private GuiFloatInputHandler pointChargeChargeInput;
        [SerializeField] private GuiPositionDisplayHandler pointChargePosDisplay;

        [SerializeField] private GuiFloatInputHandler chargedRodChargeDensityInput;
        [SerializeField] private GuiPositionDisplayHandler chargedRodStartPos;
        [SerializeField] private GuiPositionDisplayHandler chargedRodDirection;

        [SerializeField] private GuiFloatInputHandler chargedPlaneChargeDensityInput;
        [SerializeField] private GuiPositionDisplayHandler chargedPlanePosInput;
        [SerializeField] private GuiPositionDisplayHandler chargedPlaneNormalInput;

        private void SetUISelectionEmpty()
        {
            selectionLabel.text = "Selection: Empty";
            deleteButton.gameObject.SetActive(false);

            pointChargeChargeInput.gameObject.SetActive(false);
            pointChargePosDisplay.gameObject.SetActive(false);

            chargedRodChargeDensityInput.gameObject.SetActive(false);
            chargedRodStartPos.gameObject.SetActive(false);
            chargedRodDirection.gameObject.SetActive(false);

            chargedPlaneChargeDensityInput.gameObject.SetActive(false);
            chargedPlanePosInput.gameObject.SetActive(false);
            chargedPlaneNormalInput.gameObject.SetActive(false);
        }

        private void Awake()
        {
            // Initialize PointCharge inputs
            dragIconParticle.OnDragFinished.AddListener((Vector3 pos) =>
            {
                var newParticle = GameObject.Instantiate(prefabPointCharge, pos, Quaternion.identity, parentForNewObjects);
            });

            pointChargeChargeInput.SetMinMax(-PointCharge.MAX_ABSOLUTE_CHARGE * 1e6f, PointCharge.MAX_ABSOLUTE_CHARGE * 1e6f);
            pointChargeChargeInput.OnValueChanged.AddListener((float newCharge) =>
            {
                newCharge = newCharge * 1e-6f; // Conversion from microCoulomb to Coulomb
                var selected = SelectionSystem.Instance.GetSelectedObject();
                if (selected == null) return;
                var pointCharge = selected.GetComponent<PointCharge>();
                if (pointCharge == null) return;
                pointCharge.SetCharge(newCharge);
            });



            // Initialize ChargedRod inputs
            dragIconChargedRod.OnDragFinished.AddListener((Vector3 pos) =>
            {
                var newRod = GameObject.Instantiate(prefabChargedRod, pos, Quaternion.identity, parentForNewObjects);
                newRod.SetRodParameters(pos, Vector3.up);
            });

            chargedRodDirection.OnEndEdit.AddListener((Vector3 newDirection) =>
            {
                var selectable = SelectionSystem.Instance.GetSelectedObject();
                if (selectable == null) return;
                var rod = selectable.GetComponent<ChargedRod>();
                if (rod == null) return;
                if (newDirection.magnitude < 0.001)
                {
                    newDirection = Vector3.up;
                    chargedRodDirection.SetValue(newDirection);
                }
                rod.SetRodParameters(rod.transform.position, newDirection);
            });

            chargedRodChargeDensityInput.SetMinMax(-ChargedRod.MAX_CHARGE_DENSITY * 1e6f, ChargedRod.MAX_CHARGE_DENSITY * 1e6f);
            chargedRodChargeDensityInput.OnValueChanged.AddListener((float newDensity) =>
            {
                var selectable = SelectionSystem.Instance.GetSelectedObject();
                if (selectable == null) return;
                var rod = selectable.GetComponent<ChargedRod>();
                if (rod == null) return;
                rod.SetChargeDensity(newDensity * 1e-6f);
            });



            // Initialize ChargedPlane inputs
            dragIconChargedPlane.OnDragFinished.AddListener((Vector3 pos) =>
            {
                var newPlane = GameObject.Instantiate(prefabChargedPlane, pos, Quaternion.identity, parentForNewObjects);
                newPlane.SetPlaneParameters(pos, Vector3.right);
            });
            chargedPlaneNormalInput.OnEndEdit.AddListener((Vector3 newNormal) =>
            {
                var selectable = SelectionSystem.Instance.GetSelectedObject();
                if (selectable == null) return;
                var plane = selectable.GetComponent<ChargedPlane>();
                if (plane == null) return;
                if (newNormal.magnitude < 0.001)
                {
                    newNormal = Vector3.up;
                    chargedPlaneNormalInput.SetValue(newNormal);
                }
                plane.SetPlaneParameters(plane.transform.position, newNormal);
            });

            chargedPlaneChargeDensityInput.SetMinMax(-ChargedPlane.MAX_CHARGE_DENSITY * 1e6f, ChargedPlane.MAX_CHARGE_DENSITY * 1e6f);
            chargedPlaneChargeDensityInput.OnValueChanged.AddListener((float newDensity) =>
            {
                var selectable = SelectionSystem.Instance.GetSelectedObject();
                if (selectable == null) return;
                var plane = selectable.GetComponent<ChargedPlane>();
                if (plane == null) return;
                plane.SetChargeDensity(newDensity * 1e-6f);
            });


            // Initialize common inputs (selection changed and delete button)
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
                    chargedRodStartPos.gameObject.SetActive(true);
                    chargedRodStartPos.affectedObject = chargedRod.transform;
                    chargedRodDirection.gameObject.SetActive(true);
                    chargedRodDirection.SetValue(chargedRod.GetDirection());
                    chargedRodChargeDensityInput.gameObject.SetActive(true);
                    chargedRodChargeDensityInput.SetValue(chargedRod.GetChargeDenstiy() * 1e6f);
                    return;
                }

                var chargedPlane = selectable.GetComponent<ChargedPlane>();
                if (chargedPlane != null)
                {
                    selectionLabel.text = "Selection: ChargedPlane";
                    chargedPlanePosInput.gameObject.SetActive(true);
                    chargedPlanePosInput.affectedObject = chargedPlane.transform;
                    chargedPlaneNormalInput.gameObject.SetActive(true);
                    chargedPlaneNormalInput.SetValue(chargedPlane.GetNormal());
                    chargedPlaneChargeDensityInput.gameObject.SetActive(true);
                    chargedPlaneChargeDensityInput.SetValue(chargedPlane.GetChargeDensity() * 1e6f);
                    return;
                }
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
