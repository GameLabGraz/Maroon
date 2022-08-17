using GameLabGraz.UI;
using TMPro;
using UnityEngine;

namespace Maroon.UI
{
    public abstract class ResetUI : MonoBehaviour, IResetObject
    {

        [SerializeField] protected bool allowReset = true;

        public bool AllowReset
        {
            get => allowReset;
            set => allowReset = value;
        }

        public abstract void ResetObject();
    }

    public static class ResetUIExtension
    {
        public static void AllowReset(this Slider slider, bool value)
        {
            var resetObj = slider.GetComponent<ResetSlider>();
            if (resetObj == null)
                resetObj = slider.gameObject.AddComponent<ResetSlider>();

            resetObj.AllowReset = value;
        }

        public static void AllowReset(this TMP_Dropdown dropdown, bool value)
        {
            var resetObj = dropdown.GetComponent<ResetDropDown>();
            if (resetObj == null)
                resetObj = dropdown.gameObject.AddComponent<ResetDropDown>();

            resetObj.AllowReset = value;
        }
    }
}
