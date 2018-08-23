using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Maroon {
    public abstract class JointGrabAttach : GrabAttach {

        protected Joint joint;

        public override void StopGrab() {
            // Joints can be null at this point because joints can break due to exceeding the break force.
            if (joint != null) {
                GameObject.Destroy(joint);
            }
        }
    }
}