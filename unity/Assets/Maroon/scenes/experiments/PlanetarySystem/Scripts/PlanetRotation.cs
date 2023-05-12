using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    public PlanetInfo planetInfo;

    private float obliquityToOrbit; //rotation angle //z
    private float rotationPeriod;   //y


    /*
     *
     */
    void Start()
    {
        obliquityToOrbit = planetInfo.obliquityToOrbit;
        rotationPeriod = planetInfo.rotationPeriod;

        transform.Rotate(new Vector3(0, 0, planetInfo.obliquityToOrbit));
    }


    /*
     *
     */
    void Update()
    {
        transform.Rotate(new Vector3(0, (-planetInfo.rotationPeriod), 0) * Time.deltaTime);
    }
}

