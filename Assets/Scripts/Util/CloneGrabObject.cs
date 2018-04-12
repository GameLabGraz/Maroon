using UnityEngine;
using VRTK;

[RequireComponent(typeof(VRTK_InteractableObject))]
public class CloneGrabObject : MonoBehaviour
{
    private VRTK_InteractableObject interactableObject;

    private void Start()
    {
        interactableObject = GetComponent<VRTK_InteractableObject>();
        interactableObject.InteractableObjectGrabbed += OnGrabbed;
        interactableObject.InteractableObjectUngrabbed += OnUnGrabbed;
        interactableObject.isGrabbable = true;
    }

    private void OnGrabbed (object sender, InteractableObjectEventArgs e)
    {
        interactableObject.ToggleHighlight(false);

        GameObject clone = GameObject.Instantiate(gameObject, gameObject.transform.parent, false);
    }

    private void OnUnGrabbed(object sender, InteractableObjectEventArgs e)
    {
        interactableObject.isKinematic = false;
        Destroy(this);   // Remove Clone Grab from grabbed Object
    }

    private void OnDestroy()
    {
        interactableObject.InteractableObjectGrabbed -= OnGrabbed;
        interactableObject.InteractableObjectUngrabbed -= OnUnGrabbed;
    }
}
