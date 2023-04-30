using UnityEngine;

public enum PlanetInformation
{
    sun_0,
    mercury_1,
    venus_2,
    earth_3,
    mars_4,
    jupiter_5,
    saturn_6,
    uranus_7,
    neptune_8,
    moon_9,
    empty
}

public class PlanetInfo : MonoBehaviour
{
    public PlanetInformation PlanetInformationOf;

    //source: https://nssdc.gsfc.nasa.gov/planetary/factsheet/planetfact_notes.html
    public float mass;
    public float diameter;
    public float density;
    public float gravity;
    public float escapeVelocity;
    public float rotationPeriod;
    public float rotationAngle;
    public float lengthOfDay;
    public float semiMajorAxisDistanceFromSun;
    public float orbitalPeriod;
    public float orbitalVelocity;
    public float orbitalInclination;
    public float orbitalEccentricity;
    public float obliquityToOrbit;
    public float meanTemperature;


    private void Awake()
    {
        Debug.Log("PlanetInfo Awake()");

        switch (PlanetInformationOf)
        {
            case PlanetInformation.sun_0:
                mass                            = 1988500f;
                diameter                        = 1391400f;
                density                         = 1408f;
                gravity                         = 274f;
                escapeVelocity                  = 617.6f;
                rotationPeriod                  = 609.12f;
                rotationAngle                   = 0f;
                lengthOfDay                     = 0f;
                semiMajorAxisDistanceFromSun    = 0f;
                orbitalPeriod                   = 0f;
                orbitalVelocity                 = 19.4f;
                orbitalInclination              = 0f;
                orbitalEccentricity             = 0f;
                obliquityToOrbit                = 7.25f;
                meanTemperature                 = 0f;
                break;

            case PlanetInformation.mercury_1:
                mass                            = 0.33f;
                diameter                        = 4879f;
                density                         = 5429f;
                gravity                         = 3.7f;
                escapeVelocity                  = 4.3f;
                rotationPeriod                  = 1407.6f;
                rotationAngle                   = -0.01f;
                lengthOfDay                     = 4222.6f;
                semiMajorAxisDistanceFromSun    = 57.9f;
                orbitalPeriod                   = 88f;
                orbitalVelocity                 = 47.4f;
                orbitalInclination              = 7f;
                orbitalEccentricity             = 0.206f;
                obliquityToOrbit                = 0.034f;
                meanTemperature                 = 167f;
                break;

            case PlanetInformation.venus_2:
                mass                            = 4.87f;
                diameter                        = 12104f;
                density                         = 5243f;
                gravity                         = 8.9f;
                escapeVelocity                  = 10.4f;
                rotationPeriod                  = -5832.5f;
                rotationAngle                   = 2.64f;
                lengthOfDay                     = 2802f;
                semiMajorAxisDistanceFromSun    = 108.2f;
                orbitalPeriod                   = 224.7f;
                orbitalVelocity                 = 35f;
                orbitalInclination              = 3.4f;
                orbitalEccentricity             = 0.007f;
                obliquityToOrbit                = 177.4f;
                meanTemperature                 = 464f;
                break;

            case PlanetInformation.earth_3:
                mass                            = 5.97f;
                diameter                        = 12756f;
                density                         = 5514f;
                gravity                         = 9.8f;
                escapeVelocity                  = 11.2f;
                rotationPeriod                  = -23.9f;
                rotationAngle                   = 23.44f;
                lengthOfDay                     = 24f;
                semiMajorAxisDistanceFromSun    = 149.6f;
                orbitalPeriod                   = 365.2f;
                orbitalVelocity                 = 29.8f;
                orbitalInclination              = 0f;
                orbitalEccentricity             = 0.017f;
                obliquityToOrbit                = 23.4f;
                meanTemperature                 = 15f;
                break;

            case PlanetInformation.mars_4:
                mass                            = 0.642f;
                diameter                        = 6792f;
                density                         = 3934f;
                gravity                         = 3.7f;
                escapeVelocity                  = 5f;
                rotationPeriod                  = 24.6f;
                rotationAngle                   = -25.19f;
                lengthOfDay                     = 24.7f;
                semiMajorAxisDistanceFromSun    = 228f;
                orbitalPeriod                   = 687f;
                orbitalVelocity                 = 24.1f;
                orbitalInclination              = 1.8f;
                orbitalEccentricity             = 0.094f;
                obliquityToOrbit                = 25.2f;
                meanTemperature                 = -65f;
                break;
            case PlanetInformation.jupiter_5:
                mass                            = 1898f;
                diameter                        = 142984f;
                density                         = 1326f;
                gravity                         = 23.1f;
                escapeVelocity                  = 59.5f;
                rotationPeriod                  = 9.9f;
                rotationAngle                   = -3.12f;
                lengthOfDay                     = 9.9f;
                semiMajorAxisDistanceFromSun    = 778.5f;
                orbitalPeriod                   = 4331;
                orbitalVelocity                 = 13.1f;
                orbitalInclination              = 1.3f;
                orbitalEccentricity             = 0.049f;
                obliquityToOrbit                = 3.1f;
                meanTemperature                 = -110f;
                break;

            case PlanetInformation.saturn_6:
                mass                            = 568f;
                diameter                        = 120536f;
                density                         = 687f;
                gravity                         = 9f;
                escapeVelocity                  = 35.5f;
                rotationPeriod                  = 10.7f;
                rotationAngle                   = -26.73f;
                lengthOfDay                     = 10.7f;
                semiMajorAxisDistanceFromSun    = 1432f;
                orbitalPeriod                   = 10747f;
                orbitalVelocity                 = 9.7f;
                orbitalInclination              = 2.5f;
                orbitalEccentricity             = 0.052f;
                obliquityToOrbit                = 26.7f;
                meanTemperature                 = -140f;
                break;

            case PlanetInformation.uranus_7:
                mass                            = 86.8f;
                diameter                        = 51118f;
                density                         = 1270f;
                gravity                         = 8.7f;
                escapeVelocity                  = 21.3f;
                rotationPeriod                  = 17.2f;
                rotationAngle                   = -82.23f;
                lengthOfDay                     = 17.2f;
                semiMajorAxisDistanceFromSun    = 2867f;
                orbitalPeriod                   = 30589f;
                orbitalVelocity                 = 6.8f;
                orbitalInclination              = 0.8f;
                orbitalEccentricity             = 0.047f;
                obliquityToOrbit                = 97.8f;
                meanTemperature                 = -195f;
                break;

            case PlanetInformation.neptune_8:
                mass                            = 102f;
                diameter                        = 49528f;
                density                         = 1638f;
                gravity                         = 11f;
                escapeVelocity                  = 23.5f;
                rotationPeriod                  = 16.1f;
                rotationAngle                   = -28.33f;
                lengthOfDay                     = 16.1f;
                semiMajorAxisDistanceFromSun    = 4515f;
                orbitalPeriod                   = 59.8f;
                orbitalVelocity                 = 5.4f;
                orbitalInclination              = 1.8f;
                orbitalEccentricity             = 0.01f;
                obliquityToOrbit                = 28.3f;
                meanTemperature                 = -200f;
                break;

            case PlanetInformation.moon_9:
                mass                            = 0.073f;
                diameter                        = 3475f;
                density                         = 3340f;
                gravity                         = 1.6f;
                escapeVelocity                  = 2.4f;
                rotationPeriod                  = 655.7f;
                rotationAngle                   = -1.54f;
                lengthOfDay                     = 708.7f;
                semiMajorAxisDistanceFromSun    = 0.384f;
                orbitalPeriod                   = 27.3f;
                orbitalVelocity                 = 1f;
                orbitalInclination              = 5.1f;
                orbitalEccentricity             = 0.055f;
                obliquityToOrbit                = 6.7f;
                meanTemperature                 = -20f;
                break;

            default:
                mass                            = 0f;
                diameter                        = 0f;
                density                         = 0f;
                gravity                         = 0f;
                escapeVelocity                  = 0f;
                rotationPeriod                  = 0f;
                rotationAngle                   = 0f;
                lengthOfDay                     = 0f;
                semiMajorAxisDistanceFromSun    = 0f;
                orbitalPeriod                   = 0f;
                orbitalVelocity                 = 0f;
                orbitalInclination              = 0f;
                orbitalEccentricity             = 0f;
                obliquityToOrbit                = 0f;
                meanTemperature                 = 0f;
                break;

        }
    }

}
