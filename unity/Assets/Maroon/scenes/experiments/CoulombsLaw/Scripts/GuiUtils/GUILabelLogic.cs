using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEAR.Localization.Text;

namespace Maroon.Experiments.CoulombsLawNew
{
    public enum SI_Prefix
    {
        PETA,
        TERA,
        GIGA,
        MEGA,
        KILO,
        NONE,
        MILLI,
        MICRO,
        NANO,
        PICO,
        FEMTO
    }

    public class SI_Prefix_Info
    {
        public SI_Prefix prefix;
        public float factor = 1.0f;
        public string name = "";
        public string abbreviation = "";

        public SI_Prefix_Info(SI_Prefix prefix)
        {
            this.prefix = prefix;
            switch (prefix)
            {
                case SI_Prefix.PETA:  factor = 1e15f;  abbreviation = "P"; name = "Peta"; break;
                case SI_Prefix.TERA:  factor = 1e12f;  abbreviation = "T"; name = "Tera"; break;
                case SI_Prefix.GIGA:  factor = 1e9f;   abbreviation = "G"; name = "Giga"; break;
                case SI_Prefix.MEGA:  factor = 1e6f;   abbreviation = "M"; name = "Mega"; break;
                case SI_Prefix.KILO:  factor = 1e3f;   abbreviation = "k"; name = "Kilo"; break;
                case SI_Prefix.NONE:  factor = 1e0f;   abbreviation = ""; name = ""; break;
                case SI_Prefix.MILLI: factor = 1e-3f;  abbreviation = "m"; name = "Milli"; break;
                case SI_Prefix.MICRO: factor = 1e-6f;  abbreviation = "µ"; name = "Micro"; break;
                case SI_Prefix.NANO:  factor = 1e-9f;  abbreviation = "n"; name = "Nano"; break;
                case SI_Prefix.PICO:  factor = 1e-12f; abbreviation = "p"; name = "Pico"; break;
                case SI_Prefix.FEMTO: factor = 1e-15f; abbreviation = "f"; name = "Femto"; break;
            }
        }
    }

    public class GUILabelLogic : MonoBehaviour
    {
        [SerializeField] private bool isHorizontal = false;
        [SerializeField] private string localizationKey = "";
        [SerializeField] private string unitAbbreviation = "";

        // Note: GuiLabelLogic should be mostly used by the other GUI prefabs, e.g. int/float/vector3 inputs.
        //      But it can also work by itself, to just display a name, which is why the start method does what it does.
        private void Start()
        {
            GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "ValueLabel").GetComponent<LocalizedTMP>().Key = localizationKey;
            GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "UnitLabel").SetActive(unitAbbreviation.Length > 0);
        }

        public void SetLabelData(string localizationKey, string unitAbbreviation, SI_Prefix prefix)
        {
            this.localizationKey = localizationKey;
            this.unitAbbreviation = unitAbbreviation;
            
            var parentSubwindow  = GUISubwindowLogic.FindParentSubWindow(gameObject);
            var nameLocalization = GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "ValueLabel").GetComponent<LocalizedTMP>();
            var nameLayout       = GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "ValueLabel").GetComponent<UnityEngine.UI.LayoutElement>();
            var unitLabel        = GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "UnitLabel").GetComponent<TMPro.TMP_Text>();
            var unitLayout       = GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "UnitLabel").GetComponent<UnityEngine.UI.LayoutElement>();

            nameLocalization.Key = localizationKey;
            unitLayout.minWidth = isHorizontal ? parentSubwindow.minimumUnitLabelWidth : 0;
            nameLayout.minWidth = isHorizontal ? parentSubwindow.minimumLabelWidth : 0;

            unitLabel.gameObject.SetActive(unitAbbreviation.Length > 0);
            string unitText = (new SI_Prefix_Info(prefix)).abbreviation + unitAbbreviation;
            if (!isHorizontal)
            {
                unitText = "[" + unitText + "]";
            }
            unitLabel.text = unitText;
        }
    }
}
