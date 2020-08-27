Shader "Glass Reflective" {
	Properties {
		_Color ("Main Color (RGB) Gloss (A)", Color) = (1,1,1,0)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_ReflectColor ("Reflection Color (RGB) Intensity (A)", Color) = (1,1,1,0.5)
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Cube ("Cubemap", Cube) = "" {}
	}
	SubShader {
		Tags {
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		LOD 400
		
		CGPROGRAM
			#pragma surface surf BlinnPhong decal:add nolightmap addshadow
			#pragma target 3.0
			//input limit (8) exceeded, shader uses 9
			
			struct Input {
				float2 uv_BumpMap;
				float3 worldRefl;
				INTERNAL_DATA
			};
			
			sampler2D _BumpMap;
			samplerCUBE _Cube;
			
			fixed4 _Color;
			fixed4 _ReflectColor;
			half _Shininess;
			
			void surf (Input IN, inout SurfaceOutput o) {
				o.Albedo = _Color.rgb;
				o.Gloss = _SpecColor.a;
				o.Specular = _Shininess;
				
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	
				float3 worldRefl = WorldReflectionVector (IN, o.Normal);
				fixed4 reflCol = texCUBE (_Cube, worldRefl);
	
				o.Emission = reflCol.rgb * _ReflectColor.rgb * _ReflectColor.a;
				o.Alpha = reflCol.a * _ReflectColor.a;
			}
		ENDCG
	}
	FallBack "Transparent/Specular"
}