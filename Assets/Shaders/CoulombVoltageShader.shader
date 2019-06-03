// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CoulombVoltageShader" {
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _HueBased("HueBased", Range(0,1)) = 1
        _Transparency("Transparency", Range(0.0,1.0)) = 0.75
        _minValue ("Minimum Charge", Range(-10, 0)) = -5
        _maxValue ("Maximum Charge", Range(0, 10)) = 5
        _minColor("MinColor", Color) = (1,1,1,1)
        _maxColor("MaxColor", Color) = (1,1,1,1)
    }
    
    SubShader {
//        Tags {"Queue"="Background"  "IgnoreProjector"="True"}
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass {
        CGPROGRAM
        #pragma vertex vert  
        #pragma fragment frag
        #include "UnityCG.cginc"
        
        float _HueBased;
        float _Transparency;
        float _minValue;
        float _maxValue;
        float4 _minColor;
        float4 _maxColor;
        
        uniform int _EntryCnt;
        uniform float4 _Entries[100];       
        
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
        
        struct vertInput {
            float3 pos : POSITION;
        };  
        
        struct vertOutput {
            float4 pos : SV_POSITION;
            float4 cmp_pos : TEXCOORD0;
        };
        
        vertOutput vert(vertInput input) {
            vertOutput o;
            
            o.cmp_pos = float4(input.pos, 1);
            o.pos = UnityObjectToClipPos(input.pos);
            return o;
        }
        
        half4 frag(vertOutput input) : COLOR {
        
            float CoulombConstant = 9; 
            float CoulombMultiplyFactor = 0.001;
            float voltage = 0.0;
            float3 world_pos = mul (unity_ObjectToWorld, input.cmp_pos.xyz);
            int entries = _EntryCnt;
            for(int i = 0; i < entries; ++i){
                float3 pos = _Entries[i].xyz;
                float charge = _Entries[i].w;
                
                float dist = distance(world_pos, pos);
                float tmp = CoulombConstant * charge / dist; //(dist * dist);
                voltage = voltage + tmp;
            }
        
              
//            float voltage = input.cmp_pos.x; //input.cmp_pos.x;
            
            
//            if(voltage < 0) voltage = voltage * -1;
            voltage = clamp(voltage, _minValue, _maxValue);
            float range = _maxValue - _minValue;
            float percentage =  (voltage - _minValue) / range;

//            return half4(percentage, percentage, percentage, 1.0);
            
            if(_HueBased > 0.5){
                percentage = 1.0 - percentage;
                float3 HSV = float3((percentage * 240) / 360, 1, 1);   
                float3 RGB = hsv_to_rgb(HSV);        
                return half4(RGB, _Transparency); 
            }
            else if(percentage < 0.5) {
                percentage = 1.0 - percentage;
                return half4(_minColor.xyz, (percentage - 0.5) * 2.0);
            }
            else{
                return half4(_maxColor.xyz, (percentage - 0.5) * 2.0);
            }
        }
         
                 
        //min = 0, max = 360 -> but we only go to max = 240
         
//         
// 
//         struct v2f {
//             float4 pos : SV_POSITION;
//             float4 texcoord : TEXCOORD0;
//         };
// 
//         v2f vert (appdata_full v) {
//             v2f o;
//             o.pos = UnityObjectToClipPos (v.vertex);
//             o.texcoord = v.texcoord;
//             return o;
//         }
// 
//         fixed4 frag (v2f i) : COLOR {
//             fixed4 c = lerp(_ColorBot, _ColorMid, i.texcoord.y / _Middle) * step(i.texcoord.y, _Middle);
//             c += lerp(_ColorMid, _ColorTop, (i.texcoord.y - _Middle) / (1 - _Middle)) * step(_Middle, i.texcoord.y);
//             c.a = 1;
//             return c;
//         }
        ENDCG
        }
    }
}