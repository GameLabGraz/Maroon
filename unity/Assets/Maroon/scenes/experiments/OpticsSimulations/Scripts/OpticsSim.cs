using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LensProperties
{
    float rad1;
}

public struct OpticsRay
{
    public Vector2 origin;
    public Vector2 dir;
}

public struct Circle
{
    Vector2 midpoint;
    float radius; // maybe special circle with radius negative left, positive right
}


public class OpticsSim : MonoBehaviour
{


    Circle leftcircle;
    Circle rightcircle;

    Ray currentRay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void getLineEquation(OpticsRay r1)
    {
        // r1.origin.x
        float slope = r1.dir.y / r1.dir.x;

        // y - y1 = slope*(x -x1)

        float y = r1.origin.y;
        float m = slope;
        float b_c = m * (r1.origin.x + r1.dir.x) + (r1.origin.y + r1.dir.y); // todo is this 100% correct? line equation should suffice now

        // line equation is now y = m* r1.origin.x + b_c
        // doesnt work for horizontal lines for now.

        // y - y1 = m(x-x1)
        //care for exceptions!!

    }

    public void IntersectRay(OpticsRay toIntersect)
    {
        // intersect with all possible objects (circles, top/ bottom boundaries) 




        return;
    }


}
