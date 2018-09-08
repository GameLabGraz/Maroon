using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon
{
  public static class LayerConfig
  {
    public const string InteractableLayerName = "Interactable";

    public static int InteractableLayerMask {
      get { return LayerMask.GetMask(InteractableLayerName); }
    }


  }
}