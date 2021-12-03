Shader "Custom/OuterColorPickerShader"
{
    Properties
    {
        _AngleOffset("AngleOffset", int) = 0
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        int _AngleOffset;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        //0 ≤ h < 360, 0 ≤ s ≤ 1 and 0 ≤ v ≤ 1
        fixed3 hsv_to_rgb(float h, float s, float v)
        {
            float c = s * v;
            float x = c * (1 - abs(((h/60) % 2) - 1));
            float m = v - c;
            float3 rgb1;

            if(0 <= h && h < 60)
                rgb1 = float3(c, x, 0);
            else if(60 <= h && h < 120)
                rgb1 = float3(x, c, 0);
            else if(120 <= h && h < 180)
                rgb1 = float3(0, c, x);
            else if(180 <= h && h < 240)
                rgb1 = float3(0, x, c);
            else if(240 <= h && h < 300)
                rgb1 = float3(x, 0, c);
            else
                rgb1 = float3(c, 0, x);
            
            return fixed3(rgb1.x + m, rgb1.y + m, rgb1.z + m);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes the angle -> creating the color circle
            fixed PI = 3.14159265359;
            float angle = (atan2(0.5f - IN.uv_MainTex.y, 0.5f - IN.uv_MainTex.x) * 180 / PI) + _AngleOffset;
            angle = angle % 360;            
            
            o.Albedo = hsv_to_rgb(angle, 1.0, 1.0);
            // o.Albedo = fixed3(percentageAngle, percentageAngle, percentageAngle);
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
