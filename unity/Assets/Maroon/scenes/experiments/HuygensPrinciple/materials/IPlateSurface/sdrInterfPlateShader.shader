Shader "Custom/sdrInterferencePlate"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ColorMin("Tint Color At Min", Color) = (0,0,0,0)
		_ColorMax("Tint Color At Max", Color) = (1,1,1,1)
		_SlitWidth("Slit width", Float) = 0.0
		_DistanceBetweenSlits("Distance between slits", Float) = 0.0
		_DistanceBetweenPlates("Distance between plates", Float) = 0.0
		_WaveLength("Wave Length", Float) = 0.0
		_NumberOfSlits("Number of Slits", Range(1,5)) = 1
	}
		SubShader
		{
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				#define PI 3.1415926535897932384626433832795

				struct vertexInput {
					float4 pos : POSITION;

					UNITY_VERTEX_INPUT_INSTANCE_ID //Insert to render on both eyes VR
				};

				struct v2f {
					float4 sv_position   : SV_POSITION;
					float4 object_space : TEXCOORD0;
					fixed3 color : COLOR0;

					UNITY_VERTEX_OUTPUT_STEREO //Insert to render on both eyes VR
				};

				fixed4 _ColorMin;
				fixed4 _ColorMax;
				int _NumberOfSlits;
				float _SlitWidth;
				float _WaveLength;
				float _DistanceBetweenSlits;
				float _DistanceBetweenPlates;
				float _MaxIntensity = 0;

				v2f vert(vertexInput v)
				{
					v2f output;

					UNITY_SETUP_INSTANCE_ID(v); //Insert to render on both eyes VR
					UNITY_INITIALIZE_OUTPUT(v2f, output); //Insert to render on both eyes VR
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output); //Insert to render on both eyes VR
					
					output.sv_position = UnityObjectToClipPos(v.pos);
					output.object_space = v.pos;
					output.color = fixed3(0, 0, 0);
					return output;
				}

				float calculateIntensityWithWidth(float alpha)
				{
					float Intensity = 0;
					float gutterFunction = 1;
					float gutterDivident = sin((_NumberOfSlits * PI * _DistanceBetweenSlits * sin(alpha)) / _WaveLength);
					float gutterDivisor = sin((PI * _DistanceBetweenSlits * sin(alpha)) / _WaveLength);

					if (_NumberOfSlits > 1)
						gutterFunction = pow((gutterDivident / gutterDivisor), 2);

					float singleSlitDivident = sin((PI * _SlitWidth * sin(alpha)) / _WaveLength);
					float singleSlitDivisor = (PI * _SlitWidth * sin(alpha)) / _WaveLength;
					float singleSlitFunction = pow((singleSlitDivident / singleSlitDivisor), 2);

					Intensity = gutterFunction * singleSlitFunction;
					return Intensity;
				}

				// Formular: 
				// http://lampx.tugraz.at/~hadley/physikm/apps/single_slit.de.php
				// http://lampx.tugraz.at/~hadley/physikm/apps/2single_slit.de.php
				// https://www.leifiphysik.de/optik/beugung-und-interferenz/grundwissen/vielfachspalt-und-gitter
				fixed4 frag(v2f i) : SV_Target
				{
					float alpha = atan(i.object_space.x / _DistanceBetweenPlates);
					float intensity = calculateIntensityWithWidth(alpha) / pow(_NumberOfSlits, 2);

					i.color = lerp(_ColorMin, _ColorMax, intensity);
					if ((i.object_space.y + 0.5 <= intensity + 0.05) && (i.object_space.y + 0.5 >= intensity - 0.05))
					{
						i.color = fixed3(1, 0.0, 0.5);
					}

					return fixed4(i.color, 1);
				}
				ENDCG
			}
		}
}
