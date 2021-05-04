using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OpticsRay
{
    public Vector2 origin;
    public Vector2 dir;
    public float intensity;

    public OpticsRay(Vector2 origin, Vector2 dir, float intensity)
    {
        this.origin = origin;
        this.dir = dir;
        this.intensity = intensity;
    }
}

public struct OpticsSegment
{
    public Vector2 p1;
    public Vector2 p2;
    public float intensity;

    public OpticsSegment(Vector2 p1, Vector2 p2, float intensity)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.intensity = intensity;

    }
}

public struct OpticsCircle
{
    public Vector2 midpoint;
    public float radius; // maybe special circle with radius negative left, positive right
}

public struct OpticsLens
{
    public OpticsCircle leftCircle;
    public bool leftLeftSegment;
    public OpticsCircle rightCircle;
    public bool rightLeftSegment;
    public float radius;
    public float innerRefractiveidx;
    public float outerRefractiveidx;
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
        testRay.origin = new Vector2(-5.0f, 0.5f);
        testRay.dir = new Vector2(1.0f, 0.0f);
        testRay.intensity = 1.0f;


        OpticsLens lens;
        lens.leftCircle.midpoint = new Vector2(-2.0f, 0.0f);
        lens.leftCircle.radius = 2.0f;
        lens.leftLeftSegment = true;
        lens.rightCircle.midpoint = new Vector2(2.0f, 0.0f);
        lens.rightCircle.radius = 2.0f;
        lens.rightLeftSegment = false;
        lens.radius = 1.0f;
        lens.innerRefractiveidx = 1.3f;
        lens.outerRefractiveidx = 1.0f;

        (Vector2 pt, Vector2 nor, bool ishit) = getFirstLensHit(testRay, lens);


        Debug.Log("ishit: " + ishit + " pt: " + pt + " nor: " + nor);

        List<OpticsSegment> segments = new List<OpticsSegment>(20);
        /*
        calcHitsRecursive(testRay, lens, ref segments);

        Debug.Log("numofsegments: " + segments.Count);

        foreach(var seg in segments)
        {
            Debug.Log("segment: " + seg.p1 + " " +seg.p2 + " "+ seg.intensity);
        } */


        OpticsLens lens2;
        lens2.leftCircle.midpoint = new Vector2(0, 1);
        lens2.leftCircle.radius = 1.0f;
        lens2.leftLeftSegment = true;
        lens2.rightCircle.midpoint = new Vector2(0, 1);
        lens2.rightCircle.radius = 1.0f;
        lens2.rightLeftSegment = false;
        lens2.radius = 1.0f;
        lens2.innerRefractiveidx = 1.0f;
        lens2.outerRefractiveidx = 1.3f;

        OpticsRay testray2;
        testray2.origin = new Vector2(0.0f, 1.1f);
        testray2.dir = new Vector2(1.0f, 0.0f);
        testray2.intensity = 1.0f;
        List<OpticsSegment> segs2 = new List<OpticsSegment>(100);
        calcHitsRecursive(testray2, lens2, false, ref segs2);
        
        Debug.Log(segs2.Count);

        foreach (var seg in segs2)
        {
            Debug.Log("segment: " + seg.p1 + " " + seg.p2 + " " + seg.intensity);
        }

        Debug.Log("numsegments: " + segs2.Count);

        Debug.Log("---------------");

        Vector2 tvec = new Vector2(1, 0);

        Vector2 tnorm = new Vector2(-0.4f, 0.9f);

        //Debug.Log( "getrefl" + getReflection(tnorm, tvec));
        //Debug.Log("getrefl2" + getReflection2(tnorm, tvec));
        /*
        //(bool hit, Vector2 in1, Vector2 int2) = IntersectRay(testRay, leftcircle);

        (bool h1, bool h2, Vector2 int1, Vector2 int2, Vector2 nor1, Vector2 nor2) = getSegmentHit(testRay, leftcircle, false);

        //Debug.Log("h1 " + h1 + "h2 " + h2);
        List<Vector2> hits = new List<Vector2>(2);
        List<Vector2> hitnormals = new List<Vector2>(2);


        bool testus = pointInRectangleBounds(new Vector2(-1, 1),new Vector2(1, -1),new Vector2(0, 0));

        //Debug.Log("testus " + testus);

        if (h1)
        {
            hits.Add(int1);
            hitnormals.Add(nor1);
        }
        if (h2)
        {
            hits.Add(int2);
            hitnormals.Add(nor2);
        }
            

        if(hits.Count > 0)
        {
            //Debug.Log(hits[0]);
        }


        int hitidx = getNearestRayHit(hits, testRay);

        if( hitidx >= 0)
        {
            //Debug.Log("hitindex " + hitidx + "point " + hits[hitidx]);
        }

        //getRefraction(new Vector2(0.0f, 1.0f), new Vector2(2.0f, -1.0f), 1.33f, 1.0f);

        Debug.Log(getReflection(new Vector2(0, -1), new Vector2(-2.0f, -1.0f)));
        (Vector2 refr, bool totref) = getRefraction(new Vector2(0.0f, -1.0f), new Vector2(-2.0f, -1.0f), 1.0f, 1.33f);

        */
    }



    // inputs: ray, all properties of a lens 
    // recursion anchor -> add ray intensity property 

    public (float, float, float) getLineEquation(OpticsRay r1)
    {
        // r1.origin.x
        float eps = 0.0001f;
        if(r1.dir.x < eps && r1.dir.x > -eps)
        {
            r1.dir.x = eps; // TODO not good, but eliminates errors
        }

        float slope = r1.dir.y / r1.dir.x;

        //Debug.Log("slope " + slope);

        // y - y1 = slope*(x -x1)

        float y = r1.origin.y;
        float m = slope;
        float b_c = m * (r1.origin.x - (r1.dir.x + r1.origin.x)) + (r1.origin.y + r1.dir.y); // todo is this 100% correct? line equation should suffice now - prolly not - found mistake

        // line equation is now y = m* r1.origin.x + b_c
        // doesnt work for horizontal lines for now.

        // y - y1 = m(x-x1)
        //care for exceptions!!
        //Debug.Log("y: " + y + " m: " + m + " b_c: " + b_c);
        return (y, m, b_c);
    }


    // circle center x and y -> p and q
    //
    //
    //cuts a line with a circle. returns if hit or not hit and the respective hitpoints. 
    //
    //
    public (bool didhit, Vector2 inter1, Vector2 inter2) IntersectRay(OpticsRay toIntersect, OpticsCircle interCircle) // todo change to number of hits ( 0, 1 or 2) and add accordingly
    {
        // intersect with all possible objects (circles, top/ bottom boundaries) 
        Vector2 intersect1 = new Vector2(0.0f, 0.0f);
        Vector2 intersect2 = new Vector2(0.0f, 0.0f);
        (float y, float m, float c) = getLineEquation(toIntersect);


        //Debug.Log("m, c: " + m + " " + c);

        // circle center x and y -> p and q
        float p = interCircle.midpoint.x;
        float q = interCircle.midpoint.y;
        float r = interCircle.radius;

        float bigA = m * m + 1;
        float bigB = m * c - m * q - p;
        float bigC = q * q - r * r + p * p - 2 * c * q + c * c;

        float under_root = bigB * bigB - 4.0f * bigA * bigC;
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
        intersect1 = new Vector2(toIntersect.origin.x + x1 *(toIntersect.dir.x-toIntersect.origin.x ), toIntersect.origin.y +x1 *(toIntersect.dir.y - toIntersect.origin.y) );
        intersect2 = new Vector2(toIntersect.origin.x + x2 *(toIntersect.dir.x-toIntersect.origin.x ), toIntersect.origin.y +x2 *(toIntersect.dir.y - toIntersect.origin.y) );
        //intersect2 = new Vector2(x2, y2);
        // else calculate the 2 hitpoints
        return (true, intersect1, intersect2);
    }

    //
    //returns -1 if no hit in correct direction
    //
    //

    // from here http://devmag.org.za/2009/04/17/basic-collision-detection-in-2d-part-2/ , for debug
    public (bool didhit, Vector2 inter1, Vector2 inter2) IntersectRay2(OpticsRay toIntersect, OpticsCircle interCircle)
    {

        Vector2 localP1 = toIntersect.origin - interCircle.midpoint;
        Vector2 localP2 = toIntersect.origin + toIntersect.dir - interCircle.midpoint;

        Vector2 p2minp1 = localP2 - localP1;

        float a = p2minp1.x * p2minp1.x + p2minp1.y * p2minp1.y;
        float b = 2.0f * ((p2minp1.x * localP1.x) + (p2minp1.y * localP1.y));
        float c = (localP1.x * localP1.x) + localP1.y * localP1.y - interCircle.radius*interCircle.radius;

        float delta = b * b - (4 * a * c);

        if (delta < 0.0f)
        {
            return (false, new Vector2(0, 0), new Vector2(0, 0));
        }

        float sqrtdel = Mathf.Sqrt(delta);
        float u1 = (-b + sqrtdel) / (2 * a);
        float u2 = (-b - sqrtdel) / (2 * a);


        return (true, toIntersect.origin + (u1*p2minp1), toIntersect.origin + (u2*p2minp1));
    }

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

    public bool pointInRectangleBounds(Vector2 topleft, Vector2 botright, Vector2 p)
    {
        //Debug.Log("pointinrectangleounds " + topleft + " " + botright + " " + p);
        //Debug.Log("pointinrectanglebounds " + (p.x > topleft.x && p.x < botright.x && p.y < topleft.y && p.y > botright.y));
        if (p.x > topleft.x && p.x < botright.x && p.y < topleft.y && p.y > botright.y) return true;

        return false;
    }

    public (bool, bool,  Vector2, Vector2, Vector2 nor1, Vector2 nor2) getSegmentHit(OpticsRay currray, OpticsCircle circ, bool isLeftSegment, float diameter = 1.0f)
    {

        (bool didhit, Vector2 int1, Vector2 int2) = IntersectRay2(currray, circ);
        //Debug.Log("didhit " + didhit);
        //Debug.Log(int1 + "   " + int2);
        // if hit and there is atleast one intersection on the correct side and in the limits of the diameter (y-coord)
        // then we can add it to the 
        Vector2 topleft;
        Vector2 botright;

        Vector2 nor1 = new Vector2(0.0f, 0.0f);
        Vector2 nor2 = new Vector2(0.0f, 0.0f);
        
        // make hitbox for inside-check
        if (isLeftSegment)
        {
            topleft = new Vector2(circ.midpoint.x - circ.radius * 2.0f,circ.midpoint.y + diameter);
            botright = new Vector2(circ.midpoint.x, circ.midpoint.y + diameter*-1.0f);
        }else
        {
            topleft = new Vector2(circ.midpoint.x , circ.midpoint.y + diameter);
            botright = new Vector2(circ.midpoint.x+ circ.radius*2.0f, circ.midpoint.y+ diameter*-1.0f);
        }



         Debug.Log("topleft botright " + topleft + " " + botright);

        bool h1 = false, h2 = false;
        if (didhit)
        {
            //Debug.Log("topleft " + topleft + " botright " + botright + " int1 " + int1 + " int2 " + int2);
            h1 = pointInRectangleBounds(topleft, botright, int1);
            h2 = pointInRectangleBounds(topleft, botright, int2);
            nor1 = int1 - circ.midpoint;
            nor2 = int2 - circ.midpoint;
        }
        /*
        if (didhit)
        {// we hit
            bool firsthitright = circ.midpoint.x > int1.x;
            bool secondhitright = circ.midpoint.x > int2.x;

            if (isLeftSegment)
            {
                firsthitright = !firsthitright;
                secondhitright = !secondhitright;
            }

            // if one of those hits is true, we gotta check if it hitpoint y is in y bounds

            bool firsthitinbound = int1.y < diameter && int1.y > -diameter;
            bool secondhitinbound = int2.y < diameter && int2.y > -diameter;
                    }
        */

        //Debug.Log("getsegmenthit nor1 nor2: " + nor1 + " " + nor2);
        //Debug.Log("getsegmenthit h1 h2: " + h1 + " " + h2);
        //Debug.Log("getsegmenthit int1 int2: " + int1 + " " + int2);

        return (h1, h2, int1, int2, nor1, nor2);
    }


    public Vector2 getReflection(Vector2 normal, Vector2 entrydir)
    {
        entrydir = -entrydir;

        
        Vector2 nornormal = normal.normalized;

        Vector2 ret = entrydir - 2 * (Vector2.Dot(entrydir, nornormal) * nornormal);
        Debug.Log("getreflection normal + entrydir " + normal + " " + entrydir + " return " + ret);
        return ret;
    }

    public Vector2 getReflection2(Vector2 norm, Vector2 entrydir)
    {
        norm = norm.normalized;
        entrydir = entrydir.normalized;
        Debug.Log("GetReflection2");
        //Debug.Log(Vector2.Dot(norm, entrydir));
        Debug.Log("norm, entrydir " + norm + " " + entrydir);

        float nor_length = Vector2.Dot(entrydir, norm);

        Vector2 ret1 = -nor_length*norm*2.0f + entrydir;

        Debug.Log(ret1);

        Vector2 ret = entrydir - 2 * (Vector2.Dot(entrydir,norm)) * norm;


        return ret1;
    }

    public (Vector2, bool totalinternalreflection) getRefraction(Vector2 normal, Vector2 entrydir, float refIdx1, float refIdx2)
    {
        // get entry angle
        float angle = Mathf.Acos(Vector2.Dot( entrydir.normalized, normal.normalized)); // 

        if(angle*Mathf.Rad2Deg > 90.0f)
        {
            normal = -normal;
        }

        Debug.Log("normal" + normal);
        //now dot product should only be positive

        float sign_angle = Vector2.SignedAngle(normal.normalized, entrydir.normalized);

        //Debug.Log("dot product " + Vector2.Dot(entrydir.normalized, normal.normalized));
        // if normalvector is other direction, angle is more than 90deg. 
        // flip normal vector for further calculation and subtract 90 deg?

        //Debug.Log("sign_angle " + sign_angle); // had to be normalized

        // n1 * sin theta1 = n2 * sin theta2
        //umgeformt: theta2 = sin-1(  (sin(theta1)*n1)/n2 )

        //totalreflexion ab n1* sin(theta1)/n2
        float t1 = Mathf.Sin(angle) * refIdx1;

        //Debug.Log("t1 " + t1);
        float t2 = t1 / refIdx2;
        float res = Mathf.Asin(t2); // seems correct for now 

        if(float.IsNaN(res)) //nan -> total internal reflection
        {
            Debug.Log("total internal reflection");
            return (new Vector2(0, 0), true); // ISTOTALREFLECTION

        }

        // no total reflection, now we calc refraction direction
        //Debug.Log("sin (res) " + Mathf.Sin(res) + " res " + res);
        //Debug.Log("div " + refIdx1 / refIdx2);
        if (sign_angle < 0)
        {
            res = -res;
        }

        //Debug.Log("refraction angle: " + res*Mathf.Rad2Deg);
        Vector2 rvec = rotateVec(normal, res);
        

        //Debug.Log("kj " + rvec.x + " " + rvec.y);
        

        return (rotateVec(normal, res), false);
    }

    // https://answers.unity.com/questions/661383/whats-the-most-efficient-way-to-rotate-a-vector2-o.html
    private Vector2 rotateVec(Vector2 torotate, float rad)
    {

        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);

        float tx = torotate.x;
        float ty = torotate.y;

        torotate.x = (cos * tx) - (sin * ty);
        torotate.y = (sin * tx) + (cos * ty);

        return torotate;
    }



    public (Vector2, Vector2, bool) getFirstLensHit(OpticsRay r, OpticsLens lens)
    {
        // should return hitpoint and normal, or miss 
        bool[] hit = new bool[4];
        Vector2[] intersects = new Vector2[4];
        Vector2[] normals = new Vector2[4];

        List<Vector2> hits = new List<Vector2>(2);
        List<Vector2> hitnormals = new List<Vector2>(2);

        (hit[0], hit[1],intersects[0], intersects[1], normals[0], normals[1]) = getSegmentHit(r, lens.leftCircle, lens.leftLeftSegment, lens.radius);
        
        (hit[2], hit[3], intersects[2], intersects[3], normals[2], normals[3]) = getSegmentHit(r, lens.rightCircle, lens.rightLeftSegment, lens.radius);


        for(int i = 0; i < 4; i++)
        {
            if (hit[i])
            {
                //Debug.Log("hit something");
                hits.Add(intersects[i]);
                hitnormals.Add(normals[i]);
            }
        }

        int hitidx = getNearestRayHit(hits, r);
        Debug.Log(" getfirstlenshit index: " + hitidx + " "+ hits.Count);
        

        if(hitidx < 0)
        {
            return ( Vector2.zero, Vector2.zero, false); // nothing hit
        }
        else
        {
            Debug.Log("normals at hitindex " + hitnormals[hitidx]);
            return ( hits[hitidx], hitnormals[hitidx], true); // hits, hitsnormals @ hitidx
        }
    }




    public void calcHitsRecursive(OpticsRay myRay, OpticsLens myLens, bool outside, ref List<OpticsSegment> segs)
    {
        if (myRay.intensity < 0.02) return; // dont go further if intensity low. TODO change to be changeable param.

        (Vector2 hit, Vector2 norm, bool didhit) = getFirstLensHit(myRay, myLens);

        

        if (!didhit)
        {
            Debug.Log("nothing hit. returning. ADDED END ::::::::::::::::::::::::::");
            OpticsSegment addend = new OpticsSegment(myRay.origin, myRay.origin + 100.0f * myRay.dir, myRay.intensity);
            segs.Add(addend);
            return; // we are done here... nothing hit.


        }
        Debug.Log("NORMAL HIT calchitsrecu: " + norm +" " + hit);

        //TODO add raysegment to segs list...
        OpticsSegment toadd = new OpticsSegment(myRay.origin, hit, myRay.intensity);
        segs.Add(toadd);

        var innerref = myLens.innerRefractiveidx;
        var outerref = myLens.outerRefractiveidx;

        if (!outside)
        {
            var temp = innerref;
            innerref = outerref;
            outerref = temp;
        }

        (Vector2 refracdir, bool wastotalreflection) = getRefraction(norm, myRay.dir, innerref, outerref);

        Vector2 reflecdir = getReflection2(-norm, myRay.dir);

        Debug.Log("reflecidr " + reflecdir + " refracdir " + refracdir);

        if (wastotalreflection)
        { // was  a total reflection, only follow the reflection vector.
            OpticsRay reflectedRay = new OpticsRay(hit + 0.0001f * reflecdir.normalized, reflecdir.normalized, myRay.intensity * 0.95f);
            //TODO epsilon changeable

            calcHitsRecursive(reflectedRay, myLens, outside, ref segs);
            return;
        }
        else
        {
            OpticsRay reflectedRay = new OpticsRay(hit + 0.0001f * reflecdir.normalized, reflecdir.normalized, myRay.intensity * 0.45f);
            OpticsRay refractedRay = new OpticsRay(hit + 0.0001f * refracdir.normalized, refracdir.normalized, myRay.intensity * 0.45f);

            calcHitsRecursive(reflectedRay, myLens, outside, ref segs);
            calcHitsRecursive(refractedRay, myLens, !outside, ref segs);
        }
    }


}

