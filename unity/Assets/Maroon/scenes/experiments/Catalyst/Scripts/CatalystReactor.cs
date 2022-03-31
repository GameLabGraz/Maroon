using System;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Chemistry.Catalyst
{
    public class CatalystReactor : MonoBehaviour, IResetObject
    {
        public UnityEvent OnReactorFilled;
        
        private bool _reactorFilled;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name.Equals("CatalystMaterial"))
            {
                _reactorFilled = true;
            }
        }
        
        public void OnMouseDown()
        {
            if (SimulationController.Instance.SimulationRunning && _reactorFilled)
            {
                _reactorFilled = false;
                OnReactorFilled?.Invoke(); // also sets the button to add CO and O2 to interactable
            }

        }

        public void ResetObject()
        {
            _reactorFilled = false;
        }
    }
}
