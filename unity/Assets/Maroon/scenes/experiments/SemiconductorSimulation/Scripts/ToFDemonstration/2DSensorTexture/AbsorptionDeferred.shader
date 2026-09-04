Shader "Custom/ToFAbsorptionDeferred"
{
    Properties
    {
        // Sensor calibration Values 
        _DiffuseWeight ("Diffuse Weight", Range(0,1)) = 0.5
        _SpecularWeight ("specular Weight", Range(0,1)) = 0.3
        _SmoothnessWeight ("Smoothness Weight", Range(0,1)) = 0.2
    }
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

            // Deferred Rendering Textures
            // https://docs.unity3d.com/Manual/RenderTech-DeferredShading.html
            sampler2D _CameraGBufferTexture0; // RT0, ARGB32 format: Diffuse color (RGB), occlusion (A).
            sampler2D _CameraGBufferTexture1; // RT1, ARGB32 format: Specular color (RGB), smoothness (A).
            sampler2D _CameraGBufferTexture2; // RT2, ARGB2101010 format: World space normal (RGB), unused (A).
            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
            
            float _DiffuseWeight;
            float _SpecularWeight;
            float _SmoothnessWeight;
            float _MaxDistance;
            
            float4x4 _SensorClipToView;
            float4x4 _SensorWorldToView;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 1. Sample the pure material properties from the G-Buffers
                float3 diffuseColor = tex2D(_CameraGBufferTexture0, uv).rgb;
                float4 specularColor = tex2D(_CameraGBufferTexture1, uv);

                // How much Infra red light is reflected based on the diffuse color of an Object
                // ITU-R Recommendation BT.601: standard that converts rgb to greyscale
                float diffuseReflectivity = dot(diffuseColor, float3(0.299, 0.587, 0.114));

                // Metallic doesn't exist in the pipeline. It is converted into specular and smoothness
                // so approximated by max color value
                float metallicApproximation = max(specularColor.r, max(specularColor.g, specularColor.b));
                
                // How much IR Light reflects based roughly on Material
                float materialReturn = (diffuseReflectivity * _DiffuseWeight) +
                                        (metallicApproximation * _SpecularWeight) + 
                                        (specularColor.a * _SmoothnessWeight);                
                
                // get surface normal of the object plane the pixel values stem from
                float3 worldNormal = normalize(tex2D(_CameraGBufferTexture2, uv).rgb * 2.0 - 1.0);
                float3 viewNormal = normalize(mul((float3x3)_SensorWorldToView, worldNormal));

                float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
                
                // Open GL vs D3D difference. One goes from 0 to 1 one the other way
                #if UNITY_REVERSED_Z
                    float clipZ = rawDepth;
                #else
                    float clipZ = rawDepth * 2.0 - 1.0;
                #endif
                float2 ndc = uv * 2.0 - 1.0;
                float4 clipPos = float4(ndc.x, ndc.y, clipZ, 1.0);

                float4 viewPosRaw = mul(_SensorClipToView, clipPos);
                float3 viewPos = viewPosRaw.xyz / viewPosRaw.w;
                float distanceFromSensor = length(viewPos);

                // normalize surface point to camera vector
                // -viewDir is roughly the IR emitter since sensor next to IR light source
                float3 viewDir = viewPos / max(distanceFromSensor, 1e-5);
                
                // angle from IR light hitting to surface normal of hit object
                // To factor in Lambertian Reflectance 
                float cosTheta = saturate(dot(viewNormal, -viewDir));
                
                // light energy is spread based on expanding sphere surface
                float distanceFalloff = saturate( (_MaxDistance * _MaxDistance) / 
                                        max(distanceFromSensor * distanceFromSensor, 0.0001));

                // to make back and forth 1/r^4 
                distanceFalloff = distanceFalloff * distanceFalloff;
                
                // how much light energy is conserved through distance and angle of reflector to camera
                float geometricReturn = cosTheta * distanceFalloff;

                float iRReturnIntensity = materialReturn * geometricReturn;

                return fixed4(iRReturnIntensity, iRReturnIntensity, iRReturnIntensity, 1.0);
            }
            ENDCG
        }
    }
}