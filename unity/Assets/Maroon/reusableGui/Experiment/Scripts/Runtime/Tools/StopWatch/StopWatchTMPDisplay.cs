using TMPro;
using UnityEngine;

namespace Maroon.Tools.Stopwatch.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class StopWatchTMPDisplay : StopWatchBaseDisplay
    {
        private TMP_Text _displayText;

        public override string DisplayText
        {
            get => _displayText.text;
            set => _displayText.text = value;
        }

        protected override void Start()
        {
            _displayText = GetComponent<TMP_Text>();
            base.Start();
        }
    }
}

