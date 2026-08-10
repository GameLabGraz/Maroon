using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Chemistry.Catalyst
{
    public class CatalystReactor : MonoBehaviour, IResetObject
    {
        [SerializeField] GameObject reactorWindowGameObject;
        [SerializeField] GameObject startButton;
        [SerializeField] Animator animator;
        [SerializeField] Transform cupboardDoorTransform;
        
        public UnityEvent OnReactorFilled;
        public bool ReactorFilled { get => _reactorFilled; }
        public bool ReactionStarted { get; set; }
        private bool _reactorFilled;
        private Vector3 _cupboardInitialPosition;

        private void Start()
        {
            if (startButton)
                startButton.SetActive(false);
            Vector3 orgPos = cupboardDoorTransform.position;
            _cupboardInitialPosition = new Vector3(orgPos.x, orgPos.y, orgPos.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name.Equals("CatalystMaterial"))
            {
                _reactorFilled = true;
                other.gameObject.GetComponent<MeshRenderer>().enabled = false;
                other.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
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
                ReactionStarted = true;
                OnReactorFilled?.Invoke(); // also sets the button to add CO and O2 to interactable
            }
        }

        public void ResetObject()
        {
            animator.enabled = false; // enabled through simulation controller start action
            cupboardDoorTransform.position = _cupboardInitialPosition;
            _reactorFilled = false;
            ReactionStarted = false;
            reactorWindowGameObject.SetActive(false);
        }
    }
}
