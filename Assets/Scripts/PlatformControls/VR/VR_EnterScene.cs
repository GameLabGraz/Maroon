using PlatformControls.BaseControls;
using UnityEngine;
using VRTK;

namespace PlatformControls.VR
{
    [RequireComponent(typeof(VRTK_InteractableObject))]
    public class VR_EnterScene : EnterScene
    {
        private void Start ()
        {
            var interactableObject = GetComponent<VRTK_InteractableObject>();
            interactableObject.isUsable = true;
            interactableObject.InteractableObjectUsed += (sender, e) => Enter();
        }
    }
}
