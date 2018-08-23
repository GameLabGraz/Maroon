using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
        protected GrabAttach grabAttach;

        protected bool inUse;
        protected bool inGrab;

        public virtual void StartUse() {
            if (toggleUse) {
                inUse = !inUse;
            }
            else {
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
                grabAttach.StartGrab(handle);
            }
        }

        public virtual void StopGrab() {
            if (grabbable) {
                grabAttach.StopGrab();
            }
        }

        protected virtual void Awake() {
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