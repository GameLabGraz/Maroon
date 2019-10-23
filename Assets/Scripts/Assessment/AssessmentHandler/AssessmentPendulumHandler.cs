using UnityEngine;

namespace Maroon.Assessment.Handler
{
    [RequireComponent(typeof(AssessmentObject))]
    public class AssessmentPendulumHandler : MonoBehaviour
    {
        public string ObjectID => $"{gameObject.name}{gameObject.GetInstanceID()}";

        public void PendulumOnRelease()
        {
            AssessmentManager.Instance.SendUserAction("pull", ObjectID);
        }
    }
}
