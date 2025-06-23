using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLawNew
{
    public class ChargedPlaneSelectionUILogic : MonoBehaviour
    {
        [SerializeField] private GuiPositionDisplayHandler positionDisplay;
        [SerializeField] private GuiPositionDisplayHandler normalDisplay;
        [SerializeField] private GuiFloatInputHandler chargeDensityInput;
        [SerializeField] private UnityEngine.UI.Button deleteButton;

        private ChargedPlane chargedPlane;

        private void Start()
        {
            chargedPlane = SelectionSystem.Instance.GetSelectedObject().GetComponent<ChargedPlane>();
            Debug.Assert(chargedPlane != null, "Charged object must be selected when this ui is created");

            positionDisplay.affectedObject = chargedPlane.transform;
            normalDisplay.SetValue(chargedPlane.GetNormal());
            normalDisplay.OnEndEdit.AddListener((Vector3 newNormal) =>
            {
                if (newNormal.magnitude < 0.001)
                {
                    newNormal = Vector3.up;
                }
                chargedPlane.SetPlaneParameters(chargedPlane.transform.position, newNormal);
            });

            chargeDensityInput.SetMinMax(-ChargedPlane.MAX_CHARGE_DENSITY * 1e6f, ChargedPlane.MAX_CHARGE_DENSITY * 1e6f);
            chargeDensityInput.SetValue(chargedPlane.GetChargeDensity() * 1e6f);
            chargeDensityInput.OnValueChanged.AddListener((float newDensity) => { chargedPlane.SetChargeDensity(newDensity * 1e-6f); });

            deleteButton.onClick.AddListener(() => GameObject.Destroy(chargedPlane.gameObject));
        }
    }
}
