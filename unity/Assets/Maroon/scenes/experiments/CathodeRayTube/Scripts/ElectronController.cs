using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronController : PausableObject, IResetObject
{
    protected override void HandleUpdate()
    {

    }
        
    protected override void HandleFixedUpdate()
    {

    }

    public void ResetObject()
    {
        Destroy(gameObject);
    }
}
