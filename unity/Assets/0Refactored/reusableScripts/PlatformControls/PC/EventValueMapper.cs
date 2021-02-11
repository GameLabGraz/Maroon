using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PlatformControls.PC
{

    [Serializable]
    public class MappedFloatEvent : UnityEvent<float>
    {
    }

    public class EventValueMapper : MonoBehaviour
    {
        [Serializable]
        public struct ReplaceValue
        {
            public int originalValue;
            public float valueToReplace;
        }

        public List<ReplaceValue> replaceMap = new List<ReplaceValue>();

        public MappedFloatEvent onMappedEvent;

        public void OnEvent(int eventValue)
        {
            var foundValue = replaceMap.FindAll(x => x.originalValue == eventValue);
            if (foundValue.Count == 1)
                onMappedEvent.Invoke(foundValue[0].valueToReplace);
        }
    }
}