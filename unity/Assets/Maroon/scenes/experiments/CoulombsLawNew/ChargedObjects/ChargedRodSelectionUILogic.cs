using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class ChargedRodSelectionUILogic : MonoBehaviour
    {
        [SerializeField] private GuiPositionDisplayHandler positionDisplay;
        [SerializeField] private GuiPositionDisplayHandler directionDisplay;
        [SerializeField] private GuiFloatInputHandler chargeDensityInput;
        [SerializeField] private UnityEngine.UI.Button deleteButton;

        private ChargedRod chargedRod;

        private void Start()
        {
            chargedRod = SelectionSystem.Instance.GetSelectedObject().GetComponent<ChargedRod>();
            Debug.Assert(chargedRod != null, "Charged object must be selected when this ui is created");

            positionDisplay.affectedObject = chargedRod.transform;
            directionDisplay.SetValue(chargedRod.GetDirection());
            directionDisplay.OnEndEdit.AddListener((Vector3 newDirection) =>
            {
                if (newDirection.magnitude < 0.001)
                {
                    newDirection = Vector3.up;
                }
                chargedRod.SetRodParameters(chargedRod.transform.position, newDirection);
            });

            chargeDensityInput.SetMinMax(-ChargedRod.MAX_CHARGE_DENSITY * 1e6f, ChargedRod.MAX_CHARGE_DENSITY * 1e6f);
            chargeDensityInput.SetValue(chargedRod.GetChargeDensity() * 1e6f);
            chargeDensityInput.OnValueChanged.AddListener((float newDensity) => { chargedRod.SetChargeDensity(newDensity * 1e-6f); });

            deleteButton.onClick.AddListener(() => GameObject.Destroy(chargedRod.gameObject));
        }
    }
}
