Shader "Ciconia Studio/Effects/Skin" {
    Properties {
        [Space(15)][Header(Main Properties)]
        [Space(15)]_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo", 2D) = "white" {}
        [Space(35)]_SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SpecGlossMap ("Specular map(Gloss A)", 2D) = "white" {}
        _SpecularIntensity ("Specular Intensity", Range(0, 8)) = 1
        _SpecularAmbient ("Specular Ambient", Range(0, 8)) = 0
        [Space(15)]_Glossiness ("Glossiness", Range(0, 2)) = 0.5
        _Ambientlight ("Ambient light", Range(0, 8)) = 0
        [Space(35)]_BumpMap ("Normal map", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0, 2)) = 1
        [Space(35)]_OcclusionMap ("Ambient Occlusion map", 2D) = "white" {}
        _AoIntensity ("Ao Intensity", Range(0, 2)) = 1
        [Space(35)]_EmissionColor ("Emission Color", Color) = (0,0,0,1)
        _EmissionMap ("Emission map", 2D) = "white" {}
        _EmissiveIntensity ("Emissive Intensity", Range(0, 2)) = 1

        [Space(15)][Header(Skin Properties)]
        [Space(15)][MaterialToggle] _Visualize ("Visualize", Float ) = 0
        [Space(15)]_TranslucencyColor ("Translucency Color", Color) = (0.9058824,0.4901961,0.4901961,1)
        [Space(15)][MaterialToggle] _Invertmap ("Invert map", Float ) = 1
        _Translucencymap ("Translucency map", 2D) = "white" {}
        _TranslucencyIntensity ("Translucency Intensity", Range(0, 1)) = 1
        [Space(15)]_TranslucencySpread ("Translucency Spread", Float ) = 0
        _Contrast ("Contrast", Float ) = 0
        [Space(35)]_SkinTone ("Skin Tone", Color) = (1,0.8581572,0.7921569,1)
        _SkinIntensity ("Skin Intensity", Range(0, 8)) = 1.3
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _SpecGlossMap; uniform float4 _SpecGlossMap_ST;
            uniform float4 _SpecularColor;
            uniform float _SpecularIntensity;
            uniform float _Glossiness;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _NormalIntensity;
            uniform float _Ambientlight;
            uniform sampler2D _OcclusionMap; uniform float4 _OcclusionMap_ST;
            uniform float _SpecularAmbient;
            uniform float _AoIntensity;
            uniform float _TranslucencyIntensity;
            uniform float4 _TranslucencyColor;
            uniform sampler2D _Translucencymap; uniform float4 _Translucencymap_ST;
            uniform float4 _SkinTone;
            uniform float _SkinIntensity;
            uniform float _Contrast;
            uniform fixed _Invertmap;
            uniform fixed _Visualize;
            uniform float _TranslucencySpread;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float4 _EmissionColor;
            uniform float _EmissiveIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 Normalmap = lerp(float3(0,0,1),_BumpMap_var.rgb,_NormalIntensity);
                float3 normalLocal = Normalmap;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float4 _SpecGlossMap_var = tex2D(_SpecGlossMap,TRANSFORM_TEX(i.uv0, _SpecGlossMap));
                float Glossiness = (_SpecGlossMap_var.a*_Glossiness);
                float gloss = Glossiness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float3 Specularmap = (((0.95*pow(1.0-max(0,dot(normalDirection, viewDirection)),1.0))+0.05)*(_SpecularColor.rgb*_SpecGlossMap_var.rgb*_SpecularIntensity));
                float3 specularColor = Specularmap;
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 indirectSpecular = (gi.indirect.specular + (_SpecularAmbient*UNITY_LIGHTMODEL_AMBIENT.rgb))*specularColor;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float3 Lightwrappingmap = (max(0,dot(i.normalDir,lightDirection))*attenuation*_SkinIntensity*_SkinTone.rgb);
                float3 w = Lightwrappingmap*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float4 _Translucencymap_var = tex2D(_Translucencymap,TRANSFORM_TEX(i.uv0, _Translucencymap));
                float node_6117 = 0.0;
                float node_6678 = (1.0+_Contrast);
                float node_7923 = (saturate((_TranslucencySpread+(node_6678 + ( (lerp( (1.0 - _Translucencymap_var.r), _Translucencymap_var.r, _Invertmap ) - node_6117) * ((-1*_Contrast) - node_6678) ) / (1.0 - node_6117))))*_TranslucencyIntensity);
                float3 Transmissionmap = saturate(((max(0,dot(i.normalDir,lightDirection))*attenuation)+(_TranslucencyColor.rgb*node_7923)));
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * Transmissionmap;
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = (forwardLight+backLight) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += (_Ambientlight*UNITY_LIGHTMODEL_AMBIENT.rgb); // Diffuse Ambient Light
                indirectDiffuse += gi.indirect.diffuse;
                float4 _OcclusionMap_var = tex2D(_OcclusionMap,TRANSFORM_TEX(i.uv0, _OcclusionMap));
                float Aomap = saturate((1.0-(1.0-_OcclusionMap_var.r)*(1.0-lerp(1,0,_AoIntensity))));
                indirectDiffuse *= Aomap; // Diffuse AO
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 Diffusemap = (_Color.rgb*_MainTex_var.rgb);
                float MaskViz = node_7923;
                float3 diffuseColor = lerp( Diffusemap, MaskViz, _Visualize );
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float4 _EmissionMap_var = tex2D(_EmissionMap,TRANSFORM_TEX(i.uv0, _EmissionMap));
                float3 Emissivemap = ((_EmissionColor.rgb*_EmissionMap_var.rgb)*_EmissiveIntensity);
                float3 emissive = Emissivemap;
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _SpecGlossMap; uniform float4 _SpecGlossMap_ST;
            uniform float4 _SpecularColor;
            uniform float _SpecularIntensity;
            uniform float _Glossiness;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _NormalIntensity;
            uniform float _TranslucencyIntensity;
            uniform float4 _TranslucencyColor;
            uniform sampler2D _Translucencymap; uniform float4 _Translucencymap_ST;
            uniform float4 _SkinTone;
            uniform float _SkinIntensity;
            uniform float _Contrast;
            uniform fixed _Invertmap;
            uniform fixed _Visualize;
            uniform float _TranslucencySpread;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float4 _EmissionColor;
            uniform float _EmissiveIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 Normalmap = lerp(float3(0,0,1),_BumpMap_var.rgb,_NormalIntensity);
                float3 normalLocal = Normalmap;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float4 _SpecGlossMap_var = tex2D(_SpecGlossMap,TRANSFORM_TEX(i.uv0, _SpecGlossMap));
                float Glossiness = (_SpecGlossMap_var.a*_Glossiness);
                float gloss = Glossiness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float3 Specularmap = (((0.95*pow(1.0-max(0,dot(normalDirection, viewDirection)),1.0))+0.05)*(_SpecularColor.rgb*_SpecGlossMap_var.rgb*_SpecularIntensity));
                float3 specularColor = Specularmap;
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float3 Lightwrappingmap = (max(0,dot(i.normalDir,lightDirection))*attenuation*_SkinIntensity*_SkinTone.rgb);
                float3 w = Lightwrappingmap*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float4 _Translucencymap_var = tex2D(_Translucencymap,TRANSFORM_TEX(i.uv0, _Translucencymap));
                float node_6117 = 0.0;
                float node_6678 = (1.0+_Contrast);
                float node_7923 = (saturate((_TranslucencySpread+(node_6678 + ( (lerp( (1.0 - _Translucencymap_var.r), _Translucencymap_var.r, _Invertmap ) - node_6117) * ((-1*_Contrast) - node_6678) ) / (1.0 - node_6117))))*_TranslucencyIntensity);
                float3 Transmissionmap = saturate(((max(0,dot(i.normalDir,lightDirection))*attenuation)+(_TranslucencyColor.rgb*node_7923)));
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * Transmissionmap;
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = (forwardLight+backLight) * attenColor;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 Diffusemap = (_Color.rgb*_MainTex_var.rgb);
                float MaskViz = node_7923;
                float3 diffuseColor = lerp( Diffusemap, MaskViz, _Visualize );
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _SpecGlossMap; uniform float4 _SpecGlossMap_ST;
            uniform float4 _SpecularColor;
            uniform float _SpecularIntensity;
            uniform float _Glossiness;
            uniform float _TranslucencyIntensity;
            uniform sampler2D _Translucencymap; uniform float4 _Translucencymap_ST;
            uniform float _Contrast;
            uniform fixed _Invertmap;
            uniform fixed _Visualize;
            uniform float _TranslucencySpread;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float4 _EmissionColor;
            uniform float _EmissiveIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float4 _EmissionMap_var = tex2D(_EmissionMap,TRANSFORM_TEX(i.uv0, _EmissionMap));
                float3 Emissivemap = ((_EmissionColor.rgb*_EmissionMap_var.rgb)*_EmissiveIntensity);
                o.Emission = Emissivemap;
                
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 Diffusemap = (_Color.rgb*_MainTex_var.rgb);
                float4 _Translucencymap_var = tex2D(_Translucencymap,TRANSFORM_TEX(i.uv0, _Translucencymap));
                float node_6117 = 0.0;
                float node_6678 = (1.0+_Contrast);
                float node_7923 = (saturate((_TranslucencySpread+(node_6678 + ( (lerp( (1.0 - _Translucencymap_var.r), _Translucencymap_var.r, _Invertmap ) - node_6117) * ((-1*_Contrast) - node_6678) ) / (1.0 - node_6117))))*_TranslucencyIntensity);
                float MaskViz = node_7923;
                float3 diffColor = lerp( Diffusemap, MaskViz, _Visualize );
                float4 _SpecGlossMap_var = tex2D(_SpecGlossMap,TRANSFORM_TEX(i.uv0, _SpecGlossMap));
                float3 Specularmap = (((0.95*pow(1.0-max(0,dot(normalDirection, viewDirection)),1.0))+0.05)*(_SpecularColor.rgb*_SpecGlossMap_var.rgb*_SpecularIntensity));
                float3 specColor = Specularmap;
                float Glossiness = (_SpecGlossMap_var.a*_Glossiness);
                float roughness = 1.0 - Glossiness;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
