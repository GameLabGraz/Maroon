Shader "Custom/EField2DSlice" 
{
    Properties 
	{
		_LineHalfWidth("LineHalfWidth", Float) = 0.005 // In Unity coordinates
		// Extra line thickness, where line smoothly fades into background color
		_LineSmoothFalloff("LineSmoothFalloff", Float) = 0.005
		_LineColor("LineColor", Vector) = (0.0, 0.0, 0.0, 1.0)
    }
    
    SubShader 
	{
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass 
		{
			CGPROGRAM

			// --------------------------------------------------------------------------------------------------------------
			// Shader setup
			#pragma vertex vert  
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "ElectricFieldShaderUtils.cginc"
			#include "ShaderUtils.cginc"
        
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
			// Shader inputs/uniforms
			uniform float4 _LineColor;
			uniform float _LineHalfWidth;
			uniform float _LineSmoothFalloff;

			uniform float _Transparency;
			uniform float4 _PlaneEquation;
			uniform float4 _PositionOffset; // In 2D mode we draw the plane with an offset, which we need to subtract again

			uniform int _HeatmapMode; // 0 = disabled, 1 = potential, 2 = magnitude
            uniform float _VoltageRange;
            uniform float _VoltageOffset;
            uniform float _VoltageInterpolationExponent;

            uniform float _MaxMagnitude;
            uniform float _MagnitudeInterpolationExponent;

			uniform int _DrawEquipotentialLines;
			uniform float _LineSpacingVoltage;

			// --------------------------------------------------------------------------------------------------------------
			// Fragment shader
			float3 getClosestEquipotentialPosition(float3 initialPos, float targetVoltage, float maxSearchRadius)
			{
				if (abs(targetVoltage) > _VoltageRange) return maxSearchRadius * 2.0;

				const int REFINEMENT_STEPS = 8;
				const float MAX_STEP_SIZE = 0.3;

				float voltage;
				float3 _unused;
				evaluateField(initialPos, voltage, _unused);
				voltage -= _VoltageOffset;

				// The nearest point on an equipotential line should be in the direction of the gradient (linear approximation)
				//	and to refine this result further we take a few gradient-descent steps
				//  Note that this does not optimize for 'closest' point on equipotential line, but rather just
				//		searches for a point on the equipotential line, but in practice this is good enough
				float3 pos = initialPos;
				for (int i = 0; i < REFINEMENT_STEPS; i++) 
				{
					// Evaluate Field at current iteration position
					float voltage;
					float3 fieldVector;
					evaluateField(pos, voltage, fieldVector);
					voltage -= _VoltageOffset;

					// Calculate Step
					// Note(MartinR): EField is the negative gradient of the Electric Potential, so we can use it for gradient descend
					float mag = length(fieldVector);
					float3 stepDir = -normalize(fieldVector);
					float stepSize = (targetVoltage - voltage) / mag;
					if (stepSize < 0) {
						stepSize = -stepSize;
						stepDir = -stepDir;
					}
					stepSize = min(stepSize, MAX_STEP_SIZE);
					pos = pos + stepSize * stepDir;

					// Limit search to a radius around initialPos
					float3 toPos = pos - initialPos;
					float distanceFromInitial = length(toPos);
					if (distanceFromInitial > maxSearchRadius) {
						pos = initialPos + toPos / distanceFromInitial * maxSearchRadius;
					}
				}

				return pos;
			}

			float getEquipotentialLineAlpha(float3 pos)
			{
				float voltage;
				float3 _unused;
				evaluateField(pos, voltage, _unused);
				voltage -= _VoltageOffset;

				// Find voltage of the two closest equipotential lines
				float intervalIndex = floor(voltage / _LineSpacingVoltage);
				float targetVoltageLow = intervalIndex * _LineSpacingVoltage;
				float targetVoltageHigh = targetVoltageLow + _LineSpacingVoltage;
				// Make search radius a little larger (1.3), so we can also detect if the line is outside of range
				float maxSearchRadius = (_LineHalfWidth + _LineSmoothFalloff) * 1.3; 

				// Try to find a point near current position which is directly on the equipotential line.
				// We use this point to determine the distance of the current position to the equipotential line,
				//		which is used to color the pixel
				float distanceVoltageLow  = length(pos - getClosestEquipotentialPosition(pos, targetVoltageLow, maxSearchRadius));
				float distanceVoltageHigh = length(pos - getClosestEquipotentialPosition(pos, targetVoltageHigh, maxSearchRadius));
				float distanceClosest     = min(distanceVoltageHigh, distanceVoltageLow);

				// Return smoothed line alpha
				float alpha = 1.0 - smoothstep(_LineHalfWidth, _LineHalfWidth + _LineSmoothFalloff, distanceClosest);
				return alpha;
			}

			float3 GetHeatmapColor(float3 pos) 
			{
				if (_HeatmapMode == 0) return float3(1, 1, 1);

				float voltage;
				float3 fieldValue;
				evaluateField(pos, voltage, fieldValue);
				voltage -= _VoltageOffset;

				if (_HeatmapMode == 1) 
				{
					// Mix color based on voltage
					float alpha = min(1.0, abs(voltage / _VoltageRange));
					alpha = pow(alpha, 1.0 / _VoltageInterpolationExponent);
					
					float3 primaryColor = float3(0, 0, 0);
					if (voltage >= 0) {
						primaryColor.x = 1.0;
					}
					else {
						primaryColor.z = 1.0;
					}

					return lerp(float3(1, 1, 1), primaryColor, alpha);
				}	
				else
				{
					// Mix colors based on magnitude
                    float tMagnitude = length(fieldValue) / _MaxMagnitude;
                    tMagnitude = min(tMagnitude, 1.0);
                    tMagnitude = pow(tMagnitude, 1.0 / _MagnitudeInterpolationExponent);
					return colorRamp5PointGetValue(tMagnitude);
				}
			}

			float4 frag(vert2frag input) : COLOR 
			{
				// Early exit if we have no charged objects
				if (_ChargedPointCount == 0 && _ChargedRodCount == 0 && _ChargedPlaneCount == 0) {
					return float4(1, 1, 1, _Transparency);
				}

				// Project frag-position onto plane (So it works on all mesh types)
				float3 posOnPlane = input.pos_world_space.xyz - _PositionOffset;
				posOnPlane = posOnPlane - _PlaneEquation.xyz * (dot(posOnPlane, _PlaneEquation.xyz) + _PlaneEquation.w);

				// Compose heatmap, equipotential lines and transparency into final color
				float4 outputColor = float4(1, 1, 1, _Transparency);
				if (_HeatmapMode != 0) {
					outputColor.xyz = GetHeatmapColor(posOnPlane);
				}

				if (_DrawEquipotentialLines != 0) {
					float lineAlpha = getEquipotentialLineAlpha(posOnPlane);
					outputColor = lerp(outputColor, _LineColor, lineAlpha);
				}

				return outputColor;
			}

			ENDCG
        }
    }
}