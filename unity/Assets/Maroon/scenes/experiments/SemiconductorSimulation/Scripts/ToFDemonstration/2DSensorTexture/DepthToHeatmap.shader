Shader "Custom/DepthToHeatmap"
{
    SubShader
    {
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _MaxDistance;
            float4x4 _SensorClipToView;

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            // HSV has hue, saturation, value
            // from https://www.laurivan.com/rgb-to-hsv-to-rgb-for-shaders/
            float3 HSVtoRGB(float3 hsv)
            {
                float4 k = float4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
                float3 p = abs(frac(hsv.xxx + k.xyz) * 6.0 - k.www);
                return hsv.z * lerp(k.xxx, saturate(p - k.xxx), hsv.y);
            }

            v2f vert(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }
            

            fixed4 frag(v2f i) : SV_Target
            {
                // Get raw depth from [0,1];
                float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);

                // LinearEyeDepth is not accurate enough
                // Fixes issue of difference between Opengl and D3D11/12
                #if UNITY_REVERSED_Z
                    float clipZ = rawDepth;
                #else
                    float clipZ = rawDepth * 2.0 - 1.0;
                #endif
                float2 ndc = i.uv * 2.0 - 1.0;
                float4 clipPos = float4(ndc.x, ndc.y, clipZ, 1.0);

                float4 viewPosRaw = mul(_SensorClipToView, clipPos);
                float3 viewPos = viewPosRaw.xyz / viewPosRaw.w;

                // true depth compared to LinearEyeDepth
                float depthWorld = length(viewPos);
                float depthNorm = saturate(depthWorld / 4.0);

                // Use HSV since there color one value from 0 to 1
                // 0.66 is blue
                float hue = lerp(-0.02, 0.66, depthNorm);
                // Converting HSV back to RGB for frag
                float3 col = HSVtoRGB(float3(hue, 1, 1));

                return fixed4(col, 1);
            }

            ENDCG
        }
    }
}
