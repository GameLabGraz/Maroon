Shader "Custom/DemoToFScanOverlay"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ScanColorTex ("Scan Color Rendertexutre", 2D) = "white" {}
        _ScanDepthTex ("Scan Depth Rendertexture", 2D) = "white" {}

        _DepthTolerance ("Depth tolerance", Float) = 0.001
        _SkipFarDepth ("SkipFarDepth", Float) = 0.999
        _ColourIntensity ("Colour Intensity", Float) = 0.85
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

            float _DepthTolerance;
            float _SkipFarDepth;
            float _ColourIntensity;

            float4x4 _PlayerClipToWorld;
            float4x4 _PlayerClipToScanClip;
            float4x4 _PlayerClipToScanView;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float4 baseCol = tex2D(_MainTex, uv);

                // If frag is very far away
                float playerDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
                if (playerDepth >= _SkipFarDepth)
                {
                    return baseCol;
                }

                // Open GL vs D3D difference. One goes from 0 to 1 one the other way
                #if UNITY_REVERSED_Z
                    float clipZ = playerDepth;
                #else
                    float clipZ = playerDepth * 2.0 - 1.0;
                #endif
                float2 ndc = uv * 2.0 - 1.0;
                float4 playerClipPos = float4(ndc.x, ndc.y, clipZ, 1.0);

                // From Player camera to Scan camera clip space
                float4 scanClipPos = mul(_PlayerClipToScanClip, playerClipPos);

                // if scan clip is behind player return early
                if (scanClipPos.w <= 0.0) 
                {
                    return baseCol;
                }
                
                // convert to 2D scan camera  screen space (UV)
                float3 scanNDCPos = scanClipPos.xyz / scanClipPos.w;
                float2 scanUVPos = scanNDCPos.xy * 0.5 + 0.5;

                // if frag projected to scan frustrum is outside of said frustrum return early
                bool inside = (scanUVPos.x >= 0.0 && scanUVPos.x <= 1.0 && scanUVPos.y >= 0.0 && scanUVPos.y <= 1.0);
                if (!inside)
                {
                    return baseCol;
                }
                
                // Transforms frag depth from player clip -> world space -> scanViewport 
                // to get the distance of frag to scan camera
                // if negative it is behind the scan frustrum -> return

                // In clip space Z is a non linear depth value and w the distance to camera
                // w linear depth in 3d space
                // 
                // only care about depth so singular DOT products of Transformation Matrix for efficiency.
                float w = dot(_PlayerClipToWorld[3], playerClipPos);
                float scanViewZRaw = dot(_PlayerClipToScanView[2], playerClipPos);
                float distanceFromScan = -(scanViewZRaw / w);
                if (distanceFromScan < _DepthTolerance) 
                {
                    return baseCol;
                }

                #if UNITY_REVERSED_Z
                    float scanDepth01 = scanNDCPos.z;
                #else
                    // OpenGL Z is [-1, 1] so map to [0,1]
                    float scanDepth01 = scanNDCPos.z * 0.5 + 0.5;
                #endif

                // Occlusion check with Depth Tolerance  
                float sampledScanDepth = SAMPLE_DEPTH_TEXTURE(_ScanDepthTex, scanUVPos);
                if (abs(sampledScanDepth - scanDepth01) > _DepthTolerance)
                {
                    return baseCol;
                }


                float4 sampledScanColor = tex2D(_ScanColorTex, scanUVPos);
                return lerp(baseCol, sampledScanColor, saturate(_ColourIntensity));
            }
            ENDCG
        }
    }
}
