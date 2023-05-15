using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    public PlanetInfo planetInfo;


    /*
     * rotate planets obliquityToOrbit = rotation angle
     * z axis
     */
    void Start()
    {
        transform.Rotate(new Vector3(0, 0, planetInfo.obliquityToOrbit)); 
    }


    /*
     * rotate planet in its rotation period
     * y axis
     */
    void Update()
    {
        transform.Rotate(new Vector3(0, (-planetInfo.rotationPeriod), 0) * Time.deltaTime);
    }
}

