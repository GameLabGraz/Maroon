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
        
        private void Start()
        {
            _isInteractable = true;
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
            animator.SetTrigger(_resetAnimation);
            _isInteractable = true;
        }
    }
}
