Shader "Custom/CoulombEquipotentialLineShader" {
    Properties {
		// Note(MartinR): To prevent division by 0, the voltage calculation always clamps the distance to a minimum value
        _MinDistance ("Minimum Distance To Charges", Float) = 0.05
        _LineColor("Line Color", Color) = (0.3, 0.3, 0.3, 1)
        _BgColor("Background Color", Color) = (1,1,1,1)

		// At which voltage intervals the lines are drawn
        _LineSpacingVoltage ("LineSpacingVoltage", Float) = 30000
		// Voltage at which we stop drawing lines (As the lines would get too dense)
        _MaxAbsLineVoltage ("MaxAbsLineVoltage", Float) = 500000

		_LineHalfWidth("LineHalfWidth", Float) = 0.005 // In Unity coordinates
		// Extra line thickness (percentual to normal line thickness), where line smoothly fades into background color
		_LineSmoothFalloff("LineSmoothFalloff", Range(0, 1)) = 1.0
    }
    
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass {
			CGPROGRAM

			// --------------------------------------------------------------------------------------------------------------
			// Shader setup
			#pragma vertex vert  
			#pragma fragment frag
			#include "UnityCG.cginc"
        
			struct vertInput {
				float4 pos : POSITION;
			};  
        
			struct vert2frag {
				float4 pos_world_space  : TEXCOORD1;
				float4 pos_clip_space   : SV_POSITION;
			};

			// --------------------------------------------------------------------------------------------------------------
			// Vertex shader
        
			vert2frag vert(vertInput input) {
				vert2frag output;
				output.pos_world_space = mul(unity_ObjectToWorld, input.pos);
				output.pos_clip_space = UnityObjectToClipPos(input.pos);
				return output;
			}

			// --------------------------------------------------------------------------------------------------------------
			// Data structures
			float _MinDistance;
			float4 _LineColor;
			float4 _BgColor;

			float _LineSpacingVoltage;
			float _MaxAbsLineVoltage;
			float _LineHalfWidth;
			float _LineSmoothFalloff;

			uniform int _EntryCnt;
			uniform float4 _Entries[100];

			// --------------------------------------------------------------------------------------------------------------
			// Fragment shader

			// Unit is Newton meter^2 / Coulomb^2 [N m^2 / C^2] 
			#define COULOMBS_CONSTANT 9e9

			// Returns Voltage and electric field value
			void evaluateField(float3 pos, out float voltage, out float3 fieldVector) 
			{
				voltage = 0.0; // In Volt
				fieldVector = float3(0, 0, 0); // In Newton / Coulomb [N/C]
				for(int i = 0; i < _EntryCnt; ++i)
				{
					float3 chargePos    = _Entries[i].xyz;
					float  electricCharge = _Entries[i].w;

					// 2D mode
					chargePos.z = pos.z;
                
					// Safe normalization (Check if vector is 0)
					float3 direction = pos - chargePos;
					float dist = length(direction);
					if (dist < 0.0001) {
						direction = float3(0, 1, 0);
					}
					else {
						direction = direction / dist;
					}

					// Clamp distance to avoid division by 0
					dist = max(dist, _MinDistance);

					// Sum up field values
					voltage += COULOMBS_CONSTANT * electricCharge / dist;
					fieldVector += direction * COULOMBS_CONSTANT * electricCharge / (dist * dist);
				}
			}

			half4 frag(vert2frag input) : COLOR 
			{
				float3 initialPos = input.pos_world_space;
				if (_EntryCnt > 0) { // 2D mode
					initialPos.z = _Entries[0].z;
				}

				// Calculate voltage at current position
				float voltage;
				float3 _unused;
				evaluateField(initialPos, voltage, _unused);

				// Find voltage of closest field-line
				float targetVoltage = floor(voltage / _LineSpacingVoltage + 0.5) * _LineSpacingVoltage;
				if (abs(targetVoltage) > _MaxAbsLineVoltage) return _BgColor;

				// Try to find a point near current position which is directly on the equipotential line.
				// We use this point to determine the distance of the current position to the equipotential line, and
				//		use the distance for coloring the pixel
				// Search is done (Approximate search with range-limited Gradient-Descend)
				float falloffDistance = _LineHalfWidth * _LineSmoothFalloff;
				float maxSearchRadius = (_LineHalfWidth + falloffDistance) * 1.3;

				float3 pos = initialPos;
				for (int i = 0; i < 8; i++) 
				{
					// Evaluate Field at current iteration position
					float voltage;
					float3 fieldVector;
					evaluateField(pos, voltage, fieldVector);

					// Calculate Step
					// Note(MartinR): EField is the negative gradient of the Electric Potential, so we can use it for gradient descend
					float mag = length(fieldVector);
					float3 stepDir = -normalize(fieldVector);
					float stepSize = (targetVoltage - voltage) / mag;
					if (stepSize < 0) {
						stepSize = -stepSize;
						stepDir = -stepDir;
					}
					stepSize = min(stepSize, 1);
					pos = pos + stepSize * stepDir;

					// Limit search to a radius around initialPos
					float3 toPos = pos - initialPos;
					float distanceFromInitial = length(toPos);
					if (distanceFromInitial > maxSearchRadius) {
						pos = initialPos + toPos / distanceFromInitial * maxSearchRadius;
					}
				}

				// Calculate color based on distance to closest point on equipotential line
				float alpha = smoothstep(_LineHalfWidth, _LineHalfWidth + falloffDistance, distance(pos, initialPos));
				float4 finalColor = lerp(_LineColor, _BgColor, alpha);



				// TODO(MartinR): Remove debugging code when everything works
				// if (false)
				// {
				// 	float3 posA = float3(1, 1.8, input.pos_world_space.z);
				// 	float3 posB = posA;
				// 	for (int i = 0; i < 8; i++)
				// 	{
				// 		float voltage; 
				// 		float3 fieldVector;
				// 		evaluateField(posB, voltage, fieldVector);

				// 		float stepSize = (targetVoltage - voltage) / length(fieldVector);
				// 		float3 stepDir = -normalize(fieldVector);

				// 		float maxStep = 0.5;
				// 		stepSize = clamp(stepSize, -maxStep, maxStep);
				// 		posB = posB + stepSize * stepDir;

				// 		// Intermediate step circles
				// 		float a = i / 10.0;
				// 		float r = 0.1 * (0.5 + a / 2);
				// 		if (distance(input.pos_world_space, posB) < r) {
				// 			finalColor = float4(a, 0, a, 1);
				// 		}
				// 	}
				// 	posB.z = input.pos_world_space.z;

				// 	// Start circle 
				// 	if (distance(posA, input.pos_world_space) < 0.1) {
				// 		finalColor = float4(0, 1, 0, 1);
				// 	}
				// 	// End circle
				// 	if (distance(posB, input.pos_world_space) < 0.07) {
				// 		finalColor = float4(1, 0, 0, 1);
				// 	}
				// }

				return finalColor;
			}

			ENDCG
        }
    }
}