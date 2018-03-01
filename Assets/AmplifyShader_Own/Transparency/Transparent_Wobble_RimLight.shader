// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "JohannesKopf/Transparent"
{
	Properties
	{
		_RimPower("RimPower", Range( 0 , 10)) = 0
		_RimColor("RimColor", Color) = (0,0,0,0)
		_Refraction("Refraction", Range( 0 , 1)) = 0.08756032
		[Header(Refraction)]
		_IndexofRefraction("Index of Refraction", Range( -3 , 4)) = 1
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		_Opacity("Opacity", Range( 0 , 1)) = 0
		_Specular("Specular", Range( 0 , 1)) = 0
		_SoapAmount("Soap Amount", Range( 0 , 1)) = 0
		_DeformFrequency("Deform Frequency", Range( 0 , 10)) = 5
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.8
		_Occlusion("Occlusion", 2D) = "white" {}
		_Normals("Normals", 2D) = "bump" {}
		_Foam("Foam", 2D) = "white" {}
		_TextureSample0("Texture Sample 0", 2D) = "bump" {}
		_TextureSample2("Texture Sample 2", 2D) = "white" {}
		_WobbleStrength("WobbleStrength", Range( 0 , 10)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		#pragma surface surf StandardSpecular keepalpha finalcolor:RefractionF exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
			float3 viewDir;
			INTERNAL_DATA
			float3 worldNormal;
			float3 worldPos;
		};

		uniform sampler2D _Normals;
		uniform float4 _Normals_ST;
		uniform sampler2D _GrabTexture;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float _Refraction;
		uniform float _RimPower;
		uniform float4 _RimColor;
		uniform float _Specular;
		uniform sampler2D _TextureSample2;
		uniform sampler2D _Foam;
		uniform float _SoapAmount;
		uniform float _Smoothness;
		uniform sampler2D _Occlusion;
		uniform float4 _Occlusion_ST;
		uniform float _Opacity;
		uniform float _ChromaticAberration;
		uniform float _IndexofRefraction;
		uniform float _DeformFrequency;
		uniform float _WobbleStrength;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float temp_output_28_0 = ( ( _DeformFrequency * ase_vertex3Pos.y ) + _Time.y );
			float3 temp_cast_0 = (( ( ( cos( temp_output_28_0 ) * 0.015 ) + ( sin( temp_output_28_0 ) * 0.005 ) ) * _WobbleStrength )).xxx;
			v.vertex.xyz += temp_cast_0;
		}

		inline float4 Refraction( Input i, SurfaceOutputStandardSpecular o, float indexOfRefraction, float chomaticAberration ) {
			float3 worldNormal = o.Normal;
			float4 screenPos = i.screenPos;
			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0;
			#endif
			float halfPosW = screenPos.w * 0.5;
			screenPos.y = ( screenPos.y - halfPosW ) * _ProjectionParams.x * scale + halfPosW;
			#if SHADER_API_D3D9 || SHADER_API_D3D11
				screenPos.w += 0.00000000001;
			#endif
			float2 projScreenPos = ( screenPos / screenPos.w ).xy;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 refractionOffset = ( ( ( ( indexOfRefraction - 1.0 ) * mul( UNITY_MATRIX_V, float4( worldNormal, 0.0 ) ) ) * ( 1.0 / ( screenPos.z + 1.0 ) ) ) * ( 1.0 - dot( worldNormal, worldViewDir ) ) );
			float2 cameraRefraction = float2( refractionOffset.x, -( refractionOffset.y * _ProjectionParams.x ) );
			float4 redAlpha = tex2D( _GrabTexture, ( projScreenPos + cameraRefraction ) );
			float green = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;
			float blue = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;
			return float4( redAlpha.r, green, blue, redAlpha.a );
		}

		void RefractionF( Input i, SurfaceOutputStandardSpecular o, inout fixed4 color )
		{
			#ifdef UNITY_PASS_FORWARDBASE
			color.rgb = color.rgb + Refraction( i, o, _IndexofRefraction, _ChromaticAberration ) * ( 1 - color.a );
			color.a = 1;
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			o.Normal = float3(0,0,1);
			float2 uv_Normals = i.uv_texcoord * _Normals_ST.xy + _Normals_ST.zw;
			float3 tex2DNode10 = UnpackNormal( tex2D( _Normals, uv_Normals ) );
			o.Normal = tex2DNode10;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 screenColor1 = tex2D( _GrabTexture, ( ase_screenPosNorm + float4( ( UnpackNormal( tex2D( _TextureSample0, uv_TextureSample0 ) ) * _Refraction ) , 0.0 ) ).xy );
			o.Albedo = screenColor1.rgb;
			float3 normalizeResult9 = normalize( i.viewDir );
			float dotResult11 = dot( tex2DNode10 , normalizeResult9 );
			o.Emission = ( pow( ( 1.0 - saturate( dotResult11 ) ) , _RimPower ) * _RimColor ).rgb;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult46 = dot( ase_worldNormal , ase_worldViewDir );
			float4 temp_cast_4 = (_Specular).xxxx;
			float2 uv_TexCoord36 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 panner41 = ( uv_TexCoord36 + (_SinTime.x*0.5 + 0.5) * float2( 1,1 ));
			float2 temp_cast_5 = (( ( tex2D( _Foam, panner41 ).r + ( abs( (uv_TexCoord36.x*2 + -1) ) * 0.5 ) ) + _Time.x )).xx;
			float4 lerpResult58 = lerp( temp_cast_4 , saturate( tex2D( _TextureSample2, temp_cast_5 ) ) , _SoapAmount);
			o.Specular = ( ( 1.0 - saturate( ( pow( dotResult46 , 2 ) - 0.1 ) ) ) * lerpResult58 ).rgb;
			o.Smoothness = _Smoothness;
			float2 uv_Occlusion = i.uv_texcoord * _Occlusion_ST.xy + _Occlusion_ST.zw;
			o.Occlusion = tex2D( _Occlusion, uv_Occlusion ).r;
			o.Alpha = _Opacity;
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14501
204;92;1258;650;5111.249;730.5409;5.601913;True;False
Node;AmplifyShaderEditor.CommentaryNode;35;-3585.709,1323.107;Float;False;2577.155;665.7997;;24;59;58;57;56;55;54;53;52;51;50;49;48;47;46;45;44;43;42;41;40;39;38;37;36;Chromatic Specular Reflection;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-3537.709,1387.107;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinTimeNode;37;-3281.709,1515.106;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;38;-3121.709,1515.106;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;39;-3297.709,1723.106;Float;True;3;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;41;-2897.709,1387.107;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;24;-943.5186,2336.089;Float;False;1036;492;;10;34;33;32;31;30;29;28;27;26;25;Wobble;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;23;-2797.841,232.1004;Float;False;1714.099;809.0021;;12;8;9;10;11;12;14;13;16;15;19;20;17;Rim Light;1,1,1,1;0;0
Node;AmplifyShaderEditor.AbsOpNode;40;-3009.709,1723.106;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;8;-2747.841,554.1006;Float;False;Tangent;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;25;-877.5183,2482.09;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;34;-893.5184,2386.09;Float;False;Property;_DeformFrequency;Deform Frequency;8;0;Create;True;0;5;5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-2785.709,1723.106;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;43;-2193.709,1531.106;Float;False;World;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;44;-2705.709,1371.107;Float;True;Property;_Foam;Foam;13;0;Create;True;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;42;-2241.709,1387.107;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TimeNode;48;-2433.709,1755.106;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TimeNode;26;-893.5184,2626.09;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-669.5172,2450.09;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;47;-2385.709,1499.107;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1.32;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;46;-2033.708,1451.107;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;9;-2525.144,551.0009;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;10;-2747.841,282.1005;Float;True;Property;_Normals;Normals;12;0;Create;True;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;49;-1905.708,1451.107;Float;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-541.5192,2578.09;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;22;-1761.665,-659.3373;Float;False;733.6611;707.7414;;6;7;2;3;5;4;1;Transparency + Refraction;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;50;-2161.708,1739.106;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;11;-2333.444,472.6003;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;12;-2158.245,448.1003;Float;False;1;0;FLOAT;1.23;False;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;29;-381.5193,2482.09;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;30;-381.5193,2578.09;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1705.606,-66.59492;Float;False;Property;_Refraction;Refraction;2;0;Create;True;0;0.08756032;0.08756032;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;52;-1745.709,1451.107;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;51;-1953.708,1579.107;Float;True;Property;_TextureSample2;Texture Sample 2;15;0;Create;True;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-1711.664,-341.4596;Float;True;Property;_TextureSample0;Texture Sample 0;14;0;Create;True;0;None;bd734c29baceb63499732f24fbaea45f;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;55;-1617.709,1595.107;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-1953.708,1803.106;Float;False;Property;_SoapAmount;Soap Amount;7;0;Create;True;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-237.5197,2482.09;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.015;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-237.5197,2578.09;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.005;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;3;-1481.988,-609.3372;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;53;-1601.709,1451.107;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-2085.844,605.8003;Float;False;Property;_RimPower;RimPower;0;0;Create;True;0;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;13;-1989.242,490.3995;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-1398.131,-146.958;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-1953.708,1899.106;Float;False;Property;_Specular;Specular;6;0;Create;True;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;15;-1796.444,513.8;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;33;-61.51842,2530.09;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;201.1863,2549.569;Float;False;Property;_WobbleStrength;WobbleStrength;16;0;Create;True;0;1;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;58;-1425.709,1595.107;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;16;-1802.643,693.6002;Float;False;Property;_RimColor;RimColor;1;0;Create;True;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;57;-1457.709,1451.107;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;4;-1372.508,-364.7534;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenColorNode;1;-1230.003,-426.4224;Float;False;Global;_GrabScreen0;Grab Screen 0;0;0;Create;True;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;20;-1404.741,811.1031;Float;True;Property;_Occlusion;Occlusion;11;0;Create;True;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;62;-148.8545,1675.111;Float;False;Property;_Opacity;Opacity;5;0;Create;True;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;19;-1408.34,619.5012;Float;True;Property;_Metallic;Metallic;10;0;Create;True;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;61;-148.8545,1483.111;Float;False;Property;_Smoothness;Smoothness;9;0;Create;True;0;0.8;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-1233.709,1515.106;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;273.8594,2364.699;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-1554.043,528.9999;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-148.8545,1579.111;Float;False;Property;_IndexofRefraction;Index of Refraction;4;0;Create;True;0;1;0;-3;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;546.9178,644.0809;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;JohannesKopf/Transparent;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;False;0;Translucent;0.5;True;False;0;False;Opaque;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;3;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;38;0;37;1
WireConnection;39;0;36;1
WireConnection;41;0;36;0
WireConnection;41;1;38;0
WireConnection;40;0;39;0
WireConnection;45;0;40;0
WireConnection;44;1;41;0
WireConnection;27;0;34;0
WireConnection;27;1;25;2
WireConnection;47;0;44;1
WireConnection;47;1;45;0
WireConnection;46;0;42;0
WireConnection;46;1;43;0
WireConnection;9;0;8;0
WireConnection;49;0;46;0
WireConnection;28;0;27;0
WireConnection;28;1;26;2
WireConnection;50;0;47;0
WireConnection;50;1;48;1
WireConnection;11;0;10;0
WireConnection;11;1;9;0
WireConnection;12;0;11;0
WireConnection;29;0;28;0
WireConnection;30;0;28;0
WireConnection;52;0;49;0
WireConnection;51;1;50;0
WireConnection;55;0;51;0
WireConnection;31;0;29;0
WireConnection;32;0;30;0
WireConnection;53;0;52;0
WireConnection;13;0;12;0
WireConnection;5;0;2;0
WireConnection;5;1;7;0
WireConnection;15;0;13;0
WireConnection;15;1;14;0
WireConnection;33;0;31;0
WireConnection;33;1;32;0
WireConnection;58;0;56;0
WireConnection;58;1;55;0
WireConnection;58;2;54;0
WireConnection;57;0;53;0
WireConnection;4;0;3;0
WireConnection;4;1;5;0
WireConnection;1;0;4;0
WireConnection;59;0;57;0
WireConnection;59;1;58;0
WireConnection;63;0;33;0
WireConnection;63;1;64;0
WireConnection;17;0;15;0
WireConnection;17;1;16;0
WireConnection;0;0;1;0
WireConnection;0;1;10;0
WireConnection;0;2;17;0
WireConnection;0;3;59;0
WireConnection;0;4;61;0
WireConnection;0;5;20;0
WireConnection;0;8;60;0
WireConnection;0;9;62;0
WireConnection;0;11;63;0
ASEEND*/
//CHKSM=0812EDA1DC0D99CA9B0AE246884D0934FB4C6495