using UnityEngine;

public class PlanetRotation : MonoBehaviour
{

    public PlanetInfo planetInfo;

    private float obliquityToOrbit; //rotation angle //z
    private float rotationPeriod; //y

    void Start()
    {
        //transform.Rotate(new Vector3(0, 0, obliquityToOrbit));
        obliquityToOrbit = planetInfo.obliquityToOrbit;
        rotationPeriod = planetInfo.rotationPeriod;
        transform.Rotate(new Vector3(0, 0, planetInfo.obliquityToOrbit));
    }


    void Update()
    {
        //transform.Rotate(new Vector3(0, (rotationPeriod * (0 - 1)), 0) * Time.deltaTime);
        transform.Rotate(new Vector3(0, (-planetInfo.rotationPeriod), 0) * Time.deltaTime);
    }
}

