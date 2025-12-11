using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLawNew
{
    public class ChargedPointSelectionUILogic : MonoBehaviour
    {
        [SerializeField] private GUIVector3InputLogic positionDisplay;
        [SerializeField] private GUIFloatInputLogic chargeInput;
        [SerializeField] private GUIBoolInputLogic generateFieldLinesToggle;
        [SerializeField] private GUIButtonLogic deleteButton;

        [SerializeField] private GUIVector3InputLogic uiInitialVelocityInput;
        [SerializeField] private GUIFloatInputLogic   uiMassSlider;
        [SerializeField] private GUIBoolInputLogic    uiHasCollisionToggle;
        [SerializeField] private GUIBoolInputLogic    uiLockPositionToggle;
        [SerializeField] private GUIBoolInputLogic    uiContributeToEFieldToggle;
        [SerializeField] private GUIBoolInputLogic    uiIsConductiveToggle;

        private ChargedPoint chargedPoint;

        private void Start()
        {
            chargedPoint = SelectionSystem.Instance.GetSelectedObject().GetComponent<ChargedPoint>();
            Debug.Assert(chargedPoint != null, "Charged object must be selected when this ui is created");

            chargeInput.SetMinMax(-ChargedPoint.MAX_ABSOLUTE_CHARGE, ChargedPoint.MAX_ABSOLUTE_CHARGE);
            chargeInput.SetValue(chargedPoint.GetCharge());
            positionDisplay.TrackTransform(chargedPoint.transform);
            chargeInput.OnValueChanged.AddListener((float newCharge) => { 
                chargedPoint.SetCharge(newCharge); 
            });
            generateFieldLinesToggle.SetValue(chargedPoint.generateFieldLines);
            generateFieldLinesToggle.OnValueChanged.AddListener((bool newValue) =>
            {
                chargedPoint.generateFieldLines = newValue;
            });
            deleteButton.OnButtonClick.AddListener(() => GameObject.Destroy(chargedPoint.gameObject));

            uiInitialVelocityInput.SetValue(chargedPoint.GetVelocity());
            uiInitialVelocityInput.OnEndEdit.AddListener((Vector3 newValue) => 
            {
                chargedPoint.SetVelocity(newValue);
            });

            uiMassSlider.SetValue(chargedPoint.GetMass());
            uiMassSlider.OnValueChanged.AddListener((float newValue) => { chargedPoint.SetMass(newValue); });
            uiHasCollisionToggle.SetValue(chargedPoint.GetHasCollision());
            uiHasCollisionToggle.OnValueChanged.AddListener((bool newValue) => { chargedPoint.SetHasCollision(newValue); }); ;
            uiLockPositionToggle.SetValue(chargedPoint.lockPosition);
            uiLockPositionToggle.OnValueChanged.AddListener((bool newValue) => { chargedPoint.lockPosition = newValue; }); ;
            uiContributeToEFieldToggle.SetValue(chargedPoint.contributeToEField);
            uiContributeToEFieldToggle.OnValueChanged.AddListener((bool newValue) => { chargedPoint.contributeToEField = newValue; }); ;
            uiIsConductiveToggle.SetValue(chargedPoint.isConductive);
            uiIsConductiveToggle.OnValueChanged.AddListener((bool newValue) => { chargedPoint.isConductive = newValue; }); ;
        }

        private void LateUpdate()
        {
            uiInitialVelocityInput.SetValue(chargedPoint.GetVelocity());
        }
    }

}
