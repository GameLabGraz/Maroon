using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotationSpeed : MonoBehaviour
{
    public float planet_rotation_degree_per_second;


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, planet_rotation_degree_per_second, 0) * Time.deltaTime);
    }

}