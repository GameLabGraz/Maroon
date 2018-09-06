using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using VRTK;

namespace Maroon {
  public class VRInteractor : Interactor {

    [SerializeField]
    private VRTK_ControllerEvents controllerEvents;

    [SerializeField]
    private VRTK_ControllerEvents.ButtonAlias useButton;

    [SerializeField]
    private VRTK_ControllerEvents.ButtonAlias grabButton;

    private Interactable triggerInteractable;
    private Collider triggerCollider;


    private void Awake() {
      if (!controllerEvents) {
        controllerEvents = GetComponent<VRTK_ControllerEvents>();
      }

      Assert.IsNotNull(controllerEvents, "VRTK ControllerEvents have to be set on VRTK Interactors!");

      controllerEvents.SubscribeToButtonAliasEvent(useButton, true, OnUseButtonDown);
      controllerEvents.SubscribeToButtonAliasEvent(useButton, false, OnUseButtonUp);
      controllerEvents.SubscribeToButtonAliasEvent(grabButton, true, OnGrabButtonDown);
      controllerEvents.SubscribeToButtonAliasEvent(grabButton, false, OnGrabButtonUp);
    }

    private void OnUseButtonDown(object sender, ControllerInteractionEventArgs e) {
      StartUse();
    }

    private void OnUseButtonUp(object sender, ControllerInteractionEventArgs e) {
      StopUse();
    }

    private void OnGrabButtonDown(object sender, ControllerInteractionEventArgs e) {
      StartGrab();
    }

    private void OnGrabButtonUp(object sender, ControllerInteractionEventArgs e) {
      StopGrab();
    }

    protected override Interactable DetectInteractable() {
      return triggerInteractable;
    }

    private void OnTriggerEnter(Collider other) {
      if (triggerInteractable) {
        return;
      }

      triggerInteractable = GetInteractableFromTransform(other.transform);
      triggerCollider = other;
    }

    private void OnTriggerExit(Collider other) {
      if (!triggerInteractable) {
        return;
      }

      if (other == triggerCollider) {
        triggerCollider = null;
        triggerInteractable = null;
      }
    }
  }
}