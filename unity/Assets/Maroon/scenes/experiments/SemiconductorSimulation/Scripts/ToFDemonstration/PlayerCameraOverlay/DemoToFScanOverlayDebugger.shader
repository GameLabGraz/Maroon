Shader "Custom/DemoToFScanOverlayDebugger"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ScanColorTex ("Scan Color Rendertexutre", 2D) = "white" {}
        _ScanDepthTex ("Scan Depth Rendertexture", 2D) = "white" {}

        _DepthTolerance ("Depth tolerance", Float) = 0.001
        _SkipFarDepth ("SkipFarDepth", Float) = 0.999
        _ColourIntensity ("Colour Intensity", Float) = 0.85

        _WorldCenter ("WorldCenter", Vector) = (0, 0, 0, 0)
        _WorldScale ("WorldScale", Float) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _ScanColorTex;
            UNITY_DECLARE_DEPTH_TEXTURE(_ScanDepthTex);
            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            float _Mode;
            float _DepthTolerance;
            float _SkipFarDepth;
            float _ColourIntensity;

            float4 _WorldCenter;
            float _WorldScale;

            float4x4 _PlayerInvProjection;
            float4x4 _PlayerCameraToWorld;

            float4x4 _ScanViewProj;
            float4x4 _ScanWorldToCamera;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert (appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float4 baseCol = tex2D(_MainTex, uv);

                // If frag is very far away
                float playerDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
                if (playerDepth >= _SkipFarDepth)
                    return baseCol;

                // Convert Fragment Position through Rendertextures to Clip space
                // Open GL vs D3D difference. One goes from 0 to 1 one the other way
                #if UNITY_REVERSED_Z
                    float clipZ = playerDepth;
                #else
                    float clipZ = playerDepth * 2.0 - 1.0;
                #endif
                float2 playerNDCXY = uv * 2.0 - 1.0;
                float4 playerNDCPos = float4(playerNDCXY.x, playerNDCXY.y, clipZ, 1.0);

                // From ndc to view to worldspace
                float4 playerViewPosRaw = mul(_PlayerInvProjection, playerNDCPos);
                float4 playerViewPos = playerViewPosRaw / playerViewPosRaw.w;
                float3 worldPos = mul(_PlayerCameraToWorld, playerViewPos).xyz;

                // From worldspace to scan camera view space
                float4 scanViewPos = mul(_ScanWorldToCamera, float4(worldPos, 1.0));

                // Unity camera looks in -z direction -> in front of camera
                float distanceFromScan = -scanViewPos.z;
                bool isInFrontOfScanCamera = (distanceFromScan > 0.0);

                float4 scanClipPos = mul(_ScanViewProj, float4(worldPos, 1.0));

                bool isBehindScanCamera = (scanClipPos.w <= 0.0);

                float3 scanNDCPos = scanClipPos.xyz / scanClipPos.w;
                float2 scanUVPos = scanNDCPos.xy * 0.5 + 0.5;

                // convert projected depth to [0,1] space matching scan depth texture
                #if UNITY_REVERSED_Z
                    float scanProjectedDepth01 = scanNDCPos.z;
                #else
                    // OpenGL Z is [-1, 1] so map to [0,1]
                    float scanProjectedDepth01 = scanNDCPos.z * 0.5 + 0.5;
                #endif

                // does scan camera see the point translated
                bool inside = (scanUVPos.x >= 0.0 && scanUVPos.x <= 1.0 && scanUVPos.y >= 0.0 && scanUVPos.y <= 1.0);
                float sampledScanDepth = 0.0;
                float4 sampledScanColor = float4(0,0,0,1);
                if (inside)
                {
                    sampledScanDepth = SAMPLE_DEPTH_TEXTURE(_ScanDepthTex, scanUVPos);
                    sampledScanColor = tex2D(_ScanColorTex, scanUVPos);
                }

                // Occlusion check with Depth Tolerance  
                bool visibleToScan = inside && !isBehindScanCamera && abs(sampledScanDepth - scanProjectedDepth01) <= _DepthTolerance;

                // near plane check
                bool tooCloseToScan = !isInFrontOfScanCamera || distanceFromScan < _DepthTolerance;

                // Debugger Modes
                // Mode 1: base
                if (_Mode < 1.5)
                {
                    return baseCol;
                }
                // Mode 2: worldPos
                else if (_Mode < 2.5) 
                {
                    float3 c = (worldPos - _WorldCenter.xyz) / max(0.0001, _WorldScale);
                    c = saturate(c * 0.5 + 0.5);
                    return fixed4(c, 1);
                }
                // Mode 3: ScanUVPos
                else if (_Mode < 3.5)
                {
                    return fixed4(scanUVPos.x, scanUVPos.y, 0, 1);
                }
                // Mode 4: scanColor
                else if (_Mode < 4.5)
                {
                    if (!inside)
                    {
                        return fixed4(0,0,0,1);
                    }
                    return sampledScanColor;
                }
                // Mode 5: scanDepth
                else if (_Mode < 5.5)
                {
                    if (!inside)
                    {
                        return fixed4(0,0,0,1);
                    }
                    return fixed4(sampledScanDepth, sampledScanDepth, sampledScanDepth, 1);
                } 
                // Mode 6: projected depth
                else if (_Mode < 6.5)
                {
                    return fixed4(scanProjectedDepth01, scanProjectedDepth01, scanProjectedDepth01, 1);
                }
                // Mode 7: visability
                else if (_Mode < 7.5)
                {
                    return visibleToScan ? fixed4(1,1,1,1) : fixed4(0,0,0,1);
                }
                // Mode 8: isBehindScanCamera
                else if (_Mode < 8.5)
                {
                    return isBehindScanCamera ? fixed4(1,1,0,1) : fixed4(0,0,0,1);
                }
                // Mode 9: Viewpoint Depth
                else if (_Mode < 9.5) 
                {
                    float w = playerViewPosRaw.w;
                    float m = saturate((w + 10.0) / 20.0);
                    return fixed4(m, m, m, 1);
                }
                // Mode 10: All Filters together in order
                else if (_Mode < 10.5) 
                {
                    // Red
                    if (!inside)
                    {
                         return fixed4(1,0,0,1);   
                    }
                    // Yellow
                    if (isBehindScanCamera) 
                    {
                        return fixed4(1,1,0,1); 
                    }
                    // Magenta
                    if (tooCloseToScan) 
                    {
                        return fixed4(1,0,1,1); 
                    }
                    // blue
                    if (!visibleToScan) 
                    {
                        return fixed4(0,0,1,1); 
                    }                        

                    // Green
                    return fixed4(0,1,0,1);
                }
                // >Mode 11: Final Result
                else 
                {
                    if (inside && !isBehindScanCamera && !tooCloseToScan && visibleToScan)
                    {
                        float intensity = saturate(_ColourIntensity);
                        return lerp(baseCol, sampledScanColor, intensity);
                    }
                    // unmodified player color
                    return baseCol;
                }
            }
            ENDCG
        }
    }
}
