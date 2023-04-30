using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    public float G;//= 100f; //G = gravitational constant
    //public float semi_mayor_axis;
    GameObject[] planets;

    public PlanetInfo planetInfo;

    private void Awake()
    {
        //Debug.Log("Solar System Awake()");

        planets = GameObject.FindGameObjectsWithTag("Planet");
        if (planets.Length <= 0)
        {
            //Should not happen
            Debug.Log("No Planets Found:  " + planets.Length);
        }
    }

    void Start()
    {
        Debug.Log("Solar System Start()");

        InitialVelocity();
        //InitialVelocityEliptical();
    }


    private void FixedUpdate()
    {
        Gravity(); 
    }

    public class PlanetData : MonoBehaviour
    {
        public float semiMajorAxis;
        public float initialVelocity;
    }

    void Gravity()
    {    
        foreach(GameObject a in planets)
        {
            foreach(GameObject b in planets)
            {
                // object can't orbit itself
                if(!a.Equals(b))
                {
                    float m1 = a.GetComponent<Rigidbody>().mass;
                    float m2 = b.GetComponent<Rigidbody>().mass;

                    float r = Vector3.Distance(a.transform.position, b.transform.position);

                    // Newton's law of universal gravitation
                    // F = G * ((m1 * m2) / (r^2))
                    a.GetComponent<Rigidbody>().AddForce((b.transform.position - a.transform.position).normalized *
                        (G * (m1 * m2) / (r * r)));
                }
            }
        }
    }


    void InitialVelocity()
    {
        foreach(GameObject a in planets)
        {
            foreach(GameObject b in planets)
            {
                if(!a.Equals(b))
                {
                    // m2 = mass of central object
                    float m2 = b.GetComponent<Rigidbody>().mass;
                    // r = distance between the objects and a is the length of the semi-mayor axis
                    float r = Vector3.Distance(a.transform.position, b.transform.position);

                    a.transform.LookAt(b.transform);

                    // circular orbit instant velocity: v0 = sqrt((G * m2) / r)
                    a.GetComponent<Rigidbody>().velocity += a.transform.right * Mathf.Sqrt((G * m2) / r);
                }
            }
        }
    }


    void InitialVelocityEliptical()
    {
        foreach (GameObject a in planets)
        {
            foreach (GameObject b in planets)
            {
                if (!a.Equals(b))
                {
                    // m2 = mass of central object
                    float m2 = b.GetComponent<Rigidbody>().mass;
                    // r = distance between the objects
                    float r = Vector3.Distance(a.transform.position, b.transform.position);
                    // a = length of the semi-mayor axis
                    float a_axis = a.GetComponent<PlanetData>().semiMajorAxis;

                    a.transform.LookAt(b.transform);

                    // eliptic orbit instant velocity: v0 = G * m2 * (2 / r - 1 / a)
                    a.GetComponent<Rigidbody>().velocity += a.transform.right * (G * m2 * (2 / r - 1 / a_axis));
                    a.GetComponent<Rigidbody>().velocity += a.transform.right * (G * m2 * (2 / r - 1 / a_axis));
                }
            }
        }
    }




}
