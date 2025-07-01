Shader "Custom/VectorFieldFullscreenShader"
{
    Properties
    {
        [HideInInspector] _MainTex ("MainTexture", 2D) = "white" {}
    }
    SubShader
    {
        // Tags { "RenderType"="Opaque" }
        // LOD 100

        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			#define PI 3.14159265359

            uniform sampler2D _MainTex;
            uniform sampler2D _CameraDepthTexture; // Set automatically by unity, see VectorFieldFullscreenLogic
            uniform float4x4 _InverseView;

            uniform float4 _BoxMin; // xyz is box min, w is used for cell-size (Sidelength of a cell)
            uniform int _FieldResolution;
            uniform float _CellSize;

            uniform int _EnableScalingBool;
            uniform float _MinMagnitude;
            uniform float _MaxMagnitude;
            uniform float _SizeInterpolationExponent;
            uniform float _ArrowSize;
            uniform float _MinArrowSize;
            uniform int _CutoffAboveMaxBool;

            uniform StructuredBuffer<float4> _VectorFieldValues; // xyz is efield vector value at grid positions, w is potential



            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                return o;
            }



            // Returns distance to intersection, or -1 if not hit
            float raySphereIntersection(float3 rayOrigin, float3 dir, float3 spherePos, float radius, out float3 normal)
            {
                normal = float3(1, 0, 0);

                float3 toCenter = spherePos - rayOrigin;
                float t_closest = dot(toCenter, dir);
                if (t_closest < 0.0) return -1.0;

                float r2 = radius * radius;
                float d2 = dot(toCenter, toCenter) - t_closest * t_closest;
                float offset = r2 - d2;
                if (offset < 0.0) return -1.0;
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
                if (t0 >= t1) {
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
                float delta = b*b - 4 * a * c;
                if (delta < 0.0) return -1.0;
                float t1 = (-b + sqrt(delta)) / (2 * a);
                float t2 = (-b - sqrt(delta)) / (2 * a);

                // Check which/if any of the two intersections are valid
                // (The formula provides intersections for infinite cones that extend on both sides of coneOrigin)
                if (t1 > t2) { // Sort intersections
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
                    if (distanceAlongCone >= 0.0 && distanceAlongCone <= coneHeight) {
                        tCone = t1;
                    }
                }

                // Check if t2 is valid if t1 isn't
                if (tCone < 0.0 && t2 >= 0.0) 
                {
                    float3 intersection = rayOrigin + t2 * rayDir;
                    float distanceAlongCone = dot(intersection - coneOrigin, coneDir);
                    if (distanceAlongCone >= 0.0 && distanceAlongCone <= coneHeight) {
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
                    if (dot(normalize(planeIntersection - coneOrigin), coneDir) >= cos(halfAngle)) {
                        normal = coneDir;
                        return tPlane;
                    }
                }

                // Get cone normal
                float3 coneOriginToIntersection = rayOrigin + tCone * rayDir - coneOrigin;
                normal = normalize(cross(cross(coneDir, coneOriginToIntersection), coneOriginToIntersection));

                return tCone;
            }

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

            // xyz contain color, w distance on ray, or negative
            float4 rayGridTraversal(float3 rayOrigin, float3 rayDir)
            {
                // Grid setup
                const int gridResolution = _FieldResolution; // Alias for easier use/portability
                float cellSize = _CellSize; // Alias for easier use/portability
                float3 gridMin = _BoxMin;
                float3 gridMax = _BoxMin + gridResolution * cellSize;

                // Check if ray hits grid-box
                float4 result = float4(0, 0, 0, 0);
                float tBox = rayBoxIntersection(rayOrigin, rayDir, gridMin, gridMax);
                if (tBox < 0) return result;

                // Find integer coordinates of grid at box-intersection
                float3 intersectionPoint = rayOrigin + tBox * rayDir;
                rayOrigin = intersectionPoint - gridMin; // Move ray-origin to coordinate-system where grid_min is at 0
                int3 cellCoord = clamp((int3) (rayOrigin / cellSize), int3(0, 0, 0), (gridResolution - 1) * int3(1, 1, 1));

                // Grid traversal starts here
                int3 dirSign = int3(rayDir.x > 0 ? 1 : -1, rayDir.y > 0 ? 1 : -1, rayDir.z > 0 ? 1 : -1);
                float3 nextBoundary = ((float3) cellCoord + (sign(rayDir) / 2.0 + 0.5)) * cellSize - rayOrigin;
                float3 tMax = nextBoundary / rayDir;
                float3 tDelta = cellSize * float3(1, 1, 1) / abs(rayDir);

                int cellsTraversed = 0;
                while (cellsTraversed < 250) // Just a hardcoded limit so we don't run this loop forever if there are bugs
                {
                    // Handle cell logic here
                    {
                        // Get cell information
                        float3 cellCenter = (float3(cellCoord) + 0.5) * cellSize;
                        int linearCoord = cellCoord.x + cellCoord.y * _FieldResolution + cellCoord.z * _FieldResolution * _FieldResolution;
                        float4 vectorFieldValuePacked = _VectorFieldValues[linearCoord];

                        // Figure out size-scaling based on efield magnitude
                        float alpha = 1.0;
                        float efieldMagnitude = length(vectorFieldValuePacked.xyz);
                        if (_EnableScalingBool != 0)
                        {
                            alpha = (efieldMagnitude - _MinMagnitude) / (_MaxMagnitude - _MinMagnitude);
                            alpha = min(alpha, 1.0);
                            alpha = pow(alpha, _SizeInterpolationExponent);
                            alpha = max(alpha, _MinArrowSize);
                        }
                        if (_CutoffAboveMaxBool != 0 && efieldMagnitude > _MaxMagnitude) {
                            alpha = 0.0;
                        }

                        // Find cone parameters
                        float coneHeight = cellSize * 0.9 * alpha * _ArrowSize; // 0.9 so rotations don't extend the cone outwards of the cell
                        float3 coneDir;
                        if (efieldMagnitude < 0.000001) {
                            coneDir = float3(0, -1, 0);
                        }
                        else {
                            coneDir = -normalize(vectorFieldValuePacked.xyz);
                        }
                        float coneHalfAngle = 15.0 / 360.0 * 2 * PI;

                        // Ray-Cone intersection
                        float3 coneNormal;
                        float tCone = rayConeIntersection(rayOrigin, rayDir, cellCenter - coneDir * coneHeight / 2.0, coneDir, coneHalfAngle, coneHeight, coneNormal);
                        if (tCone > 0.0) {
                            float3 matColor = cellCoord / (gridResolution * float3(1, 1, 1));
                            result.xyz = phongShading(rayDir, coneNormal, matColor);
                            result.w = tBox + tCone;
                            return result;
                        }
                    }

                    // float sphereT = raySphereIntersection(rayOrigin, rayDir, cellCenter, cellSize/3.0, normal);
                    // if (sphereT > 0.0) {
                    //     float3 matColor = cellCoord / (gridResolution * float3(1, 1, 1));
                    //     result.xyz = phongShading(rayDir, normal, matColor);
                    //     result.w = 1.0;
                    //     return result;
                    // }

                    // Traverse to next cell
                    cellsTraversed += 1;
                    if (tMax.x <= tMax.y && tMax.x <= tMax.z) 
                    {
                        cellCoord.x += dirSign.x;
                        tMax.y -= tMax.x;
                        tMax.z -= tMax.x;
                        tMax.x = tDelta.x;
                    }
                    else if (tMax.y <= tMax.z) 
                    {
                        cellCoord.y += dirSign.y;
                        tMax.x -= tMax.y;
                        tMax.z -= tMax.y;
                        tMax.y = tDelta.y;
                    }
                    else 
                    {
                        cellCoord.z += dirSign.z;
                        tMax.x -= tMax.z;
                        tMax.y -= tMax.z;
                        tMax.z = tDelta.z;
                    }

                    // Check if we stepped out of grid
                    if (cellCoord.x < 0 || cellCoord.x >= gridResolution ||
                        cellCoord.y < 0 || cellCoord.y >= gridResolution ||
                        cellCoord.z < 0 || cellCoord.z >= gridResolution) 
                    {
                        break;
                    }
                }

                return result;
                // Visualize cellCoord
                result.xyz = float3(1, 1, 1) * pow(((float)cellsTraversed) / (float)(gridResolution*4), 1.0 / 2.0);
                // result.xyz = float3(1, 1, 1);
                // result.w = stepsPerAxis.y <= 3 ? 1.0 : 0.0;

                // result.x = cellUV.x;
                // result.y = cellUV.y;
                // result.z = cellUV.z;
                result.w =  1.0;

                return result;
            }

            // Returns possibly transparent color
            float4 traceRayThroughScene(float3 rayOrigin, float3 rayDir, float maxDist)
            {
                float4 result = float4(0, 0, 0, 0);

                float3 normal;
                float tSphere = raySphereIntersection(rayOrigin, rayDir, _BoxMin + (_FieldResolution * _CellSize) / 2.0, 0.2, normal);
                // tSphere = -1;
                float3 offset = float3(0, 1, 0);
                float tBox = rayBoxIntersection(rayOrigin, rayDir, float3(-1, -1, -1) + offset, float3(1, 1, 1) + offset);

                float closestDist = 1000.0;
                if (tSphere > 0.0) closestDist = min(closestDist, tSphere);
                // if (tBox > 0.0) closestDist = min(closestDist, tBox);
                if (closestDist > maxDist) return result;

                if (tSphere == closestDist) 
                {
                    float3 color = phongShading(rayDir, normal, float3(1, .3, .5));
                    result = float4(color.x, color.y, color.z, 1.0);
                }
                // else if (tBox == closestDist) 
                // {
                //     result = float4(1, 0, 0, 1);
                // }

                return result;
            }

            float3 pixelPosToWorldPos(float2 uv)
            {
                float depth = tex2D(_CameraDepthTexture, uv).r;
                depth = (1.0 - depth) * 2 - 1; // Note: From unity documentation this does not make sense, but is the only thing that works so far
                float4 clipPos = float4(uv.x * 2 - 1, uv.y * 2 - 1, depth, 1.0);

                float4 worldPos = mul(unity_CameraInvProjection, clipPos);
                worldPos = mul(_InverseView, worldPos);
                return worldPos.xyz / worldPos.w;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.position.xy / _ScreenParams.xy;
                // #if UNITY_UV_STARTS_AT_TOP
                //     uv.y = 1.0 - uv.y;
                // #endif

                float3 worldPos = pixelPosToWorldPos(uv);

                float3 pos = _WorldSpaceCameraPos;
                if (unity_OrthoParams.w > 0.5) {
                    float4 orthoOffset = float4(0, 0, 0, 0);
                    orthoOffset.x = (uv.x * 2.0 - 1.0) * unity_OrthoParams.x;
                    orthoOffset.y = (uv.y * 2.0 - 1.0) * unity_OrthoParams.y;
                    orthoOffset = mul(_InverseView, orthoOffset);
                    pos = pos + orthoOffset.xyz;
                }
                float3 dir = normalize(worldPos - pos);

                float4 outputColor = tex2D(_MainTex, uv);
                float4 gridTraversalResult = rayGridTraversal(pos, dir);
                if (gridTraversalResult.w > 0.0 && gridTraversalResult.w < length(pos - worldPos)) {
                    gridTraversalResult.w = 1.0;
                    return gridTraversalResult;
                }
                return outputColor;
            }
            ENDCG
        }
    }
}



