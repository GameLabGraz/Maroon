Shader "Custom/CoulombVoltageShader" {
    Properties {
        _Transparency("Transparency", Range(0.0,1.0)) = 0.75
		// Note(MartinR): 2 Charges with 0.1m distance at 10microCoulomb Charge-Difference results in 1 Mega Volt 
        _MaxAbsoluteVoltage ("Maximum Absolute Voltage", Float) = 300000
		// Note(MartinR): To prevent division by 0, the voltage calculation always clamps the distance to a minimum value
        _MinDistance ("Minimum Distance", Float) = 0.05

		// Note(MartinR): The shader interpolates between 2 colors, either between
		//		Neutral and Max-Color or Neutral and Min-Color, depending on the sign of the voltage at a given position
        _MaxColor("MaxColor", Color)         = (1,0,0,1)
        _MinColor("MinColor", Color)         = (0,0,1,1)
        _NeutralColor("NeutralColor", Color) = (0.7, 0.7, 0.7, 1)
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
			float4 _MinColor;
			float4 _MaxColor;
			float4 _NeutralColor;

			uniform int _EntryCnt;
			uniform float4 _Entries[100];            

			// --------------------------------------------------------------------------------------------------------------
			// Fragment shader
        
			half4 frag(vert2frag input) : COLOR {

				// Constants
				float CoulombConstant = 9e9; // Unit is Newton meter^2 / Coulomb^2 [N m^2 / C^2] 
				
				// Calculate voltage
				float voltage = 0.0;
				for(int i = 0; i < _EntryCnt; ++i)
				{
					float3 charge_entry_pos    = _Entries[i].xyz;
					float  charge_entry_charge = _Entries[i].w;
                
					float dist = max(distance(input.pos_world_space, charge_entry_pos), _MinDistance);
					float tmp = CoulombConstant * charge_entry_charge / dist;
					voltage = voltage + tmp;
				}

				// Mix color based on voltage
				half4 outputColor;
				if (voltage >= 0) {
					outputColor = lerp(_NeutralColor, _MaxColor, min(1.0, voltage / _MaxAbsoluteVoltage));
				}
				else {
					outputColor = lerp(_NeutralColor, _MinColor, min(1.0, voltage / -_MaxAbsoluteVoltage));
				}

				// Apply transparency
				outputColor.w *= _Transparency;
				return outputColor;
			}

			ENDCG
        }
    }
}