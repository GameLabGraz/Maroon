//-----------------------------------------------------------------------------
// MagnetController.cs
//
// Controller class for the magnet
//
//
// Authors: Michael Stefan Holly
//-----------------------------------------------------------------------------
//

using System.Collections;
using UnityEngine;
using VRTK;

/// <summary>
/// Controller class for the magnet
/// </summary>
public class MagnetController : VRTK_InteractableObject
{
    private bool IsMoving = false;

    private GameObject UsingObject;

    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);

        IsMoving = true;
        UsingObject = usingObject;

        StartCoroutine(Move());
    }

    public override void StopUsing(GameObject usingObject)
    {
        base.StopUsing(usingObject);

        IsMoving = false;
    }

    private IEnumerator Move()
    {
        while(IsMoving)
        {
            this.transform.position = new Vector3(UsingObject.transform.position.x, this.transform.position.y, this.transform.position.z);

            UsingObject.GetComponent<VRTK_ControllerActions>().TriggerHapticPulse((ushort)(GetComponent<Magnet>().getExternalForce().magnitude * 100));

            yield return new WaitForFixedUpdate();
        }
    }
}
