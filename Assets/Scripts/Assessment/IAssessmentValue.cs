using UnityEngine;

namespace Maroon.Assessment
{
    public abstract class IAssessmentValue : MonoBehaviour
    {
        public virtual string Name => gameObject.name;

        public abstract object GetValue();
    }
}
