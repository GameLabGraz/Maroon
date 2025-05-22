using GEAR.Gadgets.Extensions;
using Maroon.GlobalEntities;
using Maroon.Physics.CoordinateSystem;
using System.Globalization;
using GameLabGraz.UI;
using UnityEngine;

namespace Maroon.Tools.Voltmeter
{
    public class VoltmeterDisplay : MonoBehaviour
    {
        [SerializeField] private InputField voltageText;
        [SerializeField] private InputField distanceText;
        [SerializeField] private Unit selectedDistanceUnit;
        [SerializeField] private VoltmeterLogic voltmeter;

        public void LateUpdate()
        {
            // Note(MartinR): I think voltmeter should just update the voltage once per frame, instead of only when
            //      something has moved. In the previous version, the value would not be updated if charges were moved (CoulombsLaw),
            //      and while the simulation runs this has to be called once per frame anyway.
            if (voltmeter == null) return;

            if(!voltmeter.isOn || !voltmeter.startPoint.gameObject.activeSelf || !voltmeter.endPoint.gameObject.activeSelf)
            {
                voltageText.text = "---";
                distanceText.text = "---";
                return;
            }

            var culture = CultureInfo.InvariantCulture;
            var distanceUnit = CoordSystemHandler.Instance.IsCoordSystemAvailable ? selectedDistanceUnit : Unit.mm;
            var distance = voltmeter.CalculateDistance(distanceUnit);
            distanceText.text = string.Format(culture, "{0:0.###}", distance) + distanceUnit.GetStringValue();

            var difference = voltmeter.GetDifference().ToString();
            voltageText.text = string.Format(culture, "{0} {1}", difference, voltmeter.GetCurrentUnit());
        }

        public void UpdateDisplay()
        {
            // Note(MartinR): See comment in LateUpdate
        }
    }
}
