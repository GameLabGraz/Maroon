using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEAR.Localization.Text;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class GUIButtonLogic : MonoBehaviour
    {
        [SerializeField] private string buttonTextLocalizationKey = "";

        public UnityEngine.Events.UnityEvent OnButtonClick;

        void Start()
        {
            var buttonTextLocalization = GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "ButtonText").GetComponent<LocalizedTMP>();
            buttonTextLocalization.Key = buttonTextLocalizationKey;

            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                OnButtonClick.Invoke();
            });
        }
    }
}
