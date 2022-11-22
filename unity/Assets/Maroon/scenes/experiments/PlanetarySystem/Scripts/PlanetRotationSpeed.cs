//
//Author: Marcel Lohfeyer
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotationSpeed : MonoBehaviour
{
    public float rotationSpeed;

    void Update()
    {
        transform.Rotate(new Vector3(0, (rotationSpeed * (0 - 1)), 0) * Time.deltaTime);
    }

}