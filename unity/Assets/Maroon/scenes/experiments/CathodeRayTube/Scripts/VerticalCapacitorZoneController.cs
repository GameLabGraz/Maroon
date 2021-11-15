using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalCapacitorZoneController : MonoBehaviour
{
    public float horizontalDeflect;
    private void OnTriggerEnter(Collider zoneCollision)
    {
        zoneCollision.gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0,horizontalDeflect));
    }
}
