using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Assessment
{
    public enum AssessmentClass
    {
        StopWatch,
        Calculator,
        Pendulum
    }

    public class AssessmentObject : MonoBehaviour
    {
        public AssessmentClass assessmentClass;

        private readonly SortedList<string, AssessmentWatchValue> _watchValues = new SortedList<string, AssessmentWatchValue>();

        public IList<AssessmentWatchValue> WatchValues => _watchValues.Values;

        public string ObjectID => $"{gameObject.name}{gameObject.GetInstanceID()}";

        private void Start()
        {
            foreach (var watchValue in GetComponents<AssessmentWatchValue>())
                _watchValues.Add(watchValue.PropertyName, watchValue);

            AssessmentManager.Instance.RegisterAssessmentObject(this);
        }

        public void OnAttributeValueChanged(string propertyName)
        {
            var watchValue = _watchValues[propertyName];
            if (watchValue.IsDynamic)
                return;

            AssessmentManager.Instance.SendDataUpdate(ObjectID, propertyName, watchValue.GetValue());
        }
    }
}