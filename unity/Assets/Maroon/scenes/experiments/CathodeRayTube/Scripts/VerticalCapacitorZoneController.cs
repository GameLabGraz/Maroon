using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics.CathodeRayTube
{
    public class VerticalCapacitorZoneController : MonoBehaviour
    {
        [SerializeField] private float horizontalDeflect;

        private void OnTriggerEnter(Collider zoneCollision)
        {
            zoneCollision.gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, horizontalDeflect));
        }
    }
}
