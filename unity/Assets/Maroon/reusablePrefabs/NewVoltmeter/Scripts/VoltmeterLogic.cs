using Maroon.GlobalEntities;
using Maroon.Physics;
using Maroon.Physics.CoordinateSystem;
using UnityEngine;

namespace Maroon.Tools.Voltmeter
{
    public class VoltmeterLogic : MonoBehaviour
    {
        public Pin startPoint;
        public Pin endPoint;

        public bool isOn;
        public bool showUnit;

        public QuantityFloat currentValue = 0;

        public float CalculateDistance(Unit targetUnit = Unit.m) => CoordSystemHandler.Instance.CalculateDistanceBetween(startPoint.transform.position, endPoint.transform.position, targetUnit);

        public float GetDifference()
        {
            var endPinPotential = endPoint.gameObject.GetComponent<VoltmeterPinPoint>().GetPotential;
            var startPinPotential = startPoint.gameObject.GetComponent<VoltmeterPinPoint>().GetPotential;

            var currentDifference = startPinPotential - endPinPotential;

            if (Mathf.Abs(currentDifference - currentValue.Value) > 0.00000001)
                currentValue.Value = currentDifference;

            var check = currentValue.Value;
            for (var cnt = 0; Mathf.Abs(check) < 1f && cnt < 2; ++cnt)
            {
                check *= Mathf.Pow(10, 3);
            }

            return check;
        }

        public string GetCurrentUnit()
        {
            if (Mathf.Abs(currentValue.Value) > 1f) return "V";
            if (Mathf.Abs(currentValue.Value) > 0.001) return "mV";
            if (Mathf.Abs(currentValue.Value) > 0.000001) return "\u00B5V";
            return "";
        }
    }
}
