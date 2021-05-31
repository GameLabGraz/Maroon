using System.Globalization;
using UnityEngine;

namespace Maroon.Tools.Stopwatch.UI
{
    public abstract class StopWatchBaseDisplay : MonoBehaviour
    {
        public abstract string DisplayText { set; get; }

        protected virtual void Start()
        {
            DisplayText = $"0.00";
        }

        public void StopWatchOnReset(SWEventArgs args)
        {
            DisplayText = $"0.00";
        }

        public void StopWatchOnTick(SWEventArgs args)
        {

            DisplayText = string.Format(CultureInfo.InvariantCulture, "{0:0.0}", args.SecondsPassed);
        }
    }
}

