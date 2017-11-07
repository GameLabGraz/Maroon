using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MoveController : VRTK_InteractableObject
{
    [SerializeField]
    protected Vector3 MoveDirection = Vector3.right;

    [SerializeField]
    private Vector3 MinPosition;

    [SerializeField]
    private Vector3 MaxPosition;

    [SerializeField]
    private float MoveSpeedFactor = 0.25f;

    protected bool IsMoving = false;

    protected MonoBehaviour UsingObject;

    protected Vector3 UsingObjectPosition;

    protected Vector3 UsingObjectPositionOld;

    public override void StartUsing(GameObject currentUsingObject)
    {
        base.StartUsing(currentUsingObject);

        UsingObject = usingObject;
        UsingObjectPosition = usingObject.transform.position;

        StartCoroutine(Move());
    }

    public override void StopUsing(GameObject previousUsingObject)
    {
        base.StopUsing(previousUsingObject);

        IsMoving = false;
    }

    private IEnumerator Move()
    {
        while (IsMoving)
        {
            /*

            Vector3 newPosition = this.transform.position - Vector3.Scale(this.transform.position, MoveDirection);
            newPosition += Vector3.Scale(UsingObject.transform.position, MoveDirection);

            this.transform.position = newPosition;
            */

            UsingObjectPositionOld = UsingObjectPosition;
            UsingObjectPosition = usingObject.transform.position;

            Vector3 controllerMoveDirection = transform.InverseTransformDirection(UsingObjectPositionOld - UsingObjectPosition);

            float moveDistance = controllerMoveDirection.magnitude;

            if (Vector3.Dot(MoveDirection, controllerMoveDirection) > 0)
                this.transform.localPosition += MoveDirection * moveDistance * MoveSpeedFactor;
            else
                this.transform.localPosition -= MoveDirection * moveDistance * MoveSpeedFactor;


            if (Vector3.Distance(MinPosition, MaxPosition) < Vector3.Distance(MinPosition, this.transform.localPosition))
                this.transform.localPosition = MaxPosition;

            if (Vector3.Distance(MinPosition, MaxPosition) < Vector3.Distance(MaxPosition, this.transform.localPosition))
                this.transform.localPosition = MinPosition;


            yield return new WaitForFixedUpdate();
        }
    }

}
