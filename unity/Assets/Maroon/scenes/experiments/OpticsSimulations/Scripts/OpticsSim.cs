using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OpticsRay
{
    public Vector2 origin;
    public Vector2 dir;
    public float intensity;
    public Color rayColor;
    public float wavelengthMultiplier;

    public OpticsRay(Vector2 origin, Vector2 dir, float intensity)
    {
        this.origin = origin;
        this.dir = dir;
        this.intensity = intensity;
        this.rayColor = Color.red;
        this.wavelengthMultiplier = 1.0f;
    }
    public OpticsRay(Vector2 origin, Vector2 dir, float intensity, Color rayColor, float wavelengthmult = 886.0f)
    {
        this.origin = origin;
        this.dir = dir;
        this.intensity = intensity;
        this.rayColor = rayColor;
        this.wavelengthMultiplier = wavelengthmult;
    }
}

public struct OpticsSegment
{
    public Vector2 p1;
    public Vector2 p2;
    public float intensity;
    public Color segmentColor;

    public OpticsSegment(Vector2 p1, Vector2 p2, float intensity)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.intensity = intensity;
        this.segmentColor = Color.red;
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
    public float leftbound;
    public float rightbound;
}


public class OpticsSim : MonoBehaviour
{
    // inputs: ray, all properties of a lens 
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

        //iterate over distances, check for shortest distance. remove all hitpoints that are behind the ray. 
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
        if (p.x > topleft.x && p.x < botright.x && p.y < topleft.y && p.y > botright.y) return true;

        return false;
    }

    public (float,float) getBoundsHit(OpticsLens lens)
    {
        Vector2 upperlensbound = lens.leftCircle.midpoint + new Vector2(0, lens.radius);
        OpticsRay upperbound = new OpticsRay(upperlensbound, new Vector2(1, 0), 1.0f);

        float leftbound = 0.0f;
        float rightbound = 0.0f;

        (_, Vector2 int1, Vector2 int2) = IntersectRay2(upperbound, lens.leftCircle);

        if (lens.leftLeftSegment)
        {
            leftbound = int2.x;
        }
        else
        {
            leftbound = int1.x;
        }

        (_, int1, int2) = IntersectRay2(upperbound, lens.rightCircle);

        if (lens.rightLeftSegment)
        {
            rightbound = int2.x;
        }
        else
        {
            rightbound = int1.x;
        }
        //currently gets left and right bounds, best to make another function.  
        return (leftbound, rightbound);
    }

    public (bool, bool, Vector2, Vector2, Vector2, Vector2) getOuterRingHit(OpticsRay currray, OpticsLens lens)
    {
        //get if parallel, if not intersect, then test for in bounds

        if(currray.dir.y < 0.00001f && currray.dir.y > -0.00001f)
        {
            //Debug.Log("no hitpoint");
            return (false, false, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero); // return no hitpoint
        }
        //else intersect the 1 ray with the other 2, get the 2 hitpoints, check if they are in the respective correct x bounds
        //and return bool bool point point normal normal;  
        (float leftb, float rightb) = getBoundsHit(lens);

        float upperray = lens.leftCircle.midpoint.y + lens.radius;
        float lowerray = lens.leftCircle.midpoint.y - lens.radius;

        //Debug.Log("upperray lowrerray " + upperray + " " + lowerray);
        float inc = currray.dir.y / currray.dir.x;
        
        float offs = currray.origin.y + (-currray.origin.x) * inc;
        //Debug.Log("inc offs" + inc + " " + offs);
        float upperhitpt = (upperray - offs) * (1.0f / inc);
        float lowerhitpt = (lowerray - offs) * (1.0f / inc);

        //Debug.Log("upperhitpt, lowerhitpt: " + upperhitpt + " " + lowerhitpt); // wronggg
        bool upperin = false;
        bool lowerin = false;

        if (upperhitpt > leftb && upperhitpt < rightb) upperin = true;
        if (lowerhitpt > leftb && lowerhitpt < rightb) lowerin = true;

        //Debug.Log("upperin lowerin " + upperin + " " + lowerin + " "+ upperray);

        return (upperin, lowerin, new Vector2(upperhitpt, upperray), new Vector2(lowerhitpt, lowerray), Vector2.down, Vector2.up);

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

        //(float leftb, float rightb) = getBoundsHit(lens);
        
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
        //Debug.Log("topleft botright " + topleft + " " + botright);

        bool h1 = false, h2 = false;
        if (didhit)
        {
            //Debug.Log("topleft " + topleft + " botright " + botright + " int1 " + int1 + " int2 " + int2);
            h1 = pointInRectangleBounds(topleft, botright, int1);
            h2 = pointInRectangleBounds(topleft, botright, int2);
            nor1 = int1 - circ.midpoint;
            nor2 = int2 - circ.midpoint;
        }
        return (h1, h2, int1, int2, nor1, nor2);
    }

    public Vector2 getReflection2(Vector2 norm, Vector2 entrydir)
    {
        norm = norm.normalized;
        entrydir = entrydir.normalized;
        //Debug.Log("GetReflection2");
        //Debug.Log(Vector2.Dot(norm, entrydir));
        //Debug.Log("norm, entrydir " + norm + " " + entrydir);

        float nor_length = Vector2.Dot(entrydir, norm);

        Vector2 ret1 = -nor_length*norm*2.0f + entrydir;

        //Debug.Log(ret1);

        Vector2 ret = entrydir - 2 * (Vector2.Dot(entrydir,norm)) * norm;


        return ret1;
    }

    public (Vector2, bool totalinternalreflection) getRefraction(Vector2 normal, Vector2 entrydir, float refIdx1, float refIdx2, float wavelength)
    {



        //refractive multiplier, add to the refracive indices to account for wavelength differences

        wavelength = (wavelength - 380.0f) / 340.0f;

        refIdx1 += (refIdx1 - 1.0f) * wavelength * 0.2f;
        refIdx2 += (refIdx2 - 1.0f) * wavelength * 0.2f;

        // get entry angle

        float angle = Mathf.Acos(Vector2.Dot( entrydir.normalized, normal.normalized)); 

        if(angle*Mathf.Rad2Deg > 90.0f)
        {
            normal = -normal;
        }
        float sign_angle = Vector2.SignedAngle(normal.normalized, entrydir.normalized);
        float t1 = Mathf.Sin(angle) * refIdx1;

        //Debug.Log("t1 " + t1);
        float t2 = t1 / refIdx2;
        float res = Mathf.Asin(t2); // seems correct for now 

        if(float.IsNaN(res)) //nan -> total internal reflection
        {
            return (new Vector2(0, 0), true);
        }
        // no total reflection, now we calc refraction direction
        if (sign_angle < 0)
        {
            res = -res;
        }
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
        bool[] hit = new bool[6];
        Vector2[] intersects = new Vector2[6];
        Vector2[] normals = new Vector2[6];

        List<Vector2> hits = new List<Vector2>(6);
        List<Vector2> hitnormals = new List<Vector2>(6);

        (hit[0], hit[1],intersects[0], intersects[1], normals[0], normals[1]) = getSegmentHit(r, lens.leftCircle, lens.leftLeftSegment, lens.radius);
        (hit[2], hit[3], intersects[2], intersects[3], normals[2], normals[3]) = getSegmentHit(r, lens.rightCircle, lens.rightLeftSegment, lens.radius);
        (hit[4], hit[5], intersects[4], intersects[5], normals[4], normals[5]) = getOuterRingHit(r, lens);

        for(int i = 0; i < hit.Length; i++)
        {
            if (hit[i])
            {
                hits.Add(intersects[i]);
                hitnormals.Add(normals[i]);
            }
        }

        int hitidx = getNearestRayHit(hits, r);     

        if(hitidx < 0)
        {
            return ( Vector2.zero, Vector2.zero, false); // nothing hit
        }
        else
        {
            return ( hits[hitidx], hitnormals[hitidx], true); // hits, hitsnormals @ hitidx
        }
    }




    public void calcHitsRecursive(OpticsRay myRay, OpticsLens myLens, bool outside, ref List<OpticsSegment> segs, float loss = 0.9f, float reflvsrefr = 0.3f)
    {
        if (myRay.intensity < 0.02) return; // dont go further if intensity low. TODO change to be changeable param.

        (Vector2 hit, Vector2 norm, bool didhit) = getFirstLensHit(myRay, myLens);

        if (!didhit)
        {
            OpticsSegment addend = new OpticsSegment(myRay.origin, myRay.origin + 30.0f * myRay.dir, myRay.intensity);
            addend.segmentColor = myRay.rayColor;
            segs.Add(addend);
            return; // we are done here... nothing hit.


        }

        OpticsSegment toadd = new OpticsSegment(myRay.origin, hit, myRay.intensity);
        toadd.segmentColor = myRay.rayColor;
        segs.Add(toadd);

        var innerref = myLens.innerRefractiveidx;
        var outerref = myLens.outerRefractiveidx;

        if (!outside)
        {
            var temp = innerref;
            innerref = outerref;
            outerref = temp;
        }

        (Vector2 refracdir, bool wastotalreflection) = getRefraction(norm, myRay.dir, outerref, innerref, myRay.wavelengthMultiplier);

        Vector2 reflecdir = getReflection2(-norm, myRay.dir);

        if (wastotalreflection)
        { // was  a total reflection, only follow the reflection vector.
            OpticsRay reflectedRay = new OpticsRay(hit + 0.0001f * reflecdir.normalized, reflecdir.normalized, myRay.intensity * 1.0f *loss, myRay.rayColor, myRay.wavelengthMultiplier);
            //TODO epsilon changeable

            calcHitsRecursive(reflectedRay, myLens, outside, ref segs, loss, reflvsrefr);
            return;
        }
        else
        {
            OpticsRay reflectedRay = new OpticsRay(hit + 0.0001f * reflecdir.normalized, reflecdir.normalized, myRay.intensity * reflvsrefr*loss, myRay.rayColor, myRay.wavelengthMultiplier);
            OpticsRay refractedRay = new OpticsRay(hit + 0.0001f * refracdir.normalized, refracdir.normalized, myRay.intensity * (1.0f - reflvsrefr)*loss, myRay.rayColor, myRay.wavelengthMultiplier);

            calcHitsRecursive(reflectedRay, myLens, outside, ref segs, loss, reflvsrefr);
            calcHitsRecursive(refractedRay, myLens, !outside, ref segs, loss, reflvsrefr);
        }
    }
}

