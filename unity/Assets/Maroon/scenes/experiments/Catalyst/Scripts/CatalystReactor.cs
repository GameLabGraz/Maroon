﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Chemistry.Catalyst
{
    public class CatalystReactor : MonoBehaviour, IResetObject
    {
        [SerializeField] GameObject reactorWindowGameObject;
        [SerializeField] GameObject startButton;
        
        public UnityEvent OnReactorFilled;
        
        private bool _reactorFilled;

        private void Start()
        {
            if (startButton)
                startButton.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name.Equals("CatalystMaterial"))
            {
                _reactorFilled = true;
                other.gameObject.GetComponent<MeshRenderer>().enabled = false;
                StartCoroutine(DelayedWindowClose());
            }
        }

        private IEnumerator DelayedWindowClose()
        {
            yield return new WaitForSeconds(0.5f);
            reactorWindowGameObject.SetActive(true);
            if (startButton)
                startButton.SetActive(true);
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
