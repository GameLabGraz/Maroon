using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLawNew
{
    public class ChargedPointSelectionUILogic : MonoBehaviour
    {
        [SerializeField] private GUIVector3InputLogic positionDisplay;
        [SerializeField] private GUIFloatInputLogic chargeInput;
        [SerializeField] private GUIButtonLogic deleteButton;

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
            deleteButton.OnButtonClick.AddListener(() => GameObject.Destroy(chargedPoint.gameObject));
        }
    }

}
