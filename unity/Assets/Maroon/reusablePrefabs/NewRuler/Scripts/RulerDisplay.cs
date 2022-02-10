using System.Globalization;
using Maroon.GlobalEntities;
using Maroon.Physics.CoordinateSystem;
using TMPro;
using UnityEngine;

namespace Maroon.Tools.Ruler
{
   public class RulerDisplay : MonoBehaviour
    {
        [SerializeField] private RulerLogic ruler;
        [SerializeField] private TMP_Text DistanceText;

        public Vector3 StartMeasuringPosition => CoordSystemHandler.Instance.GetSystemPosition(ruler.RulerStart.transform.position);
        public Vector3 EndMeasuringPosition => CoordSystemHandler.Instance.GetSystemPosition(ruler.RulerEnd.transform.position);

        public void UpdateDisplay()
        {
            if (ruler == null) return;

            if (!ruler.RulerStart.gameObject.activeSelf || !ruler.RulerEnd.gameObject.activeSelf)
            {
                DistanceText.text = "---";
            }
            else
            {
                var displayUnit = CoordSystemHandler.Instance.IsCoordSystemAvailable ? Unit.mm : Unit.m;
                var distance = ruler.CalculateDistance(displayUnit);
                DistanceText.text = string.Format(CultureInfo.InvariantCulture, "{0:0.###} ", distance) + displayUnit.ToString();
            }
        }
    }
}

