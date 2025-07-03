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
            #include "ShaderUtils.cginc"

			#define PI 3.14159265359

            // UNIFORMS
            uniform sampler2D _MainTex;
            uniform sampler2D _CameraDepthTexture; // Set automatically by unity, see VectorFieldFullscreenLogic
            uniform float4x4 _InverseView;

            uniform float4 _BoxMin; // xyz is box min, w is used for cell-size (Sidelength of a cell)
            uniform int _FieldResolutionX;
            uniform int _FieldResolutionY;
            uniform int _FieldResolutionZ;
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

            // STRUCTS
            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
            };

            // VERTEX SHADER
            v2f vert (appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                return o;
            }

            // FRAMGENT SHADER start
            // xyz contain color, w distance on ray, or negative
            float4 rayGridTraversal(float3 rayOrigin, float3 rayDir, float maxDist)
            {
                // Grid setup
                int3 gridResolution = int3(_FieldResolutionX, _FieldResolutionY, _FieldResolutionZ);
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
                int3 cellCoord = clamp((int3) (rayOrigin / cellSize), int3(0, 0, 0), gridResolution - 1);

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
                        int linearCoord = cellCoord.x + cellCoord.y * gridResolution.x + cellCoord.z * gridResolution.x * gridResolution.y;
                        float4 vectorFieldValuePacked = _VectorFieldValues[linearCoord];

                        // Figure out interpolation coefficients based on magnitude and potential size-scaling based on efield magnitude
                        float efieldMagnitude = length(vectorFieldValuePacked.xyz);
                        float tMagnitude = efieldMagnitude / _MaxMagnitude;
                        tMagnitude = min(tMagnitude, 1.0);
                        tMagnitude = pow(tMagnitude, 1.0 / _MagnitudeInterpolationExponent);

                        float tPotential = (vectorFieldValuePacked.w - _VoltageCenter) / _VoltageRange;
                        tPotential = pow(min(abs(tPotential), 1.0), 1.0 / _VoltageInterpolationExponent);

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
                    if (cellCoord.x < 0 || cellCoord.x >= gridResolution.x ||
                        cellCoord.y < 0 || cellCoord.y >= gridResolution.y ||
                        cellCoord.z < 0 || cellCoord.z >= gridResolution.z ||
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
                return outputColor;
            }
            ENDCG
        }
    }
}



