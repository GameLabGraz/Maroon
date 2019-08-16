using System.Globalization;
using Maroon.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.UI
{
    [RequireComponent(typeof(Text))]
    public class StopWatchDisplay : MonoBehaviour
    {
        private Text _displayText;

        private void Start()
        {
            _displayText = GetComponent<Text>();
            _displayText.text = $"0.00";
        }

        public void StopWatchOnReset(SWEventArgs args)
        {
            _displayText.text = $"0.00";
        }

        public void StopWatchOnTick(SWEventArgs args)
        {

            _displayText.text = string.Format(CultureInfo.InvariantCulture, "{0:0.0}", args.SecondsPassed);
        }
    }
}

