using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using Valve.VR;
using VRTK;

namespace Maroon {
    public class VRTKInteractableBridge : MonoBehaviour {
        [SerializeField]
        private VRTK_InteractableObject vrtkInteractable;

        [Tooltip("Must have a component that implements the IInteractable interface.")]
        [SerializeField]
        private GameObject interactableObject;

        private IInteractable interactable;

        private void Awake() {
            Assert.IsNotNull(interactableObject, "An interactable object must be assigned!");

            interactable = interactableObject.GetComponent(typeof(IInteractable)) as IInteractable;
            Assert.IsNotNull(interactable, "The interactable object doesn't contain a component that implements the " +
                                           "IInteractable interface!");

            if (!vrtkInteractable) {
                vrtkInteractable = GetComponent<VRTK_InteractableObject>();
            }

            Assert.IsNotNull(vrtkInteractable, "A VRTK interactable object must be assigned!");
        }

        protected virtual void OnEnable() {
            vrtkInteractable.InteractableObjectUsed += OnUsed;
        }

        protected virtual void OnDisable() {
            vrtkInteractable.InteractableObjectUsed -= OnUsed;
        }

        private void OnUsed(object sender, InteractableObjectEventArgs e) {
            interactable.Interact();
        }
    }
}