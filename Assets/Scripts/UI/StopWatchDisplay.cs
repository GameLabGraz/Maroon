using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Maroon.Util;

namespace Maroon.UI
{
    [RequireComponent(typeof(Text))]
    public class StopWatchDisplay : MonoBehaviour
    {
        [SerializeField]
        private StopWatch _stopWatch;

        private Text _displayText;

        private void Start()
        {
            if (_stopWatch == null)
                return;

            _displayText = GetComponent<Text>();
            _displayText.text = $"0.00";

            _stopWatch.OnTick += StopWatchOnTick;
            _stopWatch.OnReset += StopWatchOnReset;
        }

        private void StopWatchOnReset(StopWatchEvent evt)
        {
            _displayText.text = $"0.00";
        }

        private void StopWatchOnTick(StopWatchEvent evt)
        {

            _displayText.text = string.Format(CultureInfo.InvariantCulture, "{0:0.0}", evt.SecondsPassed);
        }
    }
}

