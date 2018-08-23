using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maroon {
    public abstract class GrabAttach : MonoBehaviour {

        [SerializeField]
        protected Interactable interactable;

        public abstract void StartGrab(GrabHandle handle);
        public abstract void StopGrab();
    }
}