using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalCapacitorZoneController : MonoBehaviour
{
    public float verticalDeflect;
    private void OnTriggerEnter(Collider zoneCollision)
    {
        zoneCollision.gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(verticalDeflect, 0,0));
    }
}

