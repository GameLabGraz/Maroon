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

    private RaycastHit hit;
    private float grabDistance;

    protected override Interactable DetectInteractable() {
      if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, reach,
          LayerConfig.InteractableLayerMask)) {
        var interactable = GetInteractableFromTransform(hit.transform);

        return interactable;
      }

      return null;
    }

    protected override void Update() {
      base.Update();

      if (!IsGrabbing) {
        grabDistance = hit.distance;
      }

      if (Input.GetButtonDown(InputButton.Use)) {
        StartUse();
      }

      if (Input.GetButtonUp(InputButton.Use)) {
        StopUse();
      }

      if (Input.GetButtonDown(InputButton.Grab)) {
        grabDistance = hit.distance;
        StartGrab();
      }

      if (Input.GetButtonUp(InputButton.Grab)) {
        StopGrab();
      }

      grabHandle.Rigidbody.MovePosition(camera.transform.position + camera.transform.forward * grabDistance);
    }

  }
}