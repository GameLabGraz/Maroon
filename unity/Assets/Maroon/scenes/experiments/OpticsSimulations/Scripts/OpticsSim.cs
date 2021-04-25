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

public struct OpticsCircle
{
    public Vector2 midpoint;
    public float radius; // maybe special circle with radius negative left, positive right
}

[ExecuteInEditMode]
public class OpticsSim : MonoBehaviour
{


    OpticsCircle leftcircle;
    OpticsCircle rightcircle;

    Ray currentRay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        leftcircle.midpoint = new Vector2(0.0f, 0.0f);
        leftcircle.radius = 2.0f;
        OpticsRay testRay;
        testRay.origin = new Vector2(0.0f, 0.0f);
        testRay.dir = new Vector2(1.0f, 1.0f);

        (bool hit, Vector2 in1, Vector2 int2) = IntersectRay(testRay, leftcircle);

        if (hit)
        {
            Debug.Log("Hit!!!!");
            Debug.Log(in1.x);
            Debug.Log(in1.y);
        }

        List<Vector2> hits = new List<Vector2>(2);
        hits.Add(in1);
        hits.Add(int2);

        int hitidx = getNearestRayHit(hits, testRay);

        Debug.Log("hitindex " + hitidx + "point " +hits[hitidx]);

    }


    public (float, float, float) getLineEquation(OpticsRay r1)
    {
        // r1.origin.x
        float slope = r1.dir.y / r1.dir.x;

        // y - y1 = slope*(x -x1)

        float y = r1.origin.y;
        float m = slope;
        float b_c = m * (r1.origin.x - (r1.dir.x + r1.origin.x)) + (r1.origin.y + r1.dir.y); // todo is this 100% correct? line equation should suffice now - prolly not - found mistake

        // line equation is now y = m* r1.origin.x + b_c
        // doesnt work for horizontal lines for now.

        // y - y1 = m(x-x1)
        //care for exceptions!!
        Debug.Log("y: " + y + " m: " + m + " b_c: " + b_c);
        return (y, m, b_c);
    }


    // circle center x and y -> p and q
    //
    //
    //cuts a line with a circle. returns if hit or not hit and the respective hitpoints. 
    //
    //
    public (bool didhit, Vector2 inter1, Vector2 inter2) IntersectRay(OpticsRay toIntersect, OpticsCircle interCircle)
    {
        // intersect with all possible objects (circles, top/ bottom boundaries) 
        Vector2 intersect1 = new Vector2(0.0f, 0.0f);
        Vector2 intersect2 = new Vector2(0.0f, 0.0f);
        (float y, float m, float c) = getLineEquation(toIntersect);

        // circle center x and y -> p and q
        float p = interCircle.midpoint.x;
        float q = interCircle.midpoint.y;
        float r = interCircle.radius;

        float bigA = m * m + 1;
        float bigB = m * c - m * q - p;
        float bigC = q * q - r * r + p * p - 2 * c * q + c * c;

        float under_root = bigB * bigB - 4 * bigA * bigC;
        // if >0  circle is cut, 2 hitpoints, else miss 

        if(under_root <= 0.0f)
        {
            return (false, intersect1, intersect2);
        }

        float under_sqrt = Mathf.Sqrt(under_root);
        //else
        float x1 = (-bigB - under_sqrt) / (2.0f * bigA);
        float x2 = (-bigB + under_sqrt) / (2.0f * bigA);


        float y1 = m * x1 + c;
        float y2 = m * x2 + c;
        intersect1 = new Vector2(x1, y1);
        intersect2 = new Vector2(x2, y2);
        // else calculate the 2 hitpoints
        return (true, intersect1, intersect2);
    }

    //
    //returns -1 if no hit in correct direction
    //
    //
    public int getNearestRayHit(List<Vector2> hitpoints, OpticsRay raytocheck) // todo could be not correctly implemented yet
    {

        //todo: iterate over distances, check for shortest distance. remove all hitpoints that are behind the ray. 
        int nearestidx = -1;
        float magn = float.MaxValue;

        int idx = 0;
        foreach(var hitpoint in hitpoints)
        {
            Vector2 fromto = hitpoint - raytocheck.origin;

            if(fromto.magnitude < magn && Vector2.Dot(fromto, raytocheck.dir) > 0.0f)
            {
                magn = fromto.magnitude;
                nearestidx = idx;
            }

            idx++;


        }

        return nearestidx;
    }

}
