using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Maroon {
  public class Interactable : MonoBehaviour {

    [Header("Use")]
    [SerializeField]
    protected bool toggleUse = false;

    [Header("Grab")]
    [SerializeField]
    [Tooltip("Can this interactable be grabbed?")]
    protected bool grabbable = false;

    [SerializeField]
    [Tooltip("The grab behaviour of the interactable.")]
    [FormerlySerializedAs("grabAttach")]
    protected GrabBehaviour grabBehaviour;

    protected bool inUse;
    protected bool inGrab;

    public virtual void StartUse() {
      if (toggleUse) {
        inUse = !inUse;
      } else {
        inUse = true;
      }
    }

    public virtual void StopUse() {
      if (!toggleUse) {
        inUse = false;
      }
    }

    protected virtual void Use() { }


    public virtual void StartGrab(GrabHandle handle) {
      if (grabbable) {
        grabBehaviour.StartGrab(handle);
      }
    }

    public virtual void StopGrab() {
      if (grabbable) {
        grabBehaviour.StopGrab();
      }
    }

    protected virtual void Awake() {
      if (!grabBehaviour) {
        grabBehaviour = GetComponentInChildren<GrabBehaviour>();
      }

      inUse = false;
      inGrab = false;
    }

    protected virtual void Update() {
      if (inUse) {
        Use();
      }
    }
  }
}