using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOnCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        var obj = other.gameObject.GetComponent<IDeleteObject>();
        obj?.OnDeleteObject();
    }
}
