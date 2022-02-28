using System;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Tools.Voltmeter
{
    [Serializable] public class VoltmeterEvent : UnityEvent<string> { }
    public class VoltmeterPinPoint : MonoBehaviour, IResetObject
    {
        [SerializeField] private IField eField;
        [SerializeField] private float potential;

        public VoltmeterEvent onVoltageChanged;
        public VoltmeterEvent onVoltageChangedUnit;

        public float GetPotential => potential;

        void Update()
        {
            if (!gameObject.activeSelf || !eField) return; 

            potential = eField.getStrength(transform.position);
        }

        public void ResetObject()
        {
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.SetActive(false);
        }
    }
}

