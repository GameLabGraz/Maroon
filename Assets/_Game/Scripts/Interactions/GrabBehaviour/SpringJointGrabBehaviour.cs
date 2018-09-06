using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maroon {
  public class SpringJointGrabBehaviour : JointGrabBehaviour {

    [Tooltip("Maximum force the Joint can withstand before breaking. " +
             "Setting to `infinity` ensures the Joint is unbreakable.")]
    public float breakForce = 1500f;

    [Tooltip("The strength of the spring.")]
    public float strength = 500f;

    [Tooltip("The amount of dampening to apply to the spring.")]
    public float damper = 50f;

    public override void StartGrab(GrabHandle handle) {
      joint = interactable.gameObject.AddComponent<SpringJoint>();
      joint.breakForce = breakForce;
      ((SpringJoint)joint).spring = strength;
      ((SpringJoint)joint).damper = damper;
      joint.connectedBody = handle.Rigidbody;
    }
  }
}