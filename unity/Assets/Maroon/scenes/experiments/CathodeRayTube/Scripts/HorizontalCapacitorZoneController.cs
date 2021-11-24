using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics.CathodeRayTube
{
    public class HorizontalCapacitorZoneController : MonoBehaviour
    {
        [SerializeField] private QuantityFloat verticalVoltage;

        private void OnTriggerEnter(Collider zoneCollision)
        {
            zoneCollision.gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(verticalVoltage, 0, 0));
        }
    }
}

