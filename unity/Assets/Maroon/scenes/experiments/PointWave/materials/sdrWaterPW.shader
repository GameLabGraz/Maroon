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


            fixed4 _ColorMin;
            fixed4 _ColorMax;

            float SIGMA = 0.01;

            /* COPY PASTE */

          /*N = 60;
            W = 200;

            
            H = W;

            D = 10;

            C = 0.04;

            C2 = C * C;

            DAMPING = 0.001;

            SIM_SPEED = 1;

            DELTA_X = W / N;

            DELTA_X2 = DELTA_X * DELTA_X;

            DELTA_Z = H / N;

            DELTA_Z2 = DELTA_Z * DELTA_Z;

            MAX_DT = 12;

            MAX_ITERATRED_DT = 100;

            MAX_Y = 50;
            */  
            //SIGMA = 0.01;
                
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

                // TESTING 
                    /*    var d2x, d2z, i, iNextX, iNextZ, iPrevX, iPrevZ, v, x, z, _i, _j, _k, _l;
                    v = geometry.vertices;
                    // we itterate all the points in the shared ?  i think so 
                    for (z = _i = 1; 1 <= N ? _i < N : _i > N; z = 1 <= N ? ++_i : --_i) {
                      for (x = _j = 1; 1 <= N ? _j < N : _j > N; x = 1 <= N ? ++_j : --_j) {
                        i = idx(x, z);
                        iPrevX = idx(x - 1, z);
                        iNextX = idx(x + 1, z);
                        iPrevZ = idx(x, z - 1);
                        iNextZ = idx(x, z + 1);
                        d2x = (v[iNextX].y - 2 * v[i].y + v[iPrevX].y) / DELTA_X2;
                        d2z = (v[iNextZ].y - 2 * v[i].y + v[iPrevZ].y) / DELTA_Z2;
                        v[i].ay = C2 * (d2x + d2z);
                        v[i].ay += -DAMPING * v[i].uy;
                        v[i].uy += dt * v[i].ay;
                        v[i].newY = v[i].y + dt * v[i].uy;
                      }
                    }
                    for (z = _k = 1; 1 <= N ? _k < N : _k > N; z = 1 <= N ? ++_k : --_k) {
                      for (x = _l = 1; 1 <= N ? _l < N : _l > N; x = 1 <= N ? ++_l : --_l) {
                        i = idx(x, z);
                        v[i].y = v[i].newY;
                      }
                    }*/

                output.pos_world_space.y = output.pos_world_space.y + lerp(0, 0.03f, amp);
                output.sv_position = mul(UNITY_MATRIX_VP, output.pos_world_space);
                output.color = lerp(_ColorMin, _ColorMax, (amp / mamp));

                if (_EntryCount == 0)
                {
                    output.color = _ColorMin;
                }

                // Tim testing
                if (_ClickCoordinates.x == v.pos.x)
                {
                    output.color = _ColorMax;
                }
                if (_ClickCoordinates.z == v.pos.z)
                {
                    output.color = _ColorMax;
                }
               // {
              //      output.color = _ColorMax;
               // }
                 
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
