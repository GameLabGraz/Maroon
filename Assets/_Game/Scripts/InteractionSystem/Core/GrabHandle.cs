using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maroon {
  public class GrabHandle : MonoBehaviour {

    [SerializeField]
    private new Rigidbody rigidbody;

    public Rigidbody Rigidbody {
      get { return rigidbody; }
    }
  }
}