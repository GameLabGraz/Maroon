using System;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Tools.Voltmeter
{
    [Serializable] public class VoltmeterEvent : UnityEvent<string> { }
    public class VoltmeterPinPoint : Pin, IResetObject
    {
        [SerializeField] private IField eField;

        public VoltmeterEvent onVoltageChanged;
        public VoltmeterEvent onVoltageChangedUnit;

        private float _potential;

        public float GetPotential => _potential;

        void Update()
        {
            if (!gameObject.activeSelf || !eField) return; 

            _potential = eField.getStrength(transform.position);
        }

        public void ResetObject()
        {
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.SetActive(false);
        }
    }
}

