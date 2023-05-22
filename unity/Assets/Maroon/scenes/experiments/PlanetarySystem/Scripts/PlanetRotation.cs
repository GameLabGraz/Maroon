using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    public PlanetInfo planetInfo;

    void Start()
    {
        SetObliquityToOrbit();
    }


    void Update()
    {
        RotatePlanets();
    }


    /*
     * rotate planets obliquityToOrbit = rotation angle
     * calles after resetting Animation
     * z axis
     */
    public void SetObliquityToOrbit()
    {
        //Debug.Log("PlanetRotation: SetObliquityToOrbit(): called");
        transform.Rotate(new Vector3(0, 0, planetInfo.obliquityToOrbit));
    }


    /*
     * rotate planet in its rotation period
     * y axis
     */
    void RotatePlanets()
    {
        transform.Rotate(new Vector3(0, (-planetInfo.rotationPeriod), 0) * Time.deltaTime);
    }
}

