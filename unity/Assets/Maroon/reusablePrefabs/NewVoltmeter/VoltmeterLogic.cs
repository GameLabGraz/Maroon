using Maroon.GlobalEntities;
using Maroon.Physics.CoordinateSystem;
using GameLabGraz.UI;
using UnityEngine;
using Maroon.Utility;

namespace Maroon.Tools.Voltmeter
{
    public class VoltmeterLogic : MonoBehaviour
    {
        [Header("UI-Element References")]
        [SerializeField] private TMPro.TMP_Text voltageText;
        [SerializeField] private TMPro.TMP_Text distanceText;
        [SerializeField] private UIPositionDisplay positionDisplayPositivePin;
        [SerializeField] private UIPositionDisplay positionDisplayNegativePin;
        [SerializeField] private UIItemDragHandlerSimple positivePinDragHandler;
        [SerializeField] private UIItemDragHandlerSimple negativePinDragHandler;

        [Header("Game-Object References")]
        [SerializeField] private Maroon.Physics.Electromagnetism.EField eField;
        [SerializeField] private Transform positivePin;
        [SerializeField] private Transform negativePin;

        private void Awake()
        {
            // Note(MartinR): Pins have a weird starting position (Y=51.7m), I guess because they are children of some UI-components,
            //      which may have weird transforms for 3D objects,
            //      so I set the position manually to zero here (Pins are disabled at startup anyways, this only affects the UI)
            positivePin.transform.position = Vector3.zero;
            negativePin.transform.position = Vector3.zero;

            // Set UI callbacks
            positionDisplayPositivePin.affectedObject = positivePin;
            positionDisplayNegativePin.affectedObject = negativePin;

            positivePinDragHandler.OnDragFinished.AddListener((Vector3 pos) =>
            {
                if (positivePin == null) return;
                positivePin.gameObject.SetActive(true);
                positivePin.position = pos;
            });
            negativePinDragHandler.OnDragFinished.AddListener((Vector3 pos) =>
            {
                if (negativePin == null) return;
                negativePin.gameObject.SetActive(true);
                negativePin.position = pos;
            });
        }

        public void LateUpdate()
        {
            // Early exit if pins are not set
            if (!positivePin.gameObject.activeSelf || !negativePin.gameObject.activeSelf)
            {
                voltageText.text = "---";
                distanceText.text = "---";
                return;
            }

            // Calculate and update distance text
            float distance = 
                (CoordSystemHandler.Instance.GetSystemPosition(positivePin.position, Unit.m) - 
                CoordSystemHandler.Instance.GetSystemPosition(negativePin.position, Unit.m)).magnitude;
            SI_Prefix distancePrefix = SI_Prefix_Helpers.GetClosestPrefix(distance);
            distanceText.text = string.Format("{0:0.000}{1}m", distance / distancePrefix.GetFactor(), distancePrefix.GetAbbreviation());

            // Calculate and update Voltage text
            float voltage = 
                eField.getStrength(CoordSystemHandler.Instance.GetSystemPosition(positivePin.position)) -
                eField.getStrength(CoordSystemHandler.Instance.GetSystemPosition(negativePin.position));
            SI_Prefix voltagePrefix = SI_Prefix_Helpers.GetClosestPrefix(voltage);
            voltageText.text = string.Format("{0:0.000}{1}V", voltage / voltagePrefix.GetFactor(), voltagePrefix.GetAbbreviation());
        }
    }
}
