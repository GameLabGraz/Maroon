using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maroon
{
  public abstract class GrabAttach : MonoBehaviour
  {

    [SerializeField]
    protected Interactable interactable;

    protected GrabHandle handle;

    public virtual void StartGrab(GrabHandle handle)
    {
      this.handle = handle;
    }

    public virtual void StopGrab()
    {
      this.handle = null;
    }
  }
}