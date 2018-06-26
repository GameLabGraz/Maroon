Shader "Custom/Eyes"
{
	// Global explaination of the shader properties here : http://ellenack.com/index.php/2016/10/22/easy-eyes-out-soon/

	Properties
	{
		[Header(Eye Texture)]
		[Space(10)]

		[NoScaleOffset] _MainTex("Eye Masks", 2D) = "white" {} // Iris (R) Color(G) Height(B) Veins(A). More informations here http://ellenack.com/index.php/2016/10/23/easy-eyes-the-mask-texture/

		[Header(Colors)]
		[Space(10)]

		_Color1("First Color", Color) = (1.0,0.75,0.33,1) // Color of the white part of the mask.
		_Color2("Second Color", Color) = (0.33,1.0,1.0,1) // Color of the black part of the mask.
		_IrisRimColor("Iris Rim Color", Color) = (1,1,1,1) // Color at the rim of the iris.
		_ScleraColor("Sclera Color", Color) = (1,1,1,1) // Color of the usually white part of the eye.
		_ScleraVeinsColor("Sclera Veins Color", Color) = (1.0,0.3,0.3,1) // Color of the veins.

		[Header(Material Information)]
		[Space(10)]

		_Smoothness("Smoothness", Range(0.0,1.0)) = .9
		_Metalness("Metalness", Range(0.0,1.0)) = 0
		_Emission("Emission", Range(0.0,10.0)) = 0

		[Header(Parallax)]
		[Space(10)]

		_PupilHeightStrenght("Parallax Strenght", Float) = -.2 // Only applied to the iris, to simulate the presence of the cornea
		_PupilHeightBias("Parallax Bias", Float) = -0.07 // Applied globally to the eye, to give the impression the eye is behond a tin transparent layer. Low values works better.
		_PupilSize("Pupil Size", Range(0.5,10.0)) = 1 // Pupil expension. Non-linear.

		[Header(Iris)]
		[Space(10)]

		_IrisSize("Iris Size", Range(0.0,1.0)) = .1
		_IrisRim("Iris Rim Distance", Range(0.0,1.0)) = .8 // As a percentage of the iris size.
		_IrisRimInFade("Iris Rim In Fade", Range(1.0,2.0)) = 1.7
		_IrisRimOutFade("Iris Rim Out Fade", Range(1.0,2.0)) = 1.2
		_PupilNormalScale("Normal Scale", Float) = 2.5
		_PupilNormalStrenght("Strenght", Range(3.0,0.0)) = 0.2
		[NoScaleOffset] _PupilNormal("Pupil Normal", 2D) = "bump" {}

		[Header(Sclera)]
		[Space(10)]

		_ScleraSize("Sclera Size", Range(0.0,1.0)) = .15
		_ScleraFade("Sclera Fade", Range(1.0,2.0)) = 2
		_VeinsFade("Veins Fade", Range(0,1)) = .2
		_VeinsScale("Veins Scale", Float) = 3
		_VeinsNormalStrenght("Strenght", Range(3.0,0.0)) = .3
		[NoScaleOffset] _VeinsNormal("Veins Normal", 2D) = "bump" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		float _Smoothness;
		float _Metalness;
		float _Emission;

		float4 _Color1;
		float4 _Color2;
		float4 _IrisRimColor;
		float4 _ScleraColor;
		float4 _ScleraVeinsColor;

		float _PupilHeightStrenght;
		float _PupilHeightBias;
		float _PupilSize;

		float _IrisSize;
		float _IrisRim;
		float _IrisRimInFade;
		float _IrisRimOutFade;
		float _PupilNormalScale;
		float _PupilNormalStrenght;
		sampler2D _PupilNormal;

		float _ScleraSize;
		float _ScleraFade;
		float _VeinsDistance;
		float _VeinsFade;
		float _VeinsScale;
		float _VeinsNormalStrenght;
		sampler2D _VeinsNormal;

		// Basically the Unity unpack function, with a control over the strenght of the normal added
		fixed3 Unpack(float4 packednormal, float strenght)
		{
			#if defined(SHADER_API_GLES) && defined(SHADER_API_MOBILE)
				return packednormal.xyz * 2 - 1;
			#else
				fixed3 normal;
				normal.xy = packednormal.wy * 2 - 1;
				normal.z = sqrt(1 - normal.x*normal.x - normal.y * normal.y) * strenght;
				return normalize(normal);
			#endif
		}

		// Convert cartesian coordinates to radial coordinates
		float2 ToRadial(float2 uv)
		{
			return float2(atan2(uv.y, uv.x), length(uv));
		}

		float2 Parallax(float2 uv, float2 viewDir, float scale, float bias)
		{
			float h = tex2D(_MainTex, uv).b;
			float hsb = h * scale + bias;
			
			return uv + viewDir * hsb;
		}

		struct Input
		{
			float2 uv_MainTex;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float2 uvBase = Parallax(IN.uv_MainTex - .5, IN.viewDir.xy, .1, _PupilHeightBias);
			float2 radial = ToRadial(uvBase);

				// Sampling

			float2 uv = (normalize(uvBase) * saturate(pow(length(uvBase) / _IrisSize, _PupilSize)) * .5) + .5;
			uv = Parallax(uv, IN.viewDir.xy, _PupilHeightStrenght, 0);
			float2 eye = tex2D(_MainTex, uv).rg;

			float2 uvNorm = uvBase * _PupilNormalScale + .5;
			float3 eyeNorm = Unpack(tex2D(_PupilNormal, uvNorm), _PupilNormalStrenght);

			float2 uvVeins = uvBase *  _VeinsScale + .5;
			float veins = tex2D(_MainTex, uvVeins).a;
			float3 veinsNorm = Unpack(tex2D(_VeinsNormal, uvVeins), _VeinsNormalStrenght);

				// Rim

			float rimInLerp = smoothstep(_IrisSize * _IrisRim, _IrisSize * _IrisRim * _IrisRimInFade, radial.y);
			float rimOutLerp = smoothstep(_IrisSize, _IrisSize * _IrisRimOutFade, radial.y);

				// Veins

			float veinsLerp = smoothstep(_ScleraSize*2, _ScleraSize * 2 * _VeinsFade, radial.y);

				// Eye Red

			float scleraLerp = smoothstep(_ScleraSize, _ScleraSize * _ScleraFade, radial.y);

				// Construction

					// Color
			float3 pupilColor = lerp(_Color2.xyz, _Color1.xyz, eye.g);
			float3 veinsColor = lerp(_ScleraColor.xyz, _ScleraVeinsColor.xyz, veins);
			float3 scleraColor = lerp(veinsColor, _ScleraColor.xyz, veinsLerp);

			float3 c = lerp(pupilColor * eye.r, _IrisRimColor.xyz, rimInLerp);
			c = lerp(c, scleraColor, rimOutLerp);
			c = lerp(c, _ScleraVeinsColor.xyz, scleraLerp);

					// Emission
			float3 e = lerp(pupilColor * eye.r, 0, rimInLerp);
			e = lerp(e, 0, rimOutLerp);

					// Normal
			float3 scleraNorm = lerp(veinsNorm, float3(0,0,1), veinsLerp);

			float3 n = lerp(float3(0, 0, 1), scleraNorm, rimInLerp);
			n = lerp(n, float3(0, 0, 1), scleraLerp);
			n += eyeNorm;

					// Fill SurfaceOutputStandard
			o.Albedo = c;
			o.Smoothness = _Smoothness;
			o.Metallic = _Metalness;
			o.Normal = normalize(n);
			o.Emission = e * _Emission;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
