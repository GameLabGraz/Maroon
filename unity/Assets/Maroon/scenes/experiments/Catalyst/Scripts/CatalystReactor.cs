using System;
using UnityEngine;

namespace Maroon.Chemistry.Catalyst
{
    public class CatalystReactor : MonoBehaviour, IResetObject
    {
        public Action OnReactorFilled;
        
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
                OnReactorFilled?.Invoke();
            }

        }

        public void ResetObject()
        {
            _reactorFilled = false;
        }
    }
}
