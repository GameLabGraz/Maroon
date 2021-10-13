using System;
using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts
{
    public class CatalystReactor : MonoBehaviour, IResetObject
    {

        private bool _reactorFilled;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name.Equals("CatalystMaterial"))
            {
                _reactorFilled = true;
            }
        }

        public void ResetObject()
        {
            _reactorFilled = false;
        }
    }
}
