Shader "Custom/SensorSurfaceShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Curvature ("Curvature Strength", Float) = 4.0
        _RainbowScale ("Rainbow Scale", Float) = 12.0
        _SinEffectStrenght ("SinCosCurvature disturbance EffectStrength", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        sampler2D _MainTex;
        float _Curvature;
        float _RainbowScale;
        float _SinEffectStrenght;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
            float3 viewDir;
        };
        
        // from 

        // HSV has hue, saturation, value
        // from https://www.laurivan.com/rgb-to-hsv-to-rgb-for-shaders/
        float3 HSVtoRGB(float3 hsv)
        {
            float4 K = float4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
            float3 p = abs(frac(hsv.xxx + K.xyz) * 6.0 - K.www);
            return hsv.z * lerp(K.xxx, saturate(p - K.xxx), hsv.y);
        }
        
        // View angle dependent Hue with variation based on sin and cos
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 baseCol = tex2D(_MainTex, IN.uv_MainTex);

            float3 worldVectorNormalized = normalize(IN.worldNormal);
            float3 up = abs(worldVectorNormalized.y) < 0.99 ? float3(0,1,0) : float3(1,0,0);
            float3 T = normalize(cross(worldVectorNormalized, up));
            float3 B = cross(worldVectorNormalized, T);

            float2 sinEffect = float2(
                sin(IN.uv_MainTex.x * 9.0) * 0.5 + sin(IN.uv_MainTex.y * 10.0) * 0.5,
                cos(IN.uv_MainTex.x * 4.0) * 0.5 + cos(IN.uv_MainTex.y * 9.0) * 0.5
            ) * _SinEffectStrenght;
            float2 offset = (IN.uv_MainTex - 0.5) * _Curvature * sinEffect;
            float3 pseudoNormal = normalize(worldVectorNormalized + T * offset.x + B * offset.y) ;

            float3 V = normalize(IN.viewDir);
            float d = 1.0 - dot(V, pseudoNormal);
            float rainbowCoord = frac(d * _RainbowScale);

            float3 rainbow = HSVtoRGB(float3(rainbowCoord, 1, 1));

            o.Albedo = baseCol.rgb / 30 + rainbow;
            o.Metallic = 0.2;
            o.Smoothness = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}