Shader "Unlit/sdrWaterPW"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _ColorMin("Tint Color At Min", Color) = (0.2,0.3,1,1)
        _ColorMax("Tint Color At Max", Color) = (0.2,0.4,1,1)
        _SceneTime("Passing time from outside to enable pause", float) = 0
        _PlatePosition("Position of the Slit Plate", Vector) = (0,0,0)
        _ScalingFactor("Scaling the Basin Wave Length", float) = 0
    }
      
        Category
        {
        Cull Back ZWrite On
        }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag       
                #include "UnityCG.cginc"

                #define PI 3.1415926535897932384626433832795

                fixed4 _ColorMin;
                fixed4 _ColorMax;

                uniform int _BasinEntryCount;
                uniform int _PlateEntryCount;
              
                uniform float _distance;
                uniform float _SceneTime;
                uniform float _WaveLength;
                uniform float _WaveFrequency;
                uniform float _ScalingFactor;

                uniform float _plateParameters[30];
                uniform float _sourceParameters[30];

                uniform float3 _PlatePosition;

                uniform float4 _sourceCoordinates[30];
                uniform float4 _plateCoordinates[30];

                struct vertexInput
                {
                    float4 pos : POSITION;
                };

                struct v2f
                {
                    float4 sv_position : SV_POSITION;
                    float4 pos_world_space : TEXCOORD1;
                    fixed3 color : COLOR0;
                };

                v2f vert(vertexInput v)
                {
                    v2f output;
                    output.sv_position = UnityObjectToClipPos(v.pos);
                    output.pos_world_space = mul(unity_ObjectToWorld, v.pos);

                    float amp = 0;
                    float mamp = 0;
                    float ampPlate = 0;
                    float mampPlate = 0;
                    _distance = 0;

                    if (output.pos_world_space.x <= _PlatePosition.x)
                    {
                        for (int j = 0; j < _BasinEntryCount; j++)
                        {            
                            _distance = _BasinEntryCount == 1 ? distance(_sourceCoordinates[j], output.pos_world_space) : distance(_sourceCoordinates[j].x, output.pos_world_space.x);
                            float w = 2 * PI / (1 / _WaveFrequency);
                            float k = (2 * PI) / (_WaveLength * _ScalingFactor);
                            float u = (_sourceParameters[j] * cos(k * _distance - w * _SceneTime));
                            amp += u;
                        }

                        output.pos_world_space.y = output.pos_world_space.y + lerp(0, 0.05, amp);
                        output.sv_position = mul(UNITY_MATRIX_VP, output.pos_world_space);
                        output.color = lerp(_ColorMin, _ColorMax, amp);
                    }
                    else
                    {
                        for (int j = 0; j < _PlateEntryCount; j++)
                        {
                            _distance = distance(_plateCoordinates[j], output.pos_world_space);
                            float w = 2 * PI / (1 / _WaveFrequency);
                            float k = (2 * PI) / _WaveLength;
                            float u = (_plateParameters[j] * cos(k * _distance - w * _SceneTime));
                            ampPlate += u;
                        }

                        output.pos_world_space.y = output.pos_world_space.y + lerp(0, 0.05, ampPlate);
                        output.sv_position = mul(UNITY_MATRIX_VP, output.pos_world_space);
                        output.color = lerp(_ColorMin, _ColorMax, (ampPlate));
                    }
                    return output;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    return fixed4(i.color, 1);
                }
                ENDCG
            }
        }
}