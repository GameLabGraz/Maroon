using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    //public GameObject sun;
    //public float gravitationalConstantG = 6.6743e-11f;
    public float gravitationalConstantG = 100f;
    // public SimulationController simulationController;
    //gravityMultiplier
    public Rigidbody[] celestials;

    //private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        celestials = FindObjectsOfType<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        ApplyGravity();
    }

    // Newton's law of universal gravitation
    // F = G * ((m1 * m2) / (r^2))
    /*void ApplyGravity2()
    {
        Vector3 directionToSun = sun.transform.position - transform.position;
        float distanceToSun = directionToSun.magnitude; //r

        // Calculate the gravitational force using Newton's law of universal gravitation
        float forceMagnitudeF = gravitationalConstantG * 
            //simulationController.gravityMultiplier *
            (rb.mass * sun.GetComponent<Rigidbody>().mass) / (distanceToSun * distanceToSun);

        Vector3 force = directionToSun.normalized * forceMagnitudeF;
        rb.AddForce(force);
    }
    */

    // Newton's law of universal gravitation
    // F = G * ((m1 * m2) / (r^2))
    void ApplyGravity()
    {
        float minDistanceThreshold = 0.001f;
        foreach (Rigidbody body1 in celestials)
        {
            foreach (Rigidbody body2 in celestials)
            {
                if (body1 == body2) continue;
                Vector3 direction = body1.position - body2.position;
                float distance = direction.magnitude;
                //
                if (distance < minDistanceThreshold) continue;

                float forceMagnitudeF = gravitationalConstantG * (body1.mass * body2.mass) / Mathf.Pow(distance, 2);
                Vector3 force = direction.normalized * forceMagnitudeF;
                body2.AddForce(force);
            }
        }
    }



}
