using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maroon {
  public class FixedJointGrabBehaviour : JointGrabBehaviour {

    [Tooltip("Maximum force the Joint can withstand before breaking. " +
             "Setting to `infinity` ensures the Joint is unbreakable.")]
    public float breakForce = 1500f;

    public override void StartGrab(GrabHandle handle) {
      joint = interactable.gameObject.AddComponent<FixedJoint>();
      joint.breakForce = breakForce;
      joint.connectedBody = handle.Rigidbody;
    }
  }
}