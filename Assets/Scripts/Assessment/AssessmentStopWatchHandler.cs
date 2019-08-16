using Antares.Evaluation.Util;
using Maroon.Tools;
using UnityEngine;

namespace Maroon.Assessment
{
    [RequireComponent(typeof(AssessmentObject))]
    public class AssessmentStopWatchHandler : MonoBehaviour
    {
        public string ObjectID => $"{gameObject.name}{gameObject.GetInstanceID()}";

        public void StopWatchOnStart(SWEventArgs args)
        {
            AssessmentManager.Instance.SendUserAction("start", ObjectID);
        }

        public void StopWatchOnStop(SWEventArgs args)
        {
            AssessmentManager.Instance.SendUserAction("stop", ObjectID);
            EventBuilder.Event().Action("stop", gameObject.name);
        }

        public void StopWatchOnReset(SWEventArgs args)
        {
            AssessmentManager.Instance.SendUserAction("reset", ObjectID);
        }
    }
}
