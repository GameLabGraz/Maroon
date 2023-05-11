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
    pluto_10,
    empty
}

public class PlanetInfo : MonoBehaviour
{
    private static PlanetInfo _instance;


    public static PlanetInfo Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PlanetInfo>();
            return _instance;
        }
    }


    public PlanetInformation PlanetInformationOf;
    //source: https://nssdc.gsfc.nasa.gov/planetary/factsheet/planetfact_notes.html

    /*
     * Mass (10^24kg) - This is the mass of the planet in septillion (1 followed by 24 zeros) kilograms.
     */
    public float mass;
    /*
     * Diameter (km) - The diameter of the planet at the equator,
     * the distance through the center of the planet from one point on the equator to the opposite side, in kilometers.
     */
    public float diameter;
    /*
     * Density (kg/m3) - The average density (mass divided by volume) of the whole planet (not including the atmosphere for the terrestrial planets)
     * in kilograms per cubic meter. Strictly speaking pounds are measures of weight, not mass,
     * but are used here to represent the mass of one pound of material under Earth gravity.
     */
    public float density;
    /*
     * Gravity (m/s^2) - The gravitational acceleration on the surface at the equator in meters per second squared,
     * including the effects of rotation. For the gas giant planets the gravity is given at the 1 bar pressure level in the atmosphere.
     * The gravity on Earth is designated as 1 "G", so the Earth ratio fact sheets gives the gravity of the other planets in G's.
     */
    public float gravity;
    /*
     * Escape Velocity (km/s) - Initial velocity, in kilometers per second, needed at the surface 
     * (at the 1 bar pressure level for the gas giants) to escape the body's gravitational pull, ignoring atmospheric drag.
     */
    public float escapeVelocity;
    /*
     * Rotation Period(hours) - This is the time it takes for the planet to complete one rotation
     * relative to the fixed background stars (not relative to the Sun) in hours.
     * Negative numbers indicate retrograde(backwards relative to the Earth) rotation.
     */
    public float rotationPeriod;
    /*
     * Length of Day (hours) - The average time in hours for the Sun to move from the noon position in the sky at a point on the equator back to the same position.
     */
    public float lengthOfDay;
    /*
     * Distance from Sun (10^6 km) - This is the average distance from the planet to the Sun in millions of kilometers, also known as the semi-major axis.
     * All planets have orbits which are elliptical, not perfectly circular,
     * so there is a point in the orbit at which the planet is closest to the Sun, the perihelion,
     * and a point furthest from the Sun, the aphelion.
     * The average distance from the Sun is midway between these two values.
     * The average distance from the Earth to the Sun is defined as 1 Astronomical Unit (AU),
     * so the ratio table gives this distance in AU.
     * * For the Moon, the average distance from the Earth is given.
     */
    public float distanceFromSun; //semiMayorAxis
    /*
     * Perihelion, Aphelion (10^6 km) - The closest and furthest points in a planet's orbit about the Sun, see "Distance from Sun" above.
     * * For the Moon, the closest and furthest points to Earth are given, known as the "Perigee" and "Apogee" respectively.
     */
    public float perihelion;
    public float aphelion;
    /*
     * Orbital Period (days) - This is the time in Earth days for a planet to orbit the Sun from one vernal equinox to the next. 
     * Also known as the tropical orbit period, this is equal to a year on Earth.
     * For the Moon, the sidereal orbit period, the time to orbit once relative to the fixed background stars, is given.
     * The time from full Moon to full Moon, or synodic period, is 29.53 days
     */
    public float orbitalPeriod;
    /*
     * Orbital Velocity (km/s or miles/s) - The average velocity or speed of the planet as it orbits the Sun, in kilometers per second or miles per second.
     * For the Moon, the average velocity around the Earth is given.
     */
    public float orbitalVelocity;
    /*
     * Orbital Inclination (degrees) - The angle in degrees at which a planets orbit around the Sun is tilted relative to the ecliptic plane.
     * The ecliptic plane is defined as the plane containing the Earth's orbit, so the Earth's inclination is 0.
     */
    public float orbitalInclination;
    /*
     * Orbital Eccentricity - This is a measure of how far a planet's orbit about the Sun (or the Moon's orbit about the Earth) is from being circular.
     * The larger the eccentricity, the more elongated is the orbit, an eccentricity of 0 means the orbit is a perfect circle.
     * There are no units for eccentricity.
     */
    public float orbitalEccentricity;
    /*
     * Obliquity to Orbit (degrees) - The angle in degrees the axis of a planet 
     * (the imaginary line running through the center of the planet from the north to south poles)
     * is tilted relative to a line perpendicular to the planet's orbit around the Sun, north pole defined by right hand rule.
     * *Venus rotates in a retrograde direction, opposite the other planets, so the tilt is almost 180 degrees,
     * it is considered to be spinning with its "top", or north pole pointing "downward" (southward).
     * Uranus rotates almost on its side relative to the orbit, Pluto is pointing slightly "down".
     * The ratios with Earth refer to the axis without reference to north or south. 
     */
    public float obliquityToOrbit; //rotation angle
    /*
     *  Mean Temperature (C) - This is the average temperature over the whole planet's surface (or for the gas giants at the one bar level)
     *  in degrees C (Celsius or Centigrade).
     */
    public float meanTemperature;
    /*
     * Surface Pressure (bars) - This is the atmospheric pressure (the weight of the atmosphere per unit area) at the surface of the planet in bars.
     * *The surfaces of Jupiter, Saturn, Uranus, and Neptune are deep in the atmosphere and the location and pressures are not known.
     */
    public float surfacePressure;
    /*
     * Number of Moons - This gives the number of IAU officially confirmed moons orbiting the planet. New moons are still being discovered.
     */
    public int numberOfMoons;
    /*
     * Ring System? - This tells whether a planet has a set of rings around it, Saturn being the most obvious example.
     */
    public bool ringSystem;
    /*
     * Global Magnetic Field? - This tells whether the planet has a measurable large-scale magnetic field.
     * Mars and the Moon have localized regional magnetic fields but no global field.
     */
    public bool globalMagneticField;


    private void Awake()
    {
        //Debug.Log("PlanetInfo Awake()");
        switch (PlanetInformationOf)
        {
            case PlanetInformation.sun_0:
                mass                            = 1988500f;
                diameter                        = 1408f; 
                density                         = 1408f;
                gravity                         = 274f;
                escapeVelocity                  = 617.6f;
                rotationPeriod                  = 609.12f;
                lengthOfDay                     = 0f;
                distanceFromSun                 = 0f;
                perihelion                      = 0f;
                aphelion                        = 0f;
                orbitalPeriod                   = 0f;
                orbitalVelocity                 = 19.4f;
                orbitalInclination              = 0f;
                orbitalEccentricity             = 0f;
                obliquityToOrbit                = 7.25f;
                meanTemperature                 = 5499f;
                surfacePressure                 = 2.477e+14f;
                numberOfMoons                   = 0;
                ringSystem                      = false;
                globalMagneticField             = true;
                break;

            case PlanetInformation.mercury_1:
                mass                            = 0.33f;
                diameter                        = 4879f;
                density                         = 5429f;
                gravity                         = 3.7f;
                escapeVelocity                  = 4.3f;
                rotationPeriod                  = 1407.6f;
                lengthOfDay                     = 4222.6f;
                distanceFromSun                 = 57.9f;
                perihelion                      = 46f;
                aphelion                        = 69.9f;
                orbitalPeriod                   = 88f;
                orbitalVelocity                 = 47.4f;
                orbitalInclination              = 7f;
                orbitalEccentricity             = 0.206f;
                obliquityToOrbit                = 0.034f;
                meanTemperature                 = 167f;
                surfacePressure                 = 0f;
                numberOfMoons                   = 0;
                ringSystem                      = false;
                globalMagneticField             = true;
                break;

            case PlanetInformation.venus_2:
                mass                            = 4.87f;
                diameter                        = 12104f;
                density                         = 5243f;
                gravity                         = 8.9f;
                escapeVelocity                  = 10.4f;
                rotationPeriod                  = -5832.5f;
                lengthOfDay                     = 2802f;
                distanceFromSun                 = 108.2f;
                perihelion                      = 107.5f;
                aphelion                        = 108.9f;
                orbitalPeriod                   = 224.7f;
                orbitalVelocity                 = 35f;
                orbitalInclination              = 3.4f;
                orbitalEccentricity             = 0.007f;
                obliquityToOrbit                = 177.4f;
                meanTemperature                 = 464f;
                surfacePressure                 = 92f;
                numberOfMoons                   = 0;
                ringSystem                      = false;
                globalMagneticField             = false;
                break;

            case PlanetInformation.earth_3:
                mass                            = 5.97f;
                diameter                        = 12756f;
                density                         = 5514f;
                gravity                         = 9.8f;
                escapeVelocity                  = 11.2f;
                rotationPeriod                  = 23.9f;
                lengthOfDay                     = 24f;
                distanceFromSun                 = 149.6f;
                perihelion                      = 147.1f;
                aphelion                        = 152.1f;
                orbitalPeriod                   = 365.2f;
                orbitalVelocity                 = 29.8f;
                orbitalInclination              = 0f;
                orbitalEccentricity             = 0.017f;
                obliquityToOrbit                = 23.4f;
                meanTemperature                 = 15f;
                surfacePressure                 = 1f;
                numberOfMoons                   = 1;
                ringSystem                      = false;
                globalMagneticField             = true;
                break;

            case PlanetInformation.mars_4:
                mass                            = 0.642f;
                diameter                        = 6792f;
                density                         = 3934f;
                gravity                         = 3.7f;
                escapeVelocity                  = 5f;
                rotationPeriod                  = 24.6f;
                lengthOfDay                     = 24.7f;
                distanceFromSun                 = 228f;
                perihelion                      = 206.7f;
                aphelion                        = 249.3f;
                orbitalPeriod                   = 687f;
                orbitalVelocity                 = 24.1f;
                orbitalInclination              = 1.8f;
                orbitalEccentricity             = 0.094f;
                obliquityToOrbit                = 25.2f;
                meanTemperature                 = -65f;
                surfacePressure                 = 0.01f;
                numberOfMoons                   = 2;
                ringSystem                      = false;
                globalMagneticField             = false;
                break;
            case PlanetInformation.jupiter_5:
                mass                            = 1898f;
                diameter                        = 142984f;
                density                         = 1326f;
                gravity                         = 23.1f;
                escapeVelocity                  = 59.5f;
                rotationPeriod                  = 9.9f;
                lengthOfDay                     = 9.9f;
                distanceFromSun                 = 778.5f;
                perihelion                      = 740.6f;
                aphelion                        = 816.4f;
                orbitalPeriod                   = 4331;
                orbitalVelocity                 = 13.1f;
                orbitalInclination              = 1.3f;
                orbitalEccentricity             = 0.049f;
                obliquityToOrbit                = 3.1f;
                meanTemperature                 = -110f;
                surfacePressure                 = 0f;
                numberOfMoons                   = 92;
                ringSystem                      = true;
                globalMagneticField             = true;
                break;

            case PlanetInformation.saturn_6:
                mass                            = 568f;
                diameter                        = 120536f;
                density                         = 687f;
                gravity                         = 9f;
                escapeVelocity                  = 35.5f;
                rotationPeriod                  = 10.7f;
                lengthOfDay                     = 10.7f;
                distanceFromSun                 = 1432f;
                perihelion                      = 1357.6f;
                aphelion                        = 1506.5f;
                orbitalPeriod                   = 10747f;
                orbitalVelocity                 = 9.7f;
                orbitalInclination              = 2.5f;
                orbitalEccentricity             = 0.052f;
                obliquityToOrbit                = 26.7f;
                meanTemperature                 = -140f;
                surfacePressure                 = 0f;
                numberOfMoons                   = 83;
                ringSystem                      = true;
                globalMagneticField             = true;
                break;

            case PlanetInformation.uranus_7:
                mass                            = 86.8f;
                diameter                        = 51118f;
                density                         = 1270f;
                gravity                         = 8.7f;
                escapeVelocity                  = 21.3f;
                rotationPeriod                  = -17.2f;
                lengthOfDay                     = 17.2f;
                distanceFromSun                 = 2867f;
                perihelion                      = 2732.7f;
                aphelion                        = 3001.4f;
                orbitalPeriod                   = 30589f;
                orbitalVelocity                 = 6.8f;
                orbitalInclination              = 0.8f;
                orbitalEccentricity             = 0.047f;
                obliquityToOrbit                = 97.8f;
                meanTemperature                 = -195f;
                surfacePressure                 = 0f;
                numberOfMoons                   = 27;
                ringSystem                      = true;
                globalMagneticField             = true;
                break;

            case PlanetInformation.neptune_8:
                mass                            = 102f;
                diameter                        = 49528f;
                density                         = 1638f;
                gravity                         = 11f;
                escapeVelocity                  = 23.5f;
                rotationPeriod                  = 16.1f;
                lengthOfDay                     = 16.1f;
                distanceFromSun                 = 4515f;
                perihelion                      = 4471.1f;
                aphelion                        = 4558.9f;
                orbitalPeriod                   = 59800f;
                orbitalVelocity                 = 5.4f;
                orbitalInclination              = 1.8f;
                orbitalEccentricity             = 0.01f;
                obliquityToOrbit                = 28.3f;
                meanTemperature                 = -200f;
                surfacePressure                 = 0f;
                numberOfMoons                   = 14;
                ringSystem                      = true;
                globalMagneticField             = true;
                break;

            case PlanetInformation.moon_9:
                mass                            = 0.073f;
                diameter                        = 3475f;
                density                         = 3340f;
                gravity                         = 1.6f;
                escapeVelocity                  = 2.4f;
                rotationPeriod                  = 655.7f;
                lengthOfDay                     = 708.7f;
                distanceFromSun                 = 0.384f;
                perihelion                      = 0.363f;
                aphelion                        = 0.406f;
                orbitalPeriod                   = 27.3f;
                orbitalVelocity                 = 1f;
                orbitalInclination              = 5.1f;
                orbitalEccentricity             = 0.055f;
                obliquityToOrbit                = 6.7f;
                meanTemperature                 = -20f;
                surfacePressure                 = 0f;
                numberOfMoons                   = 0;
                ringSystem                      = false;
                globalMagneticField             = false;
                break;

            case PlanetInformation.pluto_10:
                mass                            = 0f;
                diameter                        = 0f;
                density                         = 0f;
                gravity                         = 0f;
                escapeVelocity                  = 0f;
                rotationPeriod                  = 153f;
                lengthOfDay                     = 0f;
                distanceFromSun                 = 0f;
                perihelion                      = 0f;
                aphelion                        = 0f;
                orbitalPeriod                   = 0f;
                orbitalVelocity                 = 0f;
                orbitalInclination              = 0f;
                orbitalEccentricity             = 0f;
                obliquityToOrbit                = -57f;
                meanTemperature                 = 0f;
                surfacePressure                 = 0f;
                numberOfMoons                   = 0;
                ringSystem                      = false;
                globalMagneticField             = false;
                break;

            default:
                mass                            = 0f;
                diameter                        = 0f;
                density                         = 0f;
                gravity                         = 0f;
                escapeVelocity                  = 0f;
                rotationPeriod                  = 0f;
                lengthOfDay                     = 0f;
                distanceFromSun                 = 0f;
                perihelion                      = 0f;
                aphelion                        = 0f;
                orbitalPeriod                   = 0f;
                orbitalVelocity                 = 0f;
                orbitalInclination              = 0f;
                orbitalEccentricity             = 0f;
                obliquityToOrbit                = 0f;
                meanTemperature                 = 0f;
                surfacePressure                 = 0f;
                numberOfMoons                   = 0;
                ringSystem                      = false;
                globalMagneticField             = false;
                break;
        }
    }
}
