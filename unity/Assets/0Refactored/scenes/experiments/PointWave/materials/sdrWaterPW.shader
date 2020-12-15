Shader "Unlit/sdrWaterPW"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _ColorMin("Tint Color At Min", Color) = (1,1,0.2,1)
        _ColorMax("Tint Color At Max", Color) = (1,1,0.7,1)
        _SceneTime("Passing time from outside to enable pause", float) = 0
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
                
            uniform int _EntryCount;
            uniform float _distance; 

            // 0 = WaveAmplitude, 1 = WaveLength, 2 = WaveFrequency, 3 = WavePhase.
            uniform float4 _sourceParameters[10];
            uniform float4 _sourceCoordinates[10];
            uniform float _SceneTime;


            fixed4 _ColorMin;
            fixed4 _ColorMax;
                
            struct vertexInput 
            {
                float4 pos : POSITION;
            };

            struct v2f 
            {
                float4 sv_position   : SV_POSITION;
                float4 pos_world_space  : TEXCOORD1;
                fixed3 color : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (vertexInput v)
            {
                v2f output;
                output.sv_position = UnityObjectToClipPos(v.pos);
                output.pos_world_space = mul(unity_ObjectToWorld, v.pos);
                output.color = fixed3(1, 1, 1);

                float amp = 0;
                float mamp = 0.01f;
                _distance = 0;

                for (int j = 0; j < _EntryCount; j++)
                {
                    mamp += _sourceParameters[j].x;
                    _distance = distance(_sourceCoordinates[j], output.pos_world_space);

                    float w = 2 * PI / (1 / _sourceParameters[j].z);
                    float k = (2 * PI) / (_sourceParameters[j].y * 0.2f);
                    float r = sqrt(_distance);
                    float u = (_sourceParameters[j].x * cos(k * _distance - w * _SceneTime + _sourceParameters[j].w)) / r;

                    amp += u;
                }

                output.pos_world_space.y = output.pos_world_space.y + lerp(0, 0.03f, amp);
                output.sv_position = mul(UNITY_MATRIX_VP, output.pos_world_space);
                output.color = lerp(_ColorMin, _ColorMax, (amp / mamp));

                if (_EntryCount == 0)
                {
                    output.color = _ColorMin;
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
