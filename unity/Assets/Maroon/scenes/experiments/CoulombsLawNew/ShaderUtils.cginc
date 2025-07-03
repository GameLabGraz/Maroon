// Note(MartinR): This file contains all universal shader logic that may be reused at any point

#ifndef _SHADER_UTILS_CGINC_
#define _SHADER_UTILS_CGINC_

// Returns distance to intersection, or -1 if not hit
float raySphereIntersection(float3 rayOrigin, float3 dir, float3 spherePos, float radius, out float3 normal)
{
    normal = float3(1, 0, 0);

    float3 toCenter = spherePos - rayOrigin;
    float t_closest = dot(toCenter, dir);
    if (t_closest < 0.0)
        return -1.0;

    float r2 = radius * radius;
    float d2 = dot(toCenter, toCenter) - t_closest * t_closest;
    float offset = r2 - d2;
    if (offset < 0.0)
        return -1.0;
    offset = sqrt(offset);

    float t_intersection = t_closest - offset;
    normal = normalize((rayOrigin + dir * t_intersection) - spherePos);
    return t_intersection;
}

// Returns distance to first intersection, or -1 if not hit
float rayBoxIntersection(float3 rayOrigin, float3 rayDir, float3 boxMin, float3 boxMax)
{
    // Move box to coord system center
    rayOrigin -= (boxMax + boxMin) / 2.0;

    // Flip coordinate system so ray direction coordinates are all positive
    float3 dirSign = sign(rayDir);
    rayDir = dirSign * rayDir;
    rayOrigin = dirSign * rayOrigin;

    // Find intersection-distances with all six box planes
    float3 extends = (boxMax - boxMin) / 2.0;
    float3 tMaxs = (extends - rayOrigin) / rayDir;
    float3 tMins = (-extends - rayOrigin) / rayDir;

    // Find intersection of all 3 intervals
    float t0 = max(max(tMins.x, tMins.y), tMins.z);
    float t1 = min(min(tMaxs.x, tMaxs.y), tMaxs.z);

    // If intersection is empty, no collision
    if (t0 >= t1)
    {
        return -1.0;
    }

    return t0;
}

// Returns distance to first intersection > 0, or a negative value
// Note(MartinR): Based on the calculation from here:
//  https://lousodrome.net/blog/light/2017/01/03/intersection-of-a-ray-and-a-cone/
float rayConeIntersection(
    float3 rayOrigin, float3 rayDir, float3 coneOrigin, float3 coneDir, float halfAngle, float coneHeight, out float3 normal)
{
    normal = float3(0, 1, 0);

    // Cone intersection equation
    float cosSquared = cos(halfAngle) * cos(halfAngle);
    float3 co = rayOrigin - coneOrigin;
    float dotRayConeDir = dot(rayDir, coneDir);
    float dotCoRayDir = dot(co, rayDir);
    float dotCoConeDir = dot(co, coneDir);

    float a = dotRayConeDir * dotRayConeDir - cosSquared;
    float b = 2 * (dotRayConeDir * dotCoConeDir - dotCoRayDir * cosSquared);
    float c = dotCoConeDir * dotCoConeDir - dot(co, co) * cosSquared;

    // Get both cone intersection distances 
    float delta = b * b - 4 * a * c;
    if (delta < 0.0)
        return -1.0;
    float t1 = (-b + sqrt(delta)) / (2 * a);
    float t2 = (-b - sqrt(delta)) / (2 * a);

    // Check which/if any of the two intersections are valid
    // (The formula provides intersections for infinite cones that extend on both sides of coneOrigin)
    if (t1 > t2)
    { // Sort intersections
        float swap = t1;
        t1 = t2;
        t2 = swap;
    }

    float tCone = -1.0;
    if (t1 >= 0.0)
    {
        // Check if intersection is on "wrong side" of the cone
        float3 intersection = rayOrigin + t1 * rayDir;
        float distanceAlongCone = dot(intersection - coneOrigin, coneDir);
        if (distanceAlongCone >= 0.0 && distanceAlongCone <= coneHeight)
        {
            tCone = t1;
        }
    }

    // Check if t2 is valid if t1 isn't
    if (tCone < 0.0 && t2 >= 0.0)
    {
        float3 intersection = rayOrigin + t2 * rayDir;
        float distanceAlongCone = dot(intersection - coneOrigin, coneDir);
        if (distanceAlongCone >= 0.0 && distanceAlongCone <= coneHeight)
        {
            tCone = t2;
        }
    }

    // Check if we hit the "Cone-Top plate" before the cone (hit from above)
    float4 planeEq = float4(coneDir.x, coneDir.y, coneDir.z, -(dot(coneDir, coneOrigin) + coneHeight));
    float distanceToPlane = dot(planeEq, float4(rayOrigin.x, rayOrigin.y, rayOrigin.z, 1.0));
    float tPlane = distanceToPlane / dot(rayDir, -coneDir);
    if (tPlane > 0.0 && (tPlane <= tCone || tCone < 0.0))
    {
        float3 planeIntersection = rayOrigin + rayDir * tPlane;
        // Check if plane intersection is a point inside the cone
        if (dot(normalize(planeIntersection - coneOrigin), coneDir) >= cos(halfAngle))
        {
            normal = coneDir;
            return tPlane;
        }
    }

    // Get cone normal
    float3 coneOriginToIntersection = rayOrigin + tCone * rayDir - coneOrigin;
    normal = normalize(cross(cross(coneDir, coneOriginToIntersection), coneOriginToIntersection));

    return tCone;
}

// Note: Just uses a single hardcoded directional light and hardcoded material values for simple shading
float3 phongShading(float3 viewDir, float3 normal, float3 materialColor)
{
    const float3 lightDir = normalize(float3(1, -1, 1));
    const float3 lightColor = float3(1, 1, 1);
    const float lightStrength = 0.5;
    const float3 ambientColor = float3(1, 1, 1);
    const float specularCoefficient = 10.0;
    const float ambientStrength = 0.1;

    float3 color = float3(0, 0, 0);
    color += ambientColor * materialColor * ambientStrength;
    color += materialColor * lightColor * max(0.0, dot(normal, -lightDir)) * lightStrength;
    color += lightColor * lightStrength * pow(max(0.0, dot(reflect(viewDir, normal), -lightDir)), specularCoefficient);
    return color;
}

bool boxContainsPoint(float3 pos, float3 boxMin, float3 boxMax)
{
    return
        pos.x >= boxMin.x && pos.x <= boxMax.x &&
        pos.y >= boxMin.y && pos.y <= boxMax.y &&
        pos.z >= boxMin.z && pos.z <= boxMax.z;
}


float3 colorRamp5PointGetValue(float alpha)
{
    float3 minColor = float3(0, 0, 0);
    float3 maxColor = float3(0, 0, 0);
    float t = 0.0;
    if (alpha < 1.0 / 4.0)
    {
        minColor = float3(0, 0, 1);
        maxColor = float3(0, 1, 1);
        t = alpha / (1.0 / 4.0);
    }
    else if (alpha < 2.0 / 4)
    {
        minColor = float3(0, 1, 1);
        maxColor = float3(0, 1, 0);
        t = (alpha - 1.0 / 4.0) / (1.0 / 4.0);
    }
    else if (alpha < 3.0 / 4)
    {
        minColor = float3(0, 1, 0);
        maxColor = float3(1, 1, 0);
        t = (alpha - 2.0 / 4.0) / (1.0 / 4.0);
    }
    else
    {
        minColor = float3(1, 1, 0);
        maxColor = float3(1, 0, 0);
        t = (alpha - 3.0 / 4.0) / (1.0 / 4.0);
    }
    t = clamp(t, 0.0, 1.0);
    return lerp(minColor, maxColor, t);
}


#endif
