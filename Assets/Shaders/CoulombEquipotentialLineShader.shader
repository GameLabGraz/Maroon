// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CoulombEquipotentialLineShader" {
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}        
        _LineColor("Line Color", Color) = (1,1,1,1)
        _BkgdColor("Background Color", Color) = (1,1,1,1)
        _LineWidth("Line Width", Range(0, 1)) = 0.0001
        _ChargeStep("Charge Step", Range(0,5)) = 0.5
        _ChargeStepTolerance("Charge Step Tolerance", Range(0.0,1.0)) = 0.001
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
        
        float _ChargeStep;
        float _ChargeStepTolerance;
        float4 _LineColor;
        float4 _BkgdColor;
        float _LineWidth;
        
        uniform int _EntryCnt;
        uniform float4 _Entries[100];       
        
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
            float3 pos1, pos2, pos3, pos4;
            pos1 = pos2 = pos3 = pos4 = input.cmp_pos.xyz;
            pos1.x = pos2.x = input.cmp_pos.x - _LineWidth;
            pos3.x = pos4.x = input.cmp_pos.x + _LineWidth;
            
            pos1.z = pos3.z = input.cmp_pos.z - _LineWidth;
            pos2.z = pos4.z = input.cmp_pos.z + _LineWidth;
            
            float3 world_pos1 = mul (unity_ObjectToWorld, pos1);
            float3 world_pos2 = mul (unity_ObjectToWorld, pos2);
            float3 world_pos3 = mul (unity_ObjectToWorld, pos3);
            float3 world_pos4 = mul (unity_ObjectToWorld, pos4);
                        
            //TODO: check voltage on 4 neigbouring points -> not a tolerance for the voltage -> use linewidth
             int entries = _EntryCnt;
            float voltage1 = 0.0;
            float voltage2 = 0.0;
            float voltage3 = 0.0;
            float voltage4 = 0.0;
            
            float radius = 0.71;
            for(int i = 0; i < entries; ++i){
                float3 pos = _Entries[i].xyz;
                float charge = _Entries[i].w;
                
                float dist = distance(world_pos1, pos) - radius;
                if(dist < 0) dist = 0;
                float tmp = CoulombConstant * CoulombMultiplyFactor * charge / (dist * dist); //TODO: check formula
                voltage1 = voltage1 + tmp;
                
                
                dist = distance(world_pos2, pos) - radius;
                if(dist < 0) dist = 0;
                tmp = CoulombConstant * CoulombMultiplyFactor * charge / (dist * dist); //TODO: check formula
                voltage2 = voltage2 + tmp;
                
                
                dist = distance(world_pos3, pos) - radius;
                if(dist < 0) dist = 0;
                tmp = CoulombConstant * CoulombMultiplyFactor * charge / (dist * dist); //TODO: check formula
                voltage3 = voltage3 + tmp;
                
                
                dist = distance(world_pos4, pos) - radius;
                if(dist < 0) dist = 0;
                tmp = CoulombConstant * CoulombMultiplyFactor * charge / (dist * dist); //TODO: check formula
                voltage4 = voltage4 + tmp;
            }           
                  
             
            if(voltage1 < 0.0) voltage1 *= -1.0;
            if(voltage2 < 0.0) voltage2 *= -1.0;
            if(voltage3 < 0.0) voltage3 *= -1.0;
            if(voltage4 < 0.0) voltage4 *= -1.0;
            
            if(fmod(voltage1, _ChargeStep) < 2.0 * _ChargeStepTolerance
                || fmod(voltage2, _ChargeStep) < 2.0 * _ChargeStepTolerance
                || fmod(voltage3, _ChargeStep) < 2.0 * _ChargeStepTolerance
                || fmod(voltage4, _ChargeStep) < 2.0 * _ChargeStepTolerance){
                return _LineColor;
            }
            else{
                return _BkgdColor;
            }      
                  
//                       
//                       
//            int entries = _EntryCnt;
//            float radius = 0.71;
//            for(int i = 0; i < entries; ++i){
//                float3 pos = _Entries[i].xyz;
//                float charge = _Entries[i].w;
//                
//                float dist = distance(world_pos, pos) - radius;
//                if(dist < 0) dist = 0;
//                float tmp = CoulombConstant * CoulombMultiplyFactor * charge / (dist * dist); //TODO: check formula
//                voltage = voltage + tmp;
//            }
//            
//            if(voltage < 0.0)
//                voltage *= -1.0;
//            
//            if(fmod(voltage + _ChargeStepTolerance, _ChargeStep) < 2.0 * _ChargeStepTolerance){
//                return _LineColor;
//            }
//            else{
//                return half4(1, 0, 0, 1);
//            }
        }

        ENDCG
        }
    }
}