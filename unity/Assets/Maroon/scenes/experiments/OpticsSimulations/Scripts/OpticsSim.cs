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
    public float radius; 
}

public struct OpticsLens
{
    public OpticsCircle leftCircle;
    public bool leftLeftSegment;
    public OpticsCircle rightCircle;
    public bool rightLeftSegment;
    public float radius;
    public float cauchyA;
    public float cauchyB;
    public float leftbound;
    public float rightbound;
}




public class OpticsSim : MonoBehaviour
{

    public static float AIR_REFRACTIVE_INDEX = 1.01f;

    public (bool didhit, Vector2 inter1, Vector2 inter2) IntersectRay(OpticsRay toIntersect, OpticsCircle interCircle)
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

    public int GetNearestRayHit(List<Vector2> hitPoints, OpticsRay rayToCheck) // todo could be not correctly implemented yet
    {

        //iterate over distances, check for shortest distance. remove all hitpoints that are behind the ray. 
        int nearestIndex = -1;
        float magn = float.MaxValue;

        int idx = 0;
        foreach(var hitPoint in hitPoints)
        {
            Vector2 fromto = hitPoint - rayToCheck.origin;

            if(fromto.magnitude < magn && Vector2.Dot(fromto, rayToCheck.dir) > 0.0f)
            {
                magn = fromto.magnitude;
                nearestIndex = idx;
            }
            idx++;
        }
        return nearestIndex;
    }

    public bool PointInRectangleBounds(Vector2 topLeft, Vector2 botRight, Vector2 p)
    {
        if (p.x > topLeft.x && p.x < botRight.x && p.y < topLeft.y && p.y > botRight.y) return true;

        return false;
    }

    public (float,float) GetBoundsHit(OpticsLens lens)
    {
        Vector2 upperLensBound = lens.leftCircle.midpoint + new Vector2(0, lens.radius);
        OpticsRay upperBound = new OpticsRay(upperLensBound, new Vector2(1, 0), 1.0f);

        float leftBound;
        float rightBound;

        (_, Vector2 int1, Vector2 int2) = IntersectRay(upperBound, lens.leftCircle);

        if (lens.leftLeftSegment)
        {
            leftBound = int2.x;
        }
        else
        {
            leftBound = int1.x;
        }

        (_, int1, int2) = IntersectRay(upperBound, lens.rightCircle);

        if (lens.rightLeftSegment)
        {
            rightBound = int2.x;
        }
        else
        {
            rightBound = int1.x;
        }
        //currently gets left and right bounds, best to make another function.  
        return (leftBound, rightBound);
    }

    public (bool, bool, Vector2, Vector2, Vector2, Vector2) GetOuterRingHit(OpticsRay currentRay, OpticsLens lens)
    {
        //get if parallel, if not intersect, then test for in bounds

        if(currentRay.dir.y < 0.00001f && currentRay.dir.y > -0.00001f)
        {
            //Debug.Log("no hitpoint");
            return (false, false, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero); // return no hitpoint
        }
        //else intersect the 1 ray with the other 2, get the 2 hitpoints, check if they are in the respective correct x bounds
        //and return bool bool point point normal normal;  
        (float leftb, float rightb) = GetBoundsHit(lens);

        float upperRay = lens.leftCircle.midpoint.y + lens.radius;
        float lowerRay = lens.leftCircle.midpoint.y - lens.radius;

        //Debug.Log("upperray lowrerray " + upperray + " " + lowerray);
        float inc = currentRay.dir.y / currentRay.dir.x;
        
        float offs = currentRay.origin.y + (-currentRay.origin.x) * inc;
        //Debug.Log("inc offs" + inc + " " + offs);
        float upperHitPoint = (upperRay - offs) * (1.0f / inc);
        float lowerHitPoint = (lowerRay - offs) * (1.0f / inc);

        //Debug.Log("upperhitpt, lowerhitpt: " + upperhitpt + " " + lowerhitpt); // wronggg
        bool upperIn = false;
        bool lowerIn = false;

        if (upperHitPoint > leftb && upperHitPoint < rightb) upperIn = true;
        if (lowerHitPoint > leftb && lowerHitPoint < rightb) lowerIn = true;

        //Debug.Log("upperin lowerin " + upperin + " " + lowerin + " "+ upperray);

        return (upperIn, lowerIn, new Vector2(upperHitPoint, upperRay), new Vector2(lowerHitPoint, lowerRay), Vector2.down, Vector2.up);

    }
    public (bool, bool,  Vector2, Vector2, Vector2 nor1, Vector2 nor2) GetSegmentHit(OpticsRay currentRay, OpticsCircle circ, bool isLeftSegment, float diameter = 1.0f)
    {

        (bool didHit, Vector2 int1, Vector2 int2) = IntersectRay(currentRay, circ);
        //Debug.Log("didhit " + didhit);
        //Debug.Log(int1 + "   " + int2);
        // if hit and there is atleast one intersection on the correct side and in the limits of the diameter (y-coord)
        // then we can add it to the 
        Vector2 topLeft;
        Vector2 botRight;

        Vector2 nor1 = new Vector2(0.0f, 0.0f);
        Vector2 nor2 = new Vector2(0.0f, 0.0f);

        //(float leftb, float rightb) = getBoundsHit(lens);
        
        // make hitbox for inside-check
        if (isLeftSegment)
        {
            topLeft = new Vector2(circ.midpoint.x - circ.radius * 2.0f,circ.midpoint.y + diameter);
            botRight = new Vector2(circ.midpoint.x, circ.midpoint.y + diameter*-1.0f);
        }else
        {
            topLeft = new Vector2(circ.midpoint.x , circ.midpoint.y + diameter);
            botRight = new Vector2(circ.midpoint.x+ circ.radius*2.0f, circ.midpoint.y+ diameter*-1.0f);
        }
        //Debug.Log("topleft botright " + topleft + " " + botright);

        bool h1 = false, h2 = false;
        if (didHit)
        {
            //Debug.Log("topleft " + topleft + " botright " + botright + " int1 " + int1 + " int2 " + int2);
            h1 = PointInRectangleBounds(topLeft, botRight, int1);
            h2 = PointInRectangleBounds(topLeft, botRight, int2);
            nor1 = int1 - circ.midpoint;
            nor2 = int2 - circ.midpoint;
        }
        return (h1, h2, int1, int2, nor1, nor2);
    }

    public Vector2 GetReflection(Vector2 norm, Vector2 entryDir)
    {
        norm = norm.normalized;
        entryDir = entryDir.normalized;
        //Debug.Log("GetReflection2");
        //Debug.Log(Vector2.Dot(norm, entrydir));
        //Debug.Log("norm, entrydir " + norm + " " + entrydir);

        float nor_length = Vector2.Dot(entryDir, norm);

        Vector2 ret1 = -nor_length*norm*2.0f + entryDir;

        //Debug.Log(ret1);

        Vector2 ret = entryDir - 2 * (Vector2.Dot(entryDir,norm)) * norm;


        return ret1;
    }

    public float GetCauchy(float A, float B, float wavelength)
    {
        float wlcauchy = wavelength / 1000;
        return A + B / (wlcauchy * wlcauchy);
    }

    public (Vector2, bool totalinternalreflection) GetRefraction(Vector2 normal, Vector2 entryDir, float refractiveIndex1, float refractiveIndex2)
    {

        //refractive multiplier, add to the refracive indices to account for wavelength differences
        // get entry angle

        float angle = Mathf.Acos(Vector2.Dot( entryDir.normalized, normal.normalized)); 

        if(angle*Mathf.Rad2Deg > 90.0f)
        {
            normal = -normal;
        }
        float sign_angle = Vector2.SignedAngle(normal.normalized, entryDir.normalized);
        float t1 = Mathf.Sin(angle) * refractiveIndex1;

        //Debug.Log("t1 " + t1);
        float t2 = t1 / refractiveIndex2;
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
        return (RotateVector(normal, res), false);
    }

    // https://answers.unity.com/questions/661383/whats-the-most-efficient-way-to-rotate-a-vector2-o.html
    private Vector2 RotateVector(Vector2 toRotate, float rad)
    {

        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);

        float tx = toRotate.x;
        float ty = toRotate.y;

        toRotate.x = (cos * tx) - (sin * ty);
        toRotate.y = (sin * tx) + (cos * ty);

        return toRotate;
    }



    public (Vector2, Vector2, bool) GetFirstLensHit(OpticsRay r, OpticsLens lens)
    {
        // should return hitpoint and normal, or miss 
        bool[] hit = new bool[6];
        Vector2[] intersects = new Vector2[6];
        Vector2[] normals = new Vector2[6];

        List<Vector2> hits = new List<Vector2>(6);
        List<Vector2> hitnormals = new List<Vector2>(6);

        (hit[0], hit[1],intersects[0], intersects[1], normals[0], normals[1]) = GetSegmentHit(r, lens.leftCircle, lens.leftLeftSegment, lens.radius);
        (hit[2], hit[3], intersects[2], intersects[3], normals[2], normals[3]) = GetSegmentHit(r, lens.rightCircle, lens.rightLeftSegment, lens.radius);
        (hit[4], hit[5], intersects[4], intersects[5], normals[4], normals[5]) = GetOuterRingHit(r, lens);

        for(int i = 0; i < hit.Length; i++)
        {
            if (hit[i])
            {
                hits.Add(intersects[i]);
                hitnormals.Add(normals[i]);
            }
        }

        int hitidx = GetNearestRayHit(hits, r);     

        if(hitidx < 0)
        {
            return ( Vector2.zero, Vector2.zero, false); // nothing hit
        }
        else
        {
            return ( hits[hitidx], hitnormals[hitidx], true); // hits, hitsnormals @ hitidx
        }
    }




    public void CalculateHitsRecursive(OpticsRay myRay, OpticsLens myLens, bool outside, ref List<OpticsSegment> segs, float loss = 0.9f, float reflectionVsRefraction = 0.3f)
    {
        if (myRay.intensity < 0.02) return; // dont go further if intensity low. TODO change to be changeable param.

        (Vector2 hit, Vector2 norm, bool didHit) = GetFirstLensHit(myRay, myLens);

        if (!didHit)
        {
            OpticsSegment addEnd = new OpticsSegment(myRay.origin, myRay.origin + 30.0f * myRay.dir, myRay.intensity);
            addEnd.segmentColor = myRay.rayColor;
            segs.Add(addEnd);
            return; // we are done here... nothing hit.
        }

        OpticsSegment toAdd = new OpticsSegment(myRay.origin, hit, myRay.intensity);
        toAdd.segmentColor = myRay.rayColor;
        segs.Add(toAdd);

        var innerref = GetCauchy(myLens.cauchyA, myLens.cauchyB, myRay.wavelengthMultiplier);
        var outerref = AIR_REFRACTIVE_INDEX; //myLens.outerRefractiveidx;

        if (!outside)
        {
            var temp = innerref;
            innerref = outerref;
            outerref = temp;
        }

        (Vector2 refracDir, bool wasTotalRefraction) = GetRefraction(norm, myRay.dir, outerref, innerref);

        Vector2 reflecDir = GetReflection(-norm, myRay.dir);

        if (wasTotalRefraction)
        { // was  a total reflection, only follow the reflection vector.
            OpticsRay reflectedRay = new OpticsRay(hit + 0.0001f * reflecDir.normalized, reflecDir.normalized, myRay.intensity * 1.0f *loss, myRay.rayColor, myRay.wavelengthMultiplier);
            //TODO epsilon changeable

            CalculateHitsRecursive(reflectedRay, myLens, outside, ref segs, loss, reflectionVsRefraction);
            return;
        }
        else
        {
            OpticsRay reflectedRay = new OpticsRay(hit + 0.0001f * reflecDir.normalized, reflecDir.normalized, myRay.intensity * reflectionVsRefraction*loss, myRay.rayColor, myRay.wavelengthMultiplier);
            OpticsRay refractedRay = new OpticsRay(hit + 0.0001f * refracDir.normalized, refracDir.normalized, myRay.intensity * (1.0f - reflectionVsRefraction)*loss, myRay.rayColor, myRay.wavelengthMultiplier);

            CalculateHitsRecursive(reflectedRay, myLens, outside, ref segs, loss, reflectionVsRefraction);
            CalculateHitsRecursive(refractedRay, myLens, !outside, ref segs, loss, reflectionVsRefraction);
        }
    }
}

