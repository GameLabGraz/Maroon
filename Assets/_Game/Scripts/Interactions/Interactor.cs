using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Maroon {
    public abstract class Interactor : MonoBehaviour {
        protected Interactable detectedInteractable;

        protected abstract void DetectInteractable();

        protected abstract bool ShallInteract();

        protected virtual void Update() {
            if (ShallInteract() && detectedInteractable != null) {
                detectedInteractable.Interact();
            }
        }

        protected Interactable GetInteractableFromTransform(Transform transform) {
            return transform.GetComponentInParent(typeof(Interactable));
        }
    }
}