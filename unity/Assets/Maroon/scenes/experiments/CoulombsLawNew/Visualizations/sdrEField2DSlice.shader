Shader "Custom/EField2DSlice" {
    Properties {
		// Note(MartinR): To prevent division by 0, the voltage calculation always clamps the distance to a minimum value
        _PointChargeMinDist ("Minimum Distance To Point Charges", Float) = 0.05
        _ChargedRodMinDist ("Minimum Distance To Charged Rods", Float) = 0.05
        _Transparency("Transparency", Range(0.0,1.0)) = 0.75
		// Note(MartinR): xyz is normal vector, w is the negative distance from origin to plane
        _PlaneEquation("PlaneEquation", Vector) = (1, 0, 0, 0)

		// Voltage-Heatmap parameters
		// --------------------------
		// Note(MartinR): 2 Charges with 0.1m distance at 10microCoulomb Charge-Difference results in 1 Mega Volt 
        _DrawHeatmap("DrawHeatmap", Integer) = 1
        _HeatmapMaxVoltage("Heatmap maximum Absolute Voltage", Float) = 300000
        _HeatmapFalloff ("Heatmap Falloff Exponent", Float) = 1.0

		// Equipotential-Line parameters
		// -----------------------------
        _DrawEquipotentialLines("DrawEquipotentialLines", Integer) = 1
        _LineColor("Line Color", Color) = (0.3, 0.3, 0.3, 1)
		_LineHalfWidth("LineHalfWidth", Float) = 0.005 // In Unity coordinates
		// Extra line thickness (percentual to normal line thickness), where line smoothly fades into background color
		_LineSmoothFalloff("LineSmoothFalloff", Range(0, 1)) = 1.0
		// At which voltage intervals the lines are drawn
        _LineSpacingVoltage ("Line Spacing Voltage", Float) = 30000
		// Voltage at which we stop drawing lines (As the lines would get too dense)
        _LineMaxVoltage ("Line Max Voltage", Float) = 500000
    }
    
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        
        ZWrite Off
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
			float _PointChargeMinDist;
			float _ChargedRodMinDist;
			float _Transparency;
			float4 _PlaneEquation;

			int _DrawHeatmap;
			float _HeatmapMaxVoltage;
			float _HeatmapFalloff;

			int _DrawEquipotentialLines;
			float4 _LineColor;
			float _LineHalfWidth;
			float _LineSmoothFalloff;
			float _LineSpacingVoltage;
			float _LineMaxVoltage;

			uniform int _PointChargeCount;
			uniform float4 _PointChargeData[100]; // Packed data, xyz is position, w is charge in Coulomb
			uniform int _ChargedRodCount;
			uniform float4 _ChargedRodPositions[30]; // Packed data, xyz is position, w is charge-density in Coulomb
			uniform float4 _ChargedRodDirections[30]; // w is currently unused
			uniform int _ChargedPlaneCount;
			uniform float4 _ChargedPlaneEquations[30]; // xyz is normalized normal, w is negative distance of plane to origin
			uniform float _ChargedPlaneChargeDensities[30];

			// --------------------------------------------------------------------------------------------------------------
			// Fragment shader

			// Unit is Newton meter^2 / Coulomb^2 [N m^2 / C^2] 
			#define COULOMBS_CONSTANT 9e9
			#define PI 3.14159265359

			// Returns Voltage and electric field value
			void evaluateField(float3 pos, out float voltage, out float3 fieldVector) 
			{
				voltage = 0.0; // In Volt
				fieldVector = float3(0, 0, 0); // In Newton / Coulomb [N/C]

				// Evaluate point charges
				int i = 0; // Note: If i is declared in the loop header, there are warnings in unity...
				for(i = 0; i < _PointChargeCount; i++)
				{
					float3 chargePos      = _PointChargeData[i].xyz;
					float  electricCharge = _PointChargeData[i].w;
                
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
					dist = max(dist, _PointChargeMinDist);

					// Sum up field values
					fieldVector += COULOMBS_CONSTANT * electricCharge * direction / (dist * dist);
					voltage     += COULOMBS_CONSTANT * electricCharge / dist;
				}

				// Evaluate charged rods
				for(i = 0; i < _ChargedRodCount; i++)
				{
					float3 rodPos = _ChargedRodPositions[i].xyz;
					float3 rodDir = _ChargedRodDirections[i].xyz;
					float  rodChargeDensity = _ChargedRodPositions[i].w;

					float3 posProjected = rodPos + rodDir * dot(pos - rodPos, rodDir);
					float3 rodToPos = pos - posProjected;
					float dist = length(rodToPos);
					if (dist < 0.0001) {
						rodToPos = float3(0, 1, 0);
					}
					else {
						rodToPos = rodToPos / dist;
					}

					// Clamp distance
					dist = max(dist, _ChargedRodMinDist);

					fieldVector +=   (2.0 * COULOMBS_CONSTANT) * rodChargeDensity * rodToPos / dist;
					voltage     += - (2.0 * COULOMBS_CONSTANT) * rodChargeDensity * log(dist);
				}

				// Add plane influences
				for (i = 0; i < _ChargedPlaneCount; i++) 
				{
					float4 planeEquation = _ChargedPlaneEquations[i];
					float planeChargeDensity = _ChargedPlaneChargeDensities[i];

					float signedDistance = dot(float4(pos, 1.0), planeEquation);
				    fieldVector +=   (2 * PI * COULOMBS_CONSTANT) * planeChargeDensity * sign(signedDistance) * planeEquation.xyz;
					voltage     += - (2 * PI * COULOMBS_CONSTANT) * planeChargeDensity * abs(signedDistance);
				}
			}

			float getEquipotentialLineAlpha(float3 initialPos)
			{
				float voltage;
				float3 _unused;
				evaluateField(initialPos, voltage, _unused);

				// Find voltage of closest field-line
				float targetVoltage = floor(voltage / _LineSpacingVoltage + 0.5) * _LineSpacingVoltage;
				if (abs(targetVoltage) > _LineMaxVoltage) return half4(0, 0, 0, 0);

				// Try to find a point near current position which is directly on the equipotential line.
				// We use this point to determine the distance of the current position to the equipotential line, and
				//		use the distance for coloring the pixel
				// Approximate search is done with Gradient-Descend (step-distance limited proportionally to line-width)
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

				float alpha = 1.0 - smoothstep(_LineHalfWidth, _LineHalfWidth + falloffDistance, distance(pos, initialPos));
				return alpha;
			}

			float3 GetHeatmapColor(float3 pos) 
			{
				float voltage;
				float3 _unused;
				evaluateField(pos, voltage, _unused);

				// Mix color based on voltage
				float alpha = min(1.0, abs(voltage / _HeatmapMaxVoltage));
				alpha = pow(alpha, 1.0 / _HeatmapFalloff);
				
				float3 primaryColor = float3(0, 0, 0);
				if (voltage >= 0) {
					primaryColor.x = 1.0;
				}
				else {
					primaryColor.z = 1.0;
				}

				return lerp(float3(1, 1, 1), primaryColor, alpha);
			}

			float4 frag(vert2frag input) : COLOR 
			{
				// Early exit if we have no charged objects
				if (_PointChargeCount == 0 && _ChargedRodCount == 0 && _ChargedPlaneCount == 0) {
					return float4(1, 1, 1, _Transparency);
				}

				// Project frag-position onto plane (Used in 2D mode of CoulombsLaw, maybe this should be changed at some point)
				float3 posOnPlane = input.pos_world_space.xyz;
				posOnPlane = posOnPlane - _PlaneEquation.xyz * (dot(posOnPlane, _PlaneEquation.xyz) + _PlaneEquation.w);

				// Compose heatmap, equipotential lines and transparency into final color
				float4 outputColor = float4(1, 1, 1, 1);
				if (_DrawHeatmap != 0) {
					outputColor.xyz = GetHeatmapColor(posOnPlane);
				}

				outputColor.w = _Transparency;
				if (_DrawEquipotentialLines != 0) {
					float lineAlpha = getEquipotentialLineAlpha(posOnPlane);
					outputColor = lerp(outputColor, _LineColor, lineAlpha);
				}

				// Gamma correct alpha
				outputColor.w = pow(outputColor.w, 1.0 / 2.2);

				return outputColor;
			}

			ENDCG
        }
    }
}