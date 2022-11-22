//
//Author: Marcel Lohfeyer
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotationAngle : MonoBehaviour
{
    public float planetRotationAngle; // Z

    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(new Vector3(0, 0, planetRotationAngle));
    }
}
