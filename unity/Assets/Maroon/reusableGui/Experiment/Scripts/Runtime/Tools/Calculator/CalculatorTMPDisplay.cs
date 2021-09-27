using TMPro;
using UnityEngine;

namespace Maroon.Tools.Calculator.UI
{
    public class CalculatorTMPDisplay : CalculatorBaseDisplay
    {
        [SerializeField]
        private TMP_InputField _mainDisplay;

        [SerializeField]
        private TMP_Text _sideDisplay;

        public override string MainDisplayText
        {
            get => _mainDisplay.text;
            set => _mainDisplay.text = value;
        }

        public override string SideDisplayText
        {
            get => _sideDisplay.text;
            set => _sideDisplay.text = value;
        }
    }
}
