using Maroon.GlobalEntities;
using Maroon.Physics;
using Maroon.Physics.CoordinateSystem;
using GameLabGraz.UI;
using UnityEngine;

namespace Maroon.Tools.Voltmeter
{
    public class VoltmeterLogic : MonoBehaviour
    {
        [Header("UI-Element References")]
        [SerializeField] private InputField voltageText;
        [SerializeField] private InputField distanceText;

        [Header("Game-Object References")]
        [SerializeField] private Maroon.Physics.Electromagnetism.EField eField;
        [SerializeField] private Transform positivePin;
        [SerializeField] private Transform negativePin;

        public void LateUpdate()
        {
            if (!positivePin.gameObject.activeSelf || !negativePin.gameObject.activeSelf)
            {
                voltageText.text = "---";
                distanceText.text = "---";
                return;
            }

            // Calculate and update distance
            float distance = 
                (CoordSystemHandler.Instance.GetSystemPosition(positivePin.position, Unit.m) - 
                CoordSystemHandler.Instance.GetSystemPosition(negativePin.position, Unit.m)).magnitude;
            SI_Prefix distancePrefix = SI_Prefix_Helpers.GetClosestPrefix(distance);
            distanceText.text = string.Format("{0:0.000}{1}m", distance / distancePrefix.GetFactor(), distancePrefix.GetAbbreviation());

            // Calculate and update Voltage
            float voltage = 
                eField.getStrength(CoordSystemHandler.Instance.GetSystemPosition(positivePin.position)) -
                eField.getStrength(CoordSystemHandler.Instance.GetSystemPosition(negativePin.position));
            SI_Prefix voltagePrefix = SI_Prefix_Helpers.GetClosestPrefix(voltage);
            voltageText.text = string.Format("{0:0.000}{1}V", voltage / voltagePrefix.GetFactor(), voltagePrefix.GetAbbreviation());
        }

        public void SetPositivePinPositionAndActive(Vector3 pos)
        {
            if (positivePin == null) return;
            positivePin.gameObject.SetActive(true);
            positivePin.position = pos;
        }

        public void SetNegativePinPositionAndActive(Vector3 pos)
        {
            if (negativePin == null) return;
            negativePin.gameObject.SetActive(true);
            negativePin.position = pos;
        }
    }

    public enum SI_Prefix
    {
        PETA,
        TERA,
        GIGA,
        MEGA,
        KILO,
        NO_PREFIX,
        MILI,
        MICRO,
        NANO,
        PICO,
        FEMTO
    }

    public static class SI_Prefix_Helpers
    {
        public static float GetFactor(this SI_Prefix prefix)
        {
            switch (prefix)
            {
                case SI_Prefix.PETA: return 1e15f;
                case SI_Prefix.TERA: return 1e12f;
                case SI_Prefix.GIGA: return 1e9f;
                case SI_Prefix.MEGA: return 1e6f;
                case SI_Prefix.KILO: return 1e3f;
                case SI_Prefix.NO_PREFIX: return 1;
                case SI_Prefix.MILI: return 1e-3f;
                case SI_Prefix.MICRO: return 1e-6f;
                case SI_Prefix.NANO: return 1e-9f;
                case SI_Prefix.PICO: return 1e-12f;
                case SI_Prefix.FEMTO: return 1e-15f;
            }
            return 1;
        } 

        public static string GetAbbreviation(this SI_Prefix prefix)
        {
            switch (prefix)
            {
                case SI_Prefix.PETA: return "P";
                case SI_Prefix.TERA: return "T";
                case SI_Prefix.GIGA: return "G";
                case SI_Prefix.MEGA: return "M";
                case SI_Prefix.KILO: return "k";
                case SI_Prefix.NO_PREFIX: return "";
                case SI_Prefix.MILI: return "m";
                case SI_Prefix.MICRO: return "µ";
                case SI_Prefix.NANO: return "n";
                case SI_Prefix.PICO: return "p";
                case SI_Prefix.FEMTO: return "f";
            }
            return "";
        } 

        public static SI_Prefix GetClosestPrefix(float number)
        {
            number = Mathf.Abs(number);
            if (number >= 1e15f) return SI_Prefix.PETA;
            else if (number >= 1e12f) return SI_Prefix.TERA;
            else if (number >= 1e9f) return SI_Prefix.GIGA;
            else if (number >= 1e6f) return SI_Prefix.MEGA;
            else if (number >= 1e3f) return SI_Prefix.KILO;
            else if (number >= 1) return SI_Prefix.NO_PREFIX;
            else if (number >= 1e-3f) return SI_Prefix.MILI;
            else if (number >= 1e-6f) return SI_Prefix.MICRO;
            else if (number >= 1e-9f) return SI_Prefix.NANO;
            else if (number >= 1e-12f) return SI_Prefix.PICO;
            return SI_Prefix.FEMTO;
        }
    }
}
