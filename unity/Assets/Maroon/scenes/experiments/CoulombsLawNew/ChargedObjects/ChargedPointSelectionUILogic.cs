using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLawNew
{
    public class ChargedPointSelectionUILogic : MonoBehaviour
    {
        [SerializeField] private GuiVector3InputHandler positionDisplay;
        [SerializeField] private GuiFloatInputHandler chargeInput;
        [SerializeField] private UnityEngine.UI.Button deleteButton;

        private ChargedPoint chargedPoint;

        private void Start()
        {
            chargedPoint = SelectionSystem.Instance.GetSelectedObject().GetComponent<ChargedPoint>();
            Debug.Assert(chargedPoint != null, "Charged object must be selected when this ui is created");

            positionDisplay.TrackTransform(chargedPoint.transform);

            chargeInput.SetMinMax(-ChargedPoint.MAX_ABSOLUTE_CHARGE * 1e6f, ChargedPoint.MAX_ABSOLUTE_CHARGE * 1e6f);
            chargeInput.SetInitialValue(chargedPoint.GetCharge() * 1e6f);
            chargeInput.SetValue(chargedPoint.GetCharge() * 1e6f);
            chargeInput.OnValueChanged.AddListener((float newCharge) => { 
                chargedPoint.SetCharge(newCharge * 1e-6f); 
            });
                
            deleteButton.onClick.AddListener(() => GameObject.Destroy(chargedPoint.gameObject));
        }
    }

}
