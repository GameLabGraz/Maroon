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

    public bool IsGrabbing {
      get { return grabbedInteractable != null; }
    }

    public bool IsUsing {
      get { return usedInteractable != null; }
    }


    public GrabHandle GrabHandle {
      get { return grabHandle; }
    }

    protected abstract Interactable DetectInteractable();

    protected virtual void Update() {
      detectedInteractable = DetectInteractable();
    }

    protected void StartUse() {
      if (detectedInteractable) {
        usedInteractable = detectedInteractable;
        detectedInteractable.StartUse();
      }
    }

    protected void StopUse() {
      if (usedInteractable) {
        usedInteractable.StopUse();
        usedInteractable = null;
      }
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

    protected Interactable GetInteractableFromTransform(Transform t) {
      return t.GetComponent<Interactable>();
    }
  }
}