using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Maroon {
  public abstract class Interactor : MonoBehaviour {

    [SerializeField]
    protected GrabHandle grabHandle;

    protected Interactable detectedInteractable;
    protected Interactable usedInteractable;
    protected Interactable grabbedInteractable;

    public GrabHandle GrabHandle {
      get { return grabHandle; }
    }

    protected Interactable GetInteractableFromTransform(Transform t) {
      return t.GetComponent<Interactable>();
    }


    protected void StartUse() {
      if (detectedInteractable) {
        usedInteractable = detectedInteractable;
        usedInteractable.StartUse();
      }
    }

    protected void StopUse() {
      if (usedInteractable) {
        usedInteractable.StopUse();
        usedInteractable = null;
      }
    }

    public bool IsUsing {
      get { return usedInteractable != null; }
    }

    protected void StartGrab() {
      if (detectedInteractable) {
        grabbedInteractable = detectedInteractable;
        grabbedInteractable.StartGrab(grabHandle);
      }
    }

    protected void StopGrab() {
      if (grabbedInteractable) {
        grabbedInteractable.StopGrab();
        grabbedInteractable = null;
      }
    }

    public bool IsGrabbing {
      get { return grabbedInteractable != null; }
    }
  }
}