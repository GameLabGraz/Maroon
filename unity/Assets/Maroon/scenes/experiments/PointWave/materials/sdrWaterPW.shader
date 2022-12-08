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

            // How to transform ? 
            uniform int4 _ClickCoordinates;
            uniform int _ClickedOn;
            uniform int _verticesPerLength;
            uniform int _verticesPerWidth;
            StructuredBuffer<float4> pixels;
            StructuredBuffer<float> _uy;
            uniform int _pixelsSize;
            fixed4 _ColorMin;
            fixed4 _ColorMax;
         //   int localClicked= _ClickedOn;
            float SIGMA = 0.01;
                
            struct vertexInput 
            {
                float4 pos : POSITION; 
                float4 color : COLOR;
            };

            struct v2f 
            {
                float4 sv_position   : SV_POSITION;
                float4 pos_world_space  : TEXCOORD1;
                fixed3 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            int _uyIndex(int x, int z)
            {
               int quotient = _verticesPerLength + x + 1; ;
                int retval = 0;
                if (z < 1)
                {
                    retval = (quotient * (_verticesPerWidth * 2 + 1)) - (_verticesPerWidth - z) - 1;
                }
                else
                {
                    retval = ((quotient - 1) * (_verticesPerWidth * 2 + 1)) + (_verticesPerWidth + z);
                }
                return retval; 

            }

            float _uyElement(int x, int z)
            {
                return _uy[_uyIndex(x, z)];
            }

            float4 vertextIndex(int x, int z)
            {
                int quotient = _verticesPerLength + x + 1; ;
                int retval = 0;
                float4 value = float4(0, 0, 0, 0);
                if (z < 1)
                {
                    retval = (quotient * (_verticesPerWidth * 2 + 1)) - (_verticesPerWidth - z) - 1;
                }
                else
                {
                    retval = ((quotient - 1) * (_verticesPerWidth * 2 + 1)) + (_verticesPerWidth + z);
                }
                if (retval < 0  || retval > _pixelsSize)// test if not error by one 
                {
                    return value;
                }
  
                return  pixels[retval];
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
                if (_EntryCount == 0)
                {
                    output.color = _ColorMin;
                }
                if (_ClickedOn == 1)
                {
                    if (_ClickCoordinates.x == v.pos.x && _ClickCoordinates.z == v.pos.z ) // we get it where the click is
                    {
                        output.color = _ColorMax;
                        output.pos_world_space.w = 1; // needs to be 1, if not somewhing weirds happens
                        output.pos_world_space.y = output.pos_world_space.y +  1; // local vs wolrd coordinates 
                        //output.,
   
                    }
                }
              //  v.vertex;
                // physic i guess;
                /*
                iPrevX = idx(x - 1, z);
                iNextX = idx(x + 1, z);
                iPrevZ = idx(x, z - 1);
                iNextZ = idx(x, z + 1);
                d2x = (v[iNextX].y - 2 * v[i].y + v[iPrevX].y) / DELTA_X2;
                d2z = (v[iNextZ].y - 2 * v[i].y + v[iPrevZ].y) / DELTA_Z2;
                v[i].ay = C2 * (d2x + d2z);
                v[i].ay += -DAMPING * v[i].uy; // asking  where uy what it is look up.
                v[i].uy += dt * v[i].ay;
                v[i].newY = v[i].y + dt * v[i].uy;*/
                /*
                *   N = 60;

                      W = 200;

                *   DELTA_Z = H / N;

                 DELTA_Z2 = DELTA_Z * DELTA_Z;
               / */
                float C = 0.04;
                float damping = -0.001;
                float C2 = C * C;
                int N = 60;
                int W = 100;
                float4 iPrex = vertextIndex(v.pos.x - 1, v.pos.z);
                float4 iNextx = vertextIndex(v.pos.x + 1, v.pos.z);
                float4 iPrez = vertextIndex(v.pos.x, v.pos.z - 1 );
                float4 iNextz = vertextIndex(v.pos.x, v.pos.z + 1);
                float Delta_z = W / N;
                float d2x = (iNextx.y - 2 * v.pos.y + iPrex.y) / Delta_z;
                float d2z = (iNextz.y - 2 * v.pos.y + iPrez.y) / Delta_z;
                float ay = damping *(C2 * (d2x + d2z));
               // _uy[5]
              //  _uy[_uyIndex(v.pos.x, v.pos.z)] = _DeltaTime * ay;

               // float hecommon =  _uyElement(v.pos.x, v.pos.z);
                output.sv_position = mul(UNITY_MATRIX_VP, output.pos_world_space); // Local to global transformation important!
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
