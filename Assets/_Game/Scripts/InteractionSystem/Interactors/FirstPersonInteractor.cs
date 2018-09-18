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

    private float detectedInteractableDistance;

    private void Update() {
      detectedInteractable = DetectInteractable();

      grabHandle.transform.position = camera.transform.position +
          camera.transform.forward * detectedInteractableDistance;

      if (Input.GetButtonDown(InputButton.Use)) {
        StartUse();
      }

      if (Input.GetButtonUp(InputButton.Use)) {
        StopUse();
      }

      if (Input.GetButtonDown(InputButton.Grab)) {
        StartGrab();
      }

      if (Input.GetButtonUp(InputButton.Grab)) {
        StopGrab();
      }
    }

    private Interactable DetectInteractable() {

      if (IsUsing || IsGrabbing) {
        return detectedInteractable;
      }

      RaycastHit hit;
      if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, reach,
          LayerConfig.InteractableLayerMask)) {

        detectedInteractableDistance = hit.distance;
        return GetInteractableFromTransform(hit.transform);
      }

      return null;
    }
  }
}