Shader "Custom/IsoSurfaceShader"
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
            #include "ElectricFieldShaderUtils.cginc"

			#define PI 3.14159265359

            // UNIFORMS
            uniform sampler2D _MainTex;
            uniform sampler2D _CameraDepthTexture; // Set automatically by unity, see VectorFieldFullscreenLogic
            uniform float4x4 _InverseView;

            uniform float4 _BoxMin;
            uniform float4 _BoxMax;

            uniform float _Transparency;
            uniform int _UseMagnitudeAsColor;        // 0 = constant, 1 = potential, 2 = magnitude
            uniform float _MaxMagnitude;
            uniform float _MagnitudeInterpolationExponent;
            uniform float _VoltageCenter;

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
			float3 GetColorAtPos(float3 pos) 
			{
				if (_UseMagnitudeAsColor == 0) return float3(1, 1, 1);

				float3 fieldValue;
                float _unused;
				evaluateField(pos, _unused, fieldValue);

				// Mix colors based on magnitude
                float tMagnitude = length(fieldValue) / _MaxMagnitude;
                tMagnitude = min(tMagnitude, 1.0);
                tMagnitude = pow(tMagnitude, 1.0 / _MagnitudeInterpolationExponent);
			    return colorRamp5PointGetValue(tMagnitude);
			}

            // xyz contain color, w alpha
            float4 rayMarchIsoSurface(float3 rayOrigin, float3 rayDir, float maxDist)
            {
                const float STEP_SIZE = 0.2;
                const int BINARY_SEARCH_STEPS = 6;
                const float DISTANCE_AFTER_INTERSECTION = 0.01;
                float TARGET_POTENTIAL = _VoltageCenter;

                float3 gridMin = _BoxMin;
                float3 gridMax = _BoxMax;

                // Check if we hit scene bounding box
                float tBoxMax = 0.0;
                float tBoxMin = rayBoxIntersection(rayOrigin, rayDir, gridMin, gridMax, tBoxMax);
                if (tBoxMin >= tBoxMax || tBoxMax < 0.0 || (tBoxMin > 0.0 && maxDist < tBoxMin)) { // No intersection with box, or box behind us
                    return float4(0, 0, 0, 0);
                }

                float rayDistance = max(0.0, tBoxMin);
                maxDist = min(maxDist, tBoxMax);
				float currentPotential;
				float3 vectorValue;
				evaluateField(rayOrigin + rayDir * rayDistance, currentPotential, vectorValue);

                bool lastAbove = currentPotential > TARGET_POTENTIAL;
                rayDistance += STEP_SIZE;

                float3 resultColor = float3(0, 0, 0);
                float resultAlpha = 0.0;
                while (rayDistance - 2.0 * STEP_SIZE < maxDist)
                {
                    // Get potential at current point
				    evaluateField(rayOrigin + rayDir * rayDistance, currentPotential, vectorValue);

                    // Check if we crossed over target potential
                    bool above = currentPotential > TARGET_POTENTIAL;
                    if (lastAbove != above)
                    {
                        // Binary search to refine intersection distance
                        float minT = rayDistance - STEP_SIZE;
                        float maxT = rayDistance;
                        float midpointPotential = 0.0;
                        for (int i = 0; i < BINARY_SEARCH_STEPS; i++)
                        {
                            float midT = (minT + maxT) / 2.0;
				            evaluateField(rayOrigin + rayDir * midT, midpointPotential, vectorValue);
                            bool takeUpperInterval = midpointPotential < TARGET_POTENTIAL;
                            if (!above) takeUpperInterval = !takeUpperInterval;
                            if (takeUpperInterval) 
                            {
                                minT = midT;
                            }
                            else 
                            {
                                maxT = midT;
                            }
                        }

                        // Check intersection distance
                        float intersectionT = (maxT + minT) / 2.0;
                        if (intersectionT > maxDist) {
                            return float4(resultColor.x, resultColor.y, resultColor.z, resultAlpha);
                        }

                        // Do surface shading
                        float3 normal = normalize(vectorValue);
                        // if (dot(normal, -rayDir) < 0.0) { // Shading should work on both sides 
                        //     normal = -normal;
                        // }
                        float3 materialColor = GetColorAtPos(rayOrigin + rayDir * intersectionT);

                        // Over-Blending
                        float3 intersectionColor = phongShading(rayDir, normal, materialColor);
                        float intersectionAlpha = _Transparency;
                        resultColor = (1.0 - resultAlpha) * intersectionColor + resultColor;
                        resultAlpha = (1.0 - resultAlpha) * intersectionAlpha + resultAlpha;
                        if (resultAlpha > 0.99) {
                            return float4(resultColor.x, resultColor.y, resultColor.z, resultAlpha);
                        }

                        rayDistance = intersectionT + DISTANCE_AFTER_INTERSECTION;
                    }

                    // Continue Stepping
                    lastAbove = above;
                    rayDistance += STEP_SIZE;
                }

                return float4(resultColor.x, resultColor.y, resultColor.z, resultAlpha);
            }

			// Note(MartinR): This is copy-pasted from VectorFieldFullscreenShader, because it seems like
			//		we cannot move this to a shared source file "e.g. ShaderUtils.cginc" because of the unity defines...
            void getPixelAndCameraWorldPositions(float2 uv, float4x4 inverseView, out float3 pixelPos, out float3 cameraPos)
            {
                // Note: Camera depth-texture-mode needs to be set for this to work, see FullscreenShadervisualizations.cs
                float depth = tex2D(_CameraDepthTexture, uv).r;
                #if UNITY_REVERSED_Z
                    depth = 1.0 - depth;
                #endif
            
                // Transform to NDC
                depth = depth * 2.0 - 1.0;
                float4 ndcPos = float4(uv.x * 2 - 1, uv.y * 2 - 1, depth, 1.0);
            
                // Transform from NDC to world position
                ndcPos = mul(unity_CameraInvProjection, ndcPos);
                ndcPos = mul(inverseView, ndcPos);
                pixelPos = ndcPos.xyz / ndcPos.w;
            
                // Handle orthographic camera
                cameraPos = _WorldSpaceCameraPos;
                if (unity_OrthoParams.w > 0.5)
                {
                    float4 orthoOffset = float4(0, 0, 0, 0);
                    orthoOffset.x = (uv.x * 2.0 - 1.0) * unity_OrthoParams.x;
                    orthoOffset.y = (uv.y * 2.0 - 1.0) * unity_OrthoParams.y;
                    orthoOffset = mul(inverseView, orthoOffset);
                    cameraPos = cameraPos + orthoOffset.xyz;
                }
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.position.xy / _ScreenParams.xy;

                float3 cameraPos;
                float3 pixelPos; // Position of pixel in world-space, reconstructed from uv-coordinate and depth-buffer
                getPixelAndCameraWorldPositions(uv, _InverseView, pixelPos, cameraPos);
                float3 dir = normalize(pixelPos - cameraPos);

                float4 overlayColor = rayMarchIsoSurface(cameraPos, dir, length(cameraPos - pixelPos));
                float4 backbufferColor = tex2D(_MainTex, uv);
                return lerp(backbufferColor, overlayColor, overlayColor.w);
            }
            ENDCG
        }
    }
}



