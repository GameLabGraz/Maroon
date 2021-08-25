using UnityEngine;
using UnityEngine.UI;

namespace Maroon.Tools.Calculator.UI
{
    public class CalculatorTextDisplay : CalculatorBaseDisplay
    {
        [SerializeField]
        private InputField _mainDisplay;

        [SerializeField]
        private Text _sideDisplay;

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
