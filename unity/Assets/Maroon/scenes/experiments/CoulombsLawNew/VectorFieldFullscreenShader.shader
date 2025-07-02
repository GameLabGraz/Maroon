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
            uniform float _ArrowSize;

            uniform int _ColorMode;        // 0 = constant, 1 = potential, 2 = magnitude
            uniform int _ScaleMode;        // 0 = constant, 1 = potential, 2 = magnitude
            uniform int _TransparencyMode; // 0 = constant, 1 = potential, 2 = magnitude, 3 = FixedValue
            uniform float _FixedTransparencyValue;

            uniform float _MaxMagnitude;
            uniform float _MagnitudeInterpolationExponent;

            uniform float _VoltageCenter;
            uniform float _VoltageRange;
            uniform float _VoltageInterpolationExponent;
            uniform int _DisplayOutsideOfRangeBool;

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
                if (alpha < 1.0 / 4.0) {
                    minColor = float3(0, 0, 1);
                    maxColor = float3(0, 1, 1);
                    t = alpha / (1.0/4.0);
                }
                else if (alpha < 2.0 / 4) {
                    minColor = float3(0, 0, 1);
                    maxColor = float3(0, 1, 1);
                    t = (alpha - 1.0/4.0) / (1.0/4.0);
                }
                else if (alpha < 3.0 / 4) {
                    minColor = float3(0, 1, 0);
                    maxColor = float3(1, 1, 0);
                    t = (alpha - 2.0/4.0) / (1.0/4.0);
                }
                else {
                    minColor = float3(1, 1, 0);
                    maxColor = float3(1, 0, 0);
                    t = (alpha - 3.0/4.0) / (1.0/4.0);
                }
                t = clamp(t, 0.0, 1.0);
                return lerp(minColor, maxColor, t);
            }

            // xyz contain color, w distance on ray, or negative
            float4 rayGridTraversal(float3 rayOrigin, float3 rayDir, float maxDist)
            {
                // Grid setup
                const int gridResolution = _FieldResolution; // Alias for easier use/portability
                float cellSize = _CellSize; // Alias for easier use/portability
                float3 gridMin = _BoxMin;
                float3 gridMax = _BoxMin + gridResolution * cellSize;

                // Update ray-origin (Make gridMin the origin of the coordinate system)
                float tBox = 0.0; // distance to box, 0 if start is inside box
                if (boxContainsPoint(rayOrigin, gridMin, gridMax))
                {
                    rayOrigin = rayOrigin - gridMin;
                }
                else 
                {
                    // Check if ray hits grid-box
                    tBox = rayBoxIntersection(rayOrigin, rayDir, gridMin, gridMax);
                    if (tBox < 0 || tBox >= maxDist) return float4(0, 0, 0, 0);

                    rayOrigin = rayOrigin + tBox * rayDir - gridMin; // Move ray-origin to coordinate-system where grid_min is at 0
                    maxDist -= tBox; // Since we moved forward on the ray, we need to update maxDist
                }

                // Find integer coordinates of grid at box-intersection
                int3 cellCoord = clamp((int3) (rayOrigin / cellSize), int3(0, 0, 0), (gridResolution - 1) * int3(1, 1, 1));

                // Grid traversal starts here
                int3 dirSign = int3(rayDir.x > 0 ? 1 : -1, rayDir.y > 0 ? 1 : -1, rayDir.z > 0 ? 1 : -1);
                float3 nextBoundary = ((float3) cellCoord + (sign(rayDir) / 2.0 + 0.5)) * cellSize - rayOrigin;
                float3 tCellMax = nextBoundary / rayDir; // distance on ray until next x/y/z cell boundary
                float3 tDelta = cellSize * float3(1, 1, 1) / abs(rayDir);
                float tGrid = 0.0;

                int cellsTraversed = 0;
                float3 resultColor = float4(0, 0, 0, 0);
                float resultAlpha = 0.0;
                while (cellsTraversed < 250) // Just a hardcoded limit so we don't run this loop forever if there are bugs
                {
                    // Handle cell logic here
                    float3 cellColor = float3(0, 0, 0);
                    float cellAlpha = 0.0;
                    {
                        // Get cell information
                        float3 cellCenter = (float3(cellCoord) + 0.5) * cellSize;
                        int linearCoord = cellCoord.x + cellCoord.y * _FieldResolution + cellCoord.z * _FieldResolution * _FieldResolution;
                        float4 vectorFieldValuePacked = _VectorFieldValues[linearCoord];

                        // Figure out interpolation coefficients based on magnitude and potential size-scaling based on efield magnitude
                        float efieldMagnitude = length(vectorFieldValuePacked.xyz);
                        float tMagnitude = efieldMagnitude / _MaxMagnitude;
                        tMagnitude = min(tMagnitude, 1.0);
                        tMagnitude = pow(tMagnitude, _MagnitudeInterpolationExponent);

                        float tPotential = (vectorFieldValuePacked.w - _VoltageCenter) / _VoltageRange;
                        tPotential = pow(min(abs(tPotential), 1.0), _VoltageInterpolationExponent);

                        // Check if we want to skip cell
                        bool skipCell = false;
                        if (_DisplayOutsideOfRangeBool == 0) {
                            float potential = vectorFieldValuePacked.w;
                            if (abs(potential - _VoltageCenter) > _VoltageRange) {
                                skipCell = true;
                            }
                        }

                        if (!skipCell)
                        {
                            // Find cone parameters
                            float coneHeight = cellSize * 0.9 * _ArrowSize; // 0.9 so rotations don't extend the cone outwards of the cell
                            if (_ScaleMode == 1)
                            {
                                coneHeight *= tPotential;
                            }
                            else if (_ScaleMode == 2) 
                            {
                                coneHeight *= tMagnitude;
                            }
                            coneHeight = min(coneHeight, 0.3); // Apply max cone height (So low resolution settings don't create large arrows)

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
                            float tCone = rayConeIntersection(
                                rayOrigin, rayDir, cellCenter - coneDir * coneHeight / 2.0, coneDir, coneHalfAngle, coneHeight, coneNormal);
                            if (tCone > 0.0 && tCone < maxDist) 
                            {
                                // Find cone color
                                float3 matColor = float3(0.5, 0.5, 0.5);
                                if (_ColorMode == 1) 
                                {
                                    float3 baseColor = (vectorFieldValuePacked.w - _VoltageCenter) >= 0.0 ? float3(1.0, 0.0, 0.0) : float3(0.0, 0.0, 1.0);
                                    matColor = lerp(matColor, baseColor, tPotential);
                                }
                                else if (_ColorMode == 2)
                                {
                                    matColor = colorRamp5PointGetValue(tMagnitude);
                                }

                                // Do shading
                                cellColor = phongShading(rayDir, coneNormal, matColor);

                                // Determine alpha
                                cellAlpha = 1.0;
                                if (_TransparencyMode == 1) 
                                {
                                    cellAlpha = tPotential;
                                }
                                else if (_TransparencyMode == 2) 
                                {
                                    cellAlpha = tMagnitude;
                                }
                                else if (_TransparencyMode == 3) {
                                    cellAlpha = _FixedTransparencyValue;
                                }
                            }
                        }
                    }

                    // Accumulate colors with "under" blending
                    resultColor = (1.0 - resultAlpha) * cellColor + resultColor;
                    resultAlpha = (1.0 - resultAlpha) * cellAlpha + resultAlpha;

                    // Traverse to next cell
                    cellsTraversed += 1;
                    if (tCellMax.x <= tCellMax.y && tCellMax.x <= tCellMax.z) 
                    {
                        cellCoord.x += dirSign.x;
                        tCellMax.y -= tCellMax.x;
                        tCellMax.z -= tCellMax.x;
                        tGrid += tCellMax.x;
                        tCellMax.x = tDelta.x;
                    }
                    else if (tCellMax.y <= tCellMax.z) 
                    {
                        cellCoord.y += dirSign.y;
                        tCellMax.x -= tCellMax.y;
                        tCellMax.z -= tCellMax.y;
                        tGrid += tCellMax.y;
                        tCellMax.y = tDelta.y;
                    }
                    else 
                    {
                        cellCoord.z += dirSign.z;
                        tCellMax.x -= tCellMax.z;
                        tCellMax.y -= tCellMax.z;
                        tGrid += tCellMax.z;
                        tCellMax.z = tDelta.z;
                    }

                    // Check for stop conditions (Stepped out of cell, reached maxDist, color fully saturated/opague)
                    if (cellCoord.x < 0 || cellCoord.x >= gridResolution ||
                        cellCoord.y < 0 || cellCoord.y >= gridResolution ||
                        cellCoord.z < 0 || cellCoord.z >= gridResolution ||
                        tGrid >= maxDist || 
                        resultAlpha >= 1.0) 
                    {
                        break;
                    }
                }

                return float4(resultColor.x, resultColor.y, resultColor.z, resultAlpha);

                // Test code for grid visualization (Feel free to delete)
                // result.xyz = float3(1, 1, 1) * pow(((float)cellsTraversed) / (float)(gridResolution*4), 1.0 / 2.0);
                // result.xyz = float3(1, 1, 1);
                // result.w = stepsPerAxis.y <= 3 ? 1.0 : 0.0;

                // result.x = cellUV.x;
                // result.y = cellUV.y;
                // result.z = cellUV.z;
                // result.w =  1.0;

                // return result;
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
                float4 gridTraversalResult = rayGridTraversal(pos, dir, length(pos - worldPos));
                outputColor = lerp(outputColor, gridTraversalResult, gridTraversalResult.w);
                // if (gridTraversalResult.w > 0.0 && gridTraversalResult.w < length(pos - worldPos)) {
                //     gridTraversalResult.w = 1.0;
                //     return gridTraversalResult;
                // }
                return outputColor;
            }
            ENDCG
        }
    }
}



