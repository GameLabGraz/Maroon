using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLawNew
{
    public class ChargedPlaneSelectionUILogic : MonoBehaviour
    {
        [SerializeField] private GUIVector3InputLogic positionDisplay;
        [SerializeField] private GUIVector3InputLogic normalDisplay;
        [SerializeField] private GUIFloatInputLogic chargeDensityInput;
        [SerializeField] private GUIButtonLogic deleteButton;

        private ChargedPlane chargedPlane;

        private void Start()
        {
            chargedPlane = SelectionSystem.Instance.GetSelectedObject().GetComponent<ChargedPlane>();
            Debug.Assert(chargedPlane != null, "Charged object must be selected when this ui is created");

            positionDisplay.TrackTransform(chargedPlane.transform);
            normalDisplay.SetValue(chargedPlane.GetNormal());
            normalDisplay.OnEndEdit.AddListener((Vector3 newNormal) =>
            {
                if (newNormal.magnitude < 0.001)
                {
                    newNormal = Vector3.up;
                }
                chargedPlane.SetPlaneParameters(chargedPlane.transform.position, newNormal);
            });

            chargeDensityInput.SetMinMax(-ChargedPlane.MAX_CHARGE_DENSITY, ChargedPlane.MAX_CHARGE_DENSITY);
            chargeDensityInput.SetValue(chargedPlane.GetChargeDensity());
            chargeDensityInput.OnValueChanged.AddListener((float newDensity) => { chargedPlane.SetChargeDensity(newDensity); });
            deleteButton.OnButtonClick.AddListener(() => { GameObject.Destroy(gameObject); });
        }
    }
}
