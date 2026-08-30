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
        [SerializeField] private GUIBoolInputLogic createFieldLinesToggle;
        [SerializeField] private GUIButtonLogic deleteButton;
        [SerializeField] private GUIBoolInputLogic uiIsConductiveToggle;

        private ChargedRod chargedRod;

        private void Start()
        {
            chargedRod = SelectionSystem.Instance.GetSelectedObject().GetComponent<ChargedRod>();
            Debug.Assert(chargedRod != null, "Charged object must be selected when this ui is created");

            positionDisplay.TrackTransform(chargedRod.transform);
            positionDisplay.OnEndEdit.AddListener((Vector3 newPos) =>
            {
                chargedRod.SetRodParameters(newPos, chargedRod.GetDirection());
            });
            directionDisplay.SetValue(chargedRod.GetDirection());
            directionDisplay.OnEndEdit.AddListener((Vector3 newDirection) =>
            {
                if (newDirection.magnitude < 0.001)
                {
                    newDirection = Vector3.up;
                }
                chargedRod.SetRodParameters(chargedRod.transform.position, newDirection);
            });

            createFieldLinesToggle.SetValue(chargedRod.generateFieldLines);
            createFieldLinesToggle.OnValueChanged.AddListener((bool newValue) =>
            {
                chargedRod.generateFieldLines = newValue;
            });
            uiIsConductiveToggle.SetValue(chargedRod.isConductive);
            uiIsConductiveToggle.OnValueChanged.AddListener((bool newValue) =>
            {
                chargedRod.isConductive = newValue;
            });

            chargeDensityInput.SetMinMax(-ChargedRod.MAX_CHARGE_DENSITY, ChargedRod.MAX_CHARGE_DENSITY);
            chargeDensityInput.SetValue(chargedRod.GetChargeDensity());
            chargeDensityInput.OnValueChanged.AddListener((float newDensity) => { chargedRod.SetChargeDensity(newDensity); });
            deleteButton.OnButtonClick.AddListener(() => GameObject.Destroy(chargedRod.gameObject));

        }
    }
}
