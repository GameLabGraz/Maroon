using UnityEngine;
using UnityEngine.UI;

namespace Maroon.Tools.Stopwatch.UI
{
    [RequireComponent(typeof(Text))]
    public class StopWatchTextDisplay : StopWatchBaseDisplay
    {
        private Text _displayText;

        public override string DisplayText
        {
            get => _displayText.text;
            set => _displayText.text = value;
        }

        protected override void Start()
        {
            _displayText = GetComponent<Text>();
            base.Start();
        }
    }
}

