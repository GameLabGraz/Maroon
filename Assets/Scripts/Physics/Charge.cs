using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour
{
    [SerializeField]
    private float chargeValue;

    [SerializeField]
    private float mass;

    [SerializeField]
    private bool justCreated = true;

    public bool JustCreated
    {
        get { return justCreated; }
        set { justCreated = value; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("ElectricField"))
            return;

        Vector3 force = Vector3.zero;
        GetComponent<Rigidbody>().AddForce(force);
    }

}
