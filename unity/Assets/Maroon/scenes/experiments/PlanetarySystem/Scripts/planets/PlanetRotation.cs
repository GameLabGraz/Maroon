using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    public float planetRotationAngle; // z-achsis
    public float planetRotationSpeed;

    void Start()
    {
        planetRotationAngle = 50f;
        transform.Rotate(new Vector3(0, 0, planetRotationAngle));
    }


    void Update()
    {
        planetRotationSpeed = 50f;
        transform.Rotate(new Vector3(0, (planetRotationSpeed * (0 - 1)), 0) * Time.deltaTime);
    }
}

