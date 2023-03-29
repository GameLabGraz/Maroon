//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Used as fallback shader (for WebGL) for the teleport markers
//
//=============================================================================
// Note that this shader is a fallback shader for the Highlight shader by Valve Corporation, this shader has been written for Maroon by GameLabGraz.
// UNITY_SHADER_NO_UPGRADE
Shader "Vendor/Valve/VR/HighlightFallback"
{
	Properties
	{
		_TintColor( "Tint Color", Color ) = ( 1, 1, 1, 1 )
		_SeeThru( "SeeThru", Range( 0.0, 1.0 ) ) = 0.25
		_Darken( "Darken", Range( 0.0, 1.0 ) ) = 0.0
		_MainTex( "MainTex", 2D ) = "white" {}
	}

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	CGINCLUDE

		// Pragmas --------------------------------------------------------------------------------------------------------------------------------------------------
		#pragma target 3.0
		#pragma multi_compile_instancing

		// Includes -------------------------------------------------------------------------------------------------------------------------------------------------
		#include "UnityCG.cginc"

// Structs --------------------------------------------------------------------------------------------------------------------------------------------------
struct VertexInput
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		fixed4 color : COLOR;

		#if UNITY_VERSION >= 560
		UNITY_VERTEX_INPUT_INSTANCE_ID
		#endif
	};

	struct VertexOutput
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
		fixed4 color : COLOR;

		#if UNITY_VERSION >= 560
		UNITY_VERTEX_OUTPUT_STEREO
		#endif
	};


	// Globals --------------------------------------------------------------------------------------------------------------------------------------------------
	UNITY_INSTANCING_BUFFER_START( Props )
		UNITY_DEFINE_INSTANCED_PROP( float4, _TintColor )
		UNITY_DEFINE_INSTANCED_PROP( sampler2D, _MainTex )
		UNITY_DEFINE_INSTANCED_PROP( float4, _MainTex_ST )
		UNITY_DEFINE_INSTANCED_PROP( float, _SeeThru )
		UNITY_DEFINE_INSTANCED_PROP( float, _Darken )
	UNITY_INSTANCING_BUFFER_END( Props )

	// MainVs ---------------------------------------------------------------------------------------------------------------------------------------------------
	VertexOutput MainVS( VertexInput i )
	{
			VertexOutput o;
			UNITY_SETUP_INSTANCE_ID( i );
			UNITY_INITIALIZE_OUTPUT( VertexOutput, o );
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o ); 

			#if UNITY_VERSION >= 540
					o.vertex = UnityObjectToClipPos( i.vertex );
			#else
					o.vertex = mul( UNITY_MATRIX_MVP, i.vertex );
			#endif

			o.uv = i.uv;
			o.color = i.color;

			return o;
		}

	// MainPs ---------------------------------------------------------------------------------------------------------------------------------------------------
	float4 MainPS( VertexOutput i ) : SV_Target
	{
		float4 vTexel = tex2D(_MainTex, i.uv).rgba;
		float4 vColor = vTexel.rgba * _TintColor.rgba * i.color.rgba;
		vColor.rgba = saturate(2.0 * vColor.rgba);
		float flAlpha = vColor.a;

		vColor.rgb *= vColor.a;
		vColor.a = lerp(0.0, _Darken, flAlpha);

		return vColor.rgba;
	}

	// MainPs ---------------------------------------------------------------------------------------------------------------------------------------------------
	float4 SeeThruPS( VertexOutput i ) : SV_Target
	{
		float4 vTexel = tex2D(_MainTex, i.uv).rgba;
		float4 vColor = vTexel.rgba * _TintColor.rgba * i.color.rgba * _SeeThru;
		vColor.rgba = saturate(2.0 * vColor.rgba);
		float flAlpha = vColor.a;

		vColor.rgb *= vColor.a;
		vColor.a = lerp(0.0, _Darken, flAlpha * _SeeThru);

		return vColor.rgba;
	}

		ENDCG

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
			LOD 100

			// Behind Geometry ---------------------------------------------------------------------------------------------------------------------------------------------------
			Pass
		{
			// Render State ---------------------------------------------------------------------------------------------------------------------------------------------
			Blend One OneMinusSrcAlpha
			Cull Off
			ZWrite Off
			ZTest Greater

			CGPROGRAM
				#pragma vertex MainVS
				#pragma fragment SeeThruPS

			ENDCG
		}

			Pass
		{
			// Render State ---------------------------------------------------------------------------------------------------------------------------------------------
			Blend One OneMinusSrcAlpha
			Cull Off
			ZWrite Off
			ZTest LEqual

			CGPROGRAM
				#pragma vertex MainVS
				#pragma fragment MainPS
			ENDCG
		}
	}
}
