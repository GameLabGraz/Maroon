Shader "Custom/CoulombVoltageShader" {
    Properties {
        _Transparency("Transparency", Range(0.0,1.0)) = 0.75
        _Falloff ("Falloff Exponent", Float) = 1.0
		// Note(MartinR): 2 Charges with 0.1m distance at 10microCoulomb Charge-Difference results in 1 Mega Volt 
        _MaxAbsoluteVoltage ("Maximum Absolute Voltage", Float) = 300000
		// Note(MartinR): To prevent division by 0, the voltage calculation always clamps the distance to a minimum value (Radius of the spheres)
        _MinDistance ("Minimum Distance", Float) = 0.05
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

			struct vertInput{
				float4 pos : POSITION;
			};

			struct vert2frag {
				float4 pos_world_space : TEXCOORD1;
				float4 pos_clip_space  :  SV_POSITION;
			};

			// --------------------------------------------------------------------------------------------------------------
			// Vertex shader

			vert2frag vert(vertInput input) {
				vert2frag output;
				output.pos_world_space = mul(unity_ObjectToWorld, input.pos);
				output.pos_clip_space  = UnityObjectToClipPos(input.pos);
				return output;
			}

			// --------------------------------------------------------------------------------------------------------------
			// Data structures
        
			float _Transparency;
			float _MaxAbsoluteVoltage;
			float _MinDistance;
			float _Falloff;

			uniform int _EntryCnt;
			uniform float4 _Entries[100];            

			// --------------------------------------------------------------------------------------------------------------
			// Fragment shader
        
			float4 frag(vert2frag input) : COLOR {
				if (_EntryCnt == 0) {
					return float4(0, 0, 0, 0);
				}

				// Constants
				float CoulombConstant = 9e9; // Unit is Newton meter^2 / Coulomb^2 [N m^2 / C^2] 
				
				// Calculate voltage
				float voltage = 0.0;
				float3 pos = input.pos_world_space.xyz;
				for(int i = 0; i < _EntryCnt; i++)
				{
					float3 chargePos   = _Entries[i].xyz;
					float  chargeValue = _Entries[i].w;
                
					chargePos.z = pos.z; // 2D-Mode
					float dist = distance(chargePos, pos);
					dist = max(dist, _MinDistance);
					float tmp = CoulombConstant * chargeValue / dist;
					voltage = voltage + tmp;
				}

				// Mix color based on voltage
				float4 outputColor = float4(0, 0, 0, 0);
				float alpha = min(1.0, abs(voltage / _MaxAbsoluteVoltage));
				alpha = pow(alpha, 1.0 / _Falloff);
				outputColor.w = alpha * _Transparency;
				if (voltage >= 0) {
					outputColor.x = 1.0;
				}
				else {
					outputColor.z = 1.0;
				}

				return outputColor;
			}

			ENDCG
        }
    }
}