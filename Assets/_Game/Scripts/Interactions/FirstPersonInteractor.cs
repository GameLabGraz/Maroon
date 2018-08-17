using System.Collections;
using System.Collections.Generic;
using Maroon;
using UnityEngine;
using UnityEngine.Assertions;
using VRTK;

namespace Maroon {
    public class FirstPersonInteractor : Interactor {

        [SerializeField]
        private new Camera camera = null;

        [SerializeField]
        private float reach = 2;

        protected override void DetectInteractable() {
            RaycastHit hit;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, reach,
                                LayerMask.GetMask("Default"))) {
                interactable = hit.transform.GetComponentInParent(typeof(IInteractable)) as IInteractable;
                if (interactable != null) {
                    // TODO: Highlight object.
                }
            }

    }

    protected override bool ShallInteract() {
            return Input.GetButtonDown(InputButton.Interact);
        }

        private void Update() {
            IInteractable interactable = null;

            RaycastHit hit;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, reach,
                                LayerMask.GetMask("Default"))) {
                interactable = hit.transform.GetComponentInParent(typeof(IInteractable)) as IInteractable;
                if (interactable != null) {
                    // TODO: Highlight object.
                }
            }
        }
    }
}