using System.Collections;
using UnityEngine;
using VRTK;

public class CapacitorPlateResizeController : VRTK_InteractableObject
{
    private CapacitorPlateController capacitorPlate;

    private Vector3 resizeAxis = Vector3.zero;

    private float maxSize = 0;

    private bool isResizing = false;

    private GameObject UsingObject;


    public void setCapacitorPlate(CapacitorPlateController capacitorPlate)
    {
        this.capacitorPlate = capacitorPlate;
    }

    public void setResizeAxsis(Vector3 resizeAxis)
    {
        this.resizeAxis = resizeAxis;
    }

    public void setMaxSize(float maxSize)
    {
        this.maxSize = maxSize;
    }

    public override void StartTouching(VRTK_InteractTouch currentTouchingObject = null)
    {
        base.StartTouching(currentTouchingObject);

        if (capacitorPlate == null)
            return;

        capacitorPlate.EnableResizeObjects(true);
        capacitorPlate.ToggleHighlight(true);

        // Ignore controller collisions with capacitor plate to not interrupt resizing
        foreach (Collider usingCollider in currentTouchingObject.GetComponentsInChildren<Collider>())
            Physics.IgnoreCollision(capacitorPlate.GetComponent<Collider>(), usingCollider, true);
    }

    public override void StopTouching(VRTK_InteractTouch previousTouchingObject = null)
    {
        base.StopTouching(previousTouchingObject);

        if (capacitorPlate == null)
            return;

        capacitorPlate.ToggleHighlight(false);
        capacitorPlate.EnableResizeObjects(false);

        // Reset ignore controller collision with capacitor plate to enable plate touching
        foreach (Collider usingCollider in previousTouchingObject.GetComponentsInChildren<Collider>())
            Physics.IgnoreCollision(capacitorPlate.GetComponent<Collider>(), usingCollider, false);
    }

    public override void StartUsing(VRTK_InteractUse currentUsingObject = null)
    {
        base.StartUsing(currentUsingObject);

        UsingObject = usingObject.gameObject;

        if (capacitorPlate == null)
            return;

        isResizing = true;

        StartCoroutine(Resize());
    }

    public override void StopUsing(VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true)
    {
        base.StopUsing(previousUsingObject, resetUsingObjectState);

        isResizing = false;
    }

    private IEnumerator Resize()
    {
        while(isResizing)
        {
            Vector3 size = capacitorPlate.transform.localScale;
            size = Vector3.Scale(size, (resizeAxis - Vector3.one) * -1);

            Vector3 pos1 = Vector3.Scale(capacitorPlate.transform.position, resizeAxis);
            Vector3 pos2 = Vector3.Scale(UsingObject.transform.position, resizeAxis);

            size += Vector3.ClampMagnitude(resizeAxis * Vector3.Distance(pos1, pos2) * 2, maxSize);
            capacitorPlate.transform.localScale = size;

            yield return new WaitForFixedUpdate();
        }
    }

}
