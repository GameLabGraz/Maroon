using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class ChargedRodSelectionUILogic : MonoBehaviour
    {
        [SerializeField] private GUIVector3InputLogic positionDisplay;
        [SerializeField] private GUIVector3InputLogic directionDisplay;
        [SerializeField] private GUIFloatInputLogic chargeDensityInput;
        [SerializeField] private GUIButtonLogic deleteButton;

        private ChargedRod chargedRod;

        private void Start()
        {
            chargedRod = SelectionSystem.Instance.GetSelectedObject().GetComponent<ChargedRod>();
            Debug.Assert(chargedRod != null, "Charged object must be selected when this ui is created");

            positionDisplay.TrackTransform(chargedRod.transform);
            directionDisplay.SetValue(chargedRod.GetDirection());
            directionDisplay.OnEndEdit.AddListener((Vector3 newDirection) =>
            {
                if (newDirection.magnitude < 0.001)
                {
                    newDirection = Vector3.up;
                }
                chargedRod.SetRodParameters(chargedRod.transform.position, newDirection);
            });

            chargeDensityInput.SetMinMax(-ChargedRod.MAX_CHARGE_DENSITY, ChargedRod.MAX_CHARGE_DENSITY);
            chargeDensityInput.SetValue(chargedRod.GetChargeDensity());
            chargeDensityInput.OnValueChanged.AddListener((float newDensity) => { chargedRod.SetChargeDensity(newDensity); });
            deleteButton.OnButtonClick.AddListener(() => GameObject.Destroy(chargedRod.gameObject));
        }
    }
}
