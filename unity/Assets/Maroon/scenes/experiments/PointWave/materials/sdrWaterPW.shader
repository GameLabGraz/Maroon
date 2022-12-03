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
            uniform float _DeltaTime;
           // uniform int3 _test;
       //     uniform float4 _Vertices[10000];
            // We need Compute Buffer, because this limit ist 1024 something to pass laut  
        // https://forum.unity.com/threads/material-setvectorarray-array-size-limit.512068/

            // How to transform ? 
            uniform int4 _ClickCoordinates;
            uniform int _ClickedOn;
            uniform int _verticesPerLength;
            uniform int _verticesPerWidth;
            StructuredBuffer<float4> pixels;
            fixed4 _ColorMin;
            fixed4 _ColorMax;

            float SIGMA = 0.01;
                
            struct vertexInput 
            {
                float4 pos : POSITION;
             //   float4 x1p
             //   float4 x1m
             //   float4 z1p
             //   float4 z1n
            };

            struct v2f 
            {
                float4 sv_position   : SV_POSITION;
                float4 pos_world_space  : TEXCOORD1;
                fixed3 color : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            int vertextIndex(int x, int z)
            {
                int quotient = (_verticesPerLength + x)  ; // shoul work;
                return quotient * 162 - z; // must be ok? magic number;
         
           
            }


            v2f vert (vertexInput v)
            {

                v2f output;
                output.sv_position = UnityObjectToClipPos(v.pos);
                float4 click = UnityObjectToClipPos(_ClickCoordinates);
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
                  //  u += (_ClickCoordinates.x / 100);
                    amp += u;
                }

                output.pos_world_space.y = output.pos_world_space.y + lerp(0, 0.03f, amp);
                output.sv_position = mul(UNITY_MATRIX_VP, output.pos_world_space);
                output.color = lerp(_ColorMin, _ColorMax, (amp / mamp));

                if (_EntryCount == 0)
                {
                    output.color = _ColorMin;
                }

                if (_ClickedOn == 1)
                {
                    // acording to the webpage
                    float x = v.pos.x - _ClickCoordinates.x;
                    float z = v.pos.z - _ClickCoordinates.z;


                        /*
                output.pos_world_space.y = output.pos_world_space.y + lerp(0, 0.03f, amp);
                output.sv_position = mul(UNITY_MATRIX_VP, output.pos_world_space);
                output.color = lerp(_ColorMin, _ColorMax, (amp / mamp));
                        */
                    if (v.pos.x == ((-_verticesPerLength) / 2) || v.pos.x == (_verticesPerLength / 2) || v.pos.z == (_verticesPerWidth / 2) || v.pos.y == (-_verticesPerWidth / 2))
                    {
                        output.pos_world_space.y = 0; 
                    }
                    if (_ClickCoordinates.x == v.pos.x && _ClickCoordinates.z == v.pos.z ) // we get it where the click is

                    {
                        output.color = _ColorMax;
                        amp = 10;
                        output.pos_world_space.y = output.pos_world_space.y + lerp(0, 0.03f, amp); // further work 

                       // output.color = lerp(_ColorMin, _ColorMax, (amp / mamp));
                  //      output.sv_position = mul(UNITY_MATRIX_VP, output.pos_world_space);
                    }


                    // TESTING THE FUNCTION ;
                    
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
