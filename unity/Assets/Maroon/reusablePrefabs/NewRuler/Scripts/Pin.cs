using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Tools
{
    public class Pin : MonoBehaviour
    {
        public UnityEvent onRulerPinEnable;
        public UnityEvent onRulerPinDisable;

        private void OnDisable()
        {
            if (onRulerPinDisable != null)
                onRulerPinDisable.Invoke();
        }

        private void OnEnable()
        {
            if (onRulerPinEnable != null)
                onRulerPinEnable.Invoke();
        }
    }
}


