using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitMotion2 : MonoBehaviour
{
    public Transform orbitCenter;
    public float semiMajorAxis;
    [SerializeField, Range(0, 1)] public float eccentricity = 0f;
    public float orbitalPeriod;

    private float theta;
    private float angularSpeed;

    void Start()
    {
        theta = 0;
        angularSpeed = Mathf.PI * 2 / orbitalPeriod;
    }

    /*
     Use Kepler's laws of planetary motion to create elliptical orbits. You can use the following formula to calculate the position of a celestial body on an elliptical orbit:
     r = a * (1 - e^2) / (1 + e * cos(theta))
        Where:
        r is the distance between the center of the orbit and the celestial body
        a is the semi-major axis
        e is the eccentricity of the ellipse
        theta is the angle between the perihelion and the celestial body
     */
    void FixedUpdate()
    {
        theta += angularSpeed * Time.fixedDeltaTime;
        float r = semiMajorAxis * (1 - Mathf.Pow(eccentricity, 2)) / (1 + eccentricity * Mathf.Cos(theta));
        Vector3 newPosition = new Vector3(r * Mathf.Cos(theta), 0, r * Mathf.Sin(theta));
        transform.position = orbitCenter.position + newPosition;
    }
}