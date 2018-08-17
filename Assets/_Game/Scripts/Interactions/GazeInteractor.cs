using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maroon {
    public class GazeInteractor : MonoBehaviour {
        //    [SerializeField]
        //    private new Camera camera = null;
        //t
        //    [SerializeField]
        //    private float reach = 2;

        //    [SerializeField]
        //    private float interactDuration = 1;

        //    private IInteractable currentInteractable = null;
        //    private float timeUntilInteract = 0;

        //    private void Update() {
        //        var detectedInteractable = DetectInteractable();
        //        if (currentInteractable != null && currentInteractable == detectedInteractable) {
        //            timeUntilInteract -= Time.deltaTime;
        //        }
        //        else {
        //            currentInteractable = detectedInteractable;
        //            timeUntilInteract = interactDuration;
        //        }

        //        if (currentInteractable != null && timeUntilInteract <= 0) {
        //            currentInteractable.Interact();
        //        }
        //    }

        //    private IInteractable DetectInteractable() {
        //        RaycastHit hit;
        //        if (!Physics.Raycast(camera.transform.position,
        //                             camera.transform.forward,
        //                             out hit,
        //                             reach,
        //                             LayerMask.GetMask("Default"))) {
        //            return null;
        //        }

        //        var interactable = hit.transform.GetComponentInParent(typeof(IInteractable)) as IInteractable;
        //        return interactable;
        //    }
    }

}