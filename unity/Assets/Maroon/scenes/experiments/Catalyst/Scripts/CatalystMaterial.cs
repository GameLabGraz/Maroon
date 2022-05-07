using System;
using UnityEngine;

namespace Maroon.Chemistry.Catalyst
{
    public class CatalystMaterial : MonoBehaviour, IResetObject
    {
        [SerializeField] Animator animator;

        private readonly int _insertCatalystMaterialAnimationTigger = Animator.StringToHash("insertCatMaterialTrigger");
        private readonly int _forceExitAnimation = Animator.StringToHash("forceExit");
        private readonly int _resetAnimation = Animator.StringToHash("reset");

        private bool _isInteractable;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        
        private void Start()
        {
            _isInteractable = true;
            _initialPosition = gameObject.transform.position;
            _initialRotation = gameObject.transform.rotation;
        }

        public void OnMouseDown()
        {
            if (SimulationController.Instance.SimulationRunning && _isInteractable)
            {
                animator.SetTrigger(_insertCatalystMaterialAnimationTigger);
                _isInteractable = false;
            }

        }

        public void ResetObject()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            animator.SetTrigger(_resetAnimation);
            gameObject.transform.position = _initialPosition;
            gameObject.transform.rotation = _initialRotation;
            _isInteractable = true;
        }
    }
}
