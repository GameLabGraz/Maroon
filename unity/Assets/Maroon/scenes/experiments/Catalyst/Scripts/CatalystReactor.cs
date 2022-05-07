using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Chemistry.Catalyst
{
    public class CatalystReactor : MonoBehaviour, IResetObject
    {
        [SerializeField] GameObject reactorWindowGameObject;
        
        public UnityEvent OnReactorFilled;
        
        private bool _reactorFilled;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name.Equals("CatalystMaterial"))
            {
                _reactorFilled = true;
                StartCoroutine(DelayedWindowClose());
            }
        }

        private IEnumerator DelayedWindowClose()
        {
            yield return new WaitForSeconds(0.5f);
            reactorWindowGameObject.SetActive(true);
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
            reactorWindowGameObject.SetActive(false);
        }
    }
}
