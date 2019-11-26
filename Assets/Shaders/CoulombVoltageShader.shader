Shader "Custom/CoulombVoltageShader" {
    Properties {
        _HueBased("HueBased", Range(0,1)) = 1
        _Transparency("Transparency", Range(0.0,1.0)) = 0.75
        _minValue ("Minimum Charge", Range(-10, 10)) = 0 //-0.001
        _maxValue ("Maximum Charge", Range(0, 10)) = 0.001
        _multFactor ("Mult Factor Charge", Range(0, 10)) = 0.001
        _minColor("MinColor", Color) = (1,1,1,1)
        _maxColor("MaxColor", Color) = (1,1,1,1)
        _Distance("Distance", Range(0, 100)) = 1
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
				float4 pos_object_space : TEXCOORD0;
				float4 pos_world_space  : TEXCOORD1;
				float4 pos_clip_space   : SV_POSITION;
			};

			// --------------------------------------------------------------------------------------------------------------
			// Vertex shader

			vert2frag vert(vertInput input) {
				vert2frag output;
				output.pos_object_space = input.pos;
				output.pos_world_space  = mul(unity_ObjectToWorld, input.pos.xyz);
				output.pos_clip_space   = UnityObjectToClipPos(input.pos);
				return output;
			}

			// --------------------------------------------------------------------------------------------------------------
			// Data structures
        
			float _HueBased;
			float _Transparency;
			float _minValue;
			float _maxValue;
			float _multFactor;
			float4 _minColor;
			float4 _maxColor;
			float _Distance;

			uniform int _EntryCnt;
			uniform float4 _Entries[100];            

			// --------------------------------------------------------------------------------------------------------------
			// Helper functions
        
			float3 hsv_to_rgb(float3 HSV) {
				float3 RGB = HSV.z;           
				float var_h = HSV.x * 6;
				float var_i = floor(var_h);   // Or ... var_i = floor( var_h )
				float var_1 = HSV.z * (1.0 - HSV.y);
				float var_2 = HSV.z * (1.0 - HSV.y * (var_h-var_i));
				float var_3 = HSV.z * (1.0 - HSV.y * (1-(var_h-var_i)));
				if      (var_i == 0) { RGB = float3(HSV.z, var_3, var_1); }
				else if (var_i == 1) { RGB = float3(var_2, HSV.z, var_1); }
				else if (var_i == 2) { RGB = float3(var_1, HSV.z, var_3); }
				else if (var_i == 3) { RGB = float3(var_1, var_2, HSV.z); }
				else if (var_i == 4) { RGB = float3(var_3, var_1, HSV.z); }
				else                 { RGB = float3(HSV.z, var_1, var_2); }
           
				return (RGB);
			}
			
			// --------------------------------------------------------------------------------------------------------------
			// Fragment shader
        
			half4 frag(vert2frag input) : COLOR {

				//return half4(1, 0, 0, 1);

				// Variables
				float CoulombConstant = 9; 
				float CoulombMultiplyFactor = 0.001;
				float voltage = 0.0;
				
				// Calculate voltage
				for(int i = 0; i < _EntryCnt; ++i)
				{
					float3 charge_entry_pos    = _Entries[i].xyz;
					float  charge_entry_charge = _Entries[i].w;
                
					float dist = distance(input.pos_world_space, charge_entry_pos);
                
					if(input.pos_clip_space.x <= 0.5 && input.pos_clip_space.y <= 0.5)
						return half4(1, 0, 0, 1);

					if(dist < 0)
						dist = 0;

					float tmp = CoulombConstant * CoulombMultiplyFactor * charge_entry_charge * _multFactor / (dist * dist);
					voltage = voltage + tmp;
				}

				return half4(voltage, 0, 0, 1);
        
				//voltage = clamp(voltage, _minValue, _maxValue);
				int neg = 0;
				if(voltage < 0){
					neg = 1;
					voltage *= -1;
				}
				voltage = clamp(voltage, _minValue, _maxValue);
            
				float range = _maxValue - _minValue;
				float percentage =  (voltage - _minValue) / range;
            
				percentage = 0.5 + percentage / 2;
				if(neg == 1){
					percentage = 1 - percentage;
				}
            
				if(_HueBased > 0.5){
					percentage = 1.0 - percentage;
					float3 HSV;
                
					int border1 = 240;
					int border2 = 160;
					int border3 = 80; 
					float step = 0.49;
					if(percentage > 1.0 - step){
						HSV = float3((border2 + ((percentage - (1.0 - step)) / step) * (border1 - border2)) / 360, 1, 1); 
					}
					else if(percentage > step){
						HSV = float3((border3 + ((percentage - step) / (1.0 - 2.0 * step)) * (border2 - border3)) / 360, 1, 1);
					}
					else {
						HSV = float3(((percentage / step) * border3) / 360, 1, 1);
					}
                
	//                float3 HSV = float3((percentage * 240) / 360, 1, 1);   
					float3 RGB = hsv_to_rgb(HSV);        
					return half4(RGB, _Transparency); 
				}
				else if(percentage < 0.5) {
					percentage = 1.0 - percentage;
					return half4(_minColor.xyz, (percentage - 0.5) * 2.0 * _Transparency);
				}
				else{
					return half4(_maxColor.xyz, (percentage - 0.5) * 2.0 * _Transparency);
				}
			}

			ENDCG
        }
    }
}