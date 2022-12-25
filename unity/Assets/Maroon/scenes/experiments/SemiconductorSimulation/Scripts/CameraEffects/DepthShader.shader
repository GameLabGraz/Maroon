Shader "Hidden/DepthShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 position : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.position = v.vertex;
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _CameraDepthNormalsTexture;
            
            

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float4 NormalDepth;

                float3 _ColorRed = float3(255.0,0.0,0.0);
                float3 _ColorBlue = float3(0.0,0.0,255);

                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), NormalDepth.w, NormalDepth.xyz);
                //float dx = i.position.x - 0.5;
                //float dy = i.position.y - 0.5;

                //float inCircle = 0.0;

                //if(dx * dx + dy * dy <= 0.005)
                //    inCircle = 1.0;

                //if(inCircle == 1.0) 
                //{
                //    col.r = 50 * NormalDepth.w;
                //    col.g = NormalDepth.w;
                //    col.b = NormalDepth.w;
                //}
                
                //col.r = 50 * NormalDepth.w;
                //col.g = NormalDepth.w;
                //col.b = NormalDepth.w;

                col.rgb = _ColorRed * NormalDepth.w + _ColorBlue * (1 -  NormalDepth.w);
                
                return col;
            }


            ENDCG
        }
    }
}
