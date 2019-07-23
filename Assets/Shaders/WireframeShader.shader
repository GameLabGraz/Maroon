// inspiration from https://answers.unity.com/questions/309489/how-to-use-the-barymetric-wireframe-shader.html
Shader "Custom/WireframeShader"
{
    Properties
    {
        _LineColorFront ("Front Line Color", Color) = (1,1,1,1)
        _GridColorFront ("Front Grid Color", Color) = (1,1,1,0)
        _LineWidthFront ("Back Line Width", float) = 0.2
        _LineColorBack ("Back Line Color", Color) = (1,1,1,0)
        _GridColorBack ("Back Grid Color", Color) = (1,1,1,0)
        _LineWidthBack ("Back Line Width", float) = 0.2
        [Toggle] _RemoveDiag("Remove diagonals?", Float) = 0.
    }
    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
//        AlphaTest Greater 0.5
    Pass
    {
      Cull Front
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
 
      uniform float4 _LineColorBack;
      uniform float4 _GridColorBack;
      uniform float _LineWidthBack;
 
      // vertex input: position, uv1, uv2
      struct appdata
      {
        float4 vertex : POSITION;
        float4 texcoord1 : TEXCOORD0;
        float4 color : COLOR;
      };
 
      struct v2f
      {
        float4 pos : POSITION;
        float4 texcoord1 : TEXCOORD0;
        float4 color : COLOR;
      };
 
      v2f vert (appdata v)
      {
        v2f o;
        o.pos = UnityObjectToClipPos( v.vertex);
        o.texcoord1 = v.texcoord1;
        o.color = v.color;
        return o;
      }
 
      fixed4 frag(v2f i) : COLOR
      {
        fixed4 answer;
 
        float lx = step(_LineWidthBack, i.texcoord1.x);
        float ly = step(_LineWidthBack, i.texcoord1.y);
        float hx = step(i.texcoord1.x, 1.0 - _LineWidthBack);
        float hy = step(i.texcoord1.y, 1.0 - _LineWidthBack);
 
        answer = lerp(_LineColorBack, _GridColorBack, lx*ly*hx*hy);
 
        return answer;
      }
      ENDCG
     }
    
    Pass
    {
      Cull Back
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
 
      uniform float4 _LineColorFront;
      uniform float4 _GridColorFront;
      uniform float _LineWidthFront;
 
      // vertex input: position, uv1, uv2
      struct appdata
      {
        float4 vertex : POSITION;
        float4 texcoord1 : TEXCOORD0;
        float4 color : COLOR;
      };
 
      struct v2f
      {
        float4 pos : POSITION;
        float4 texcoord1 : TEXCOORD0;
        float4 color : COLOR;
      };
 
      v2f vert (appdata v)
      {
        v2f o;
        o.pos = UnityObjectToClipPos( v.vertex);
        o.texcoord1 = v.texcoord1;
        o.color = v.color;
        return o;
      }
 
      fixed4 frag(v2f i) : COLOR
      {
        fixed4 answer;
 
        float lx = step(_LineWidthFront, i.texcoord1.x);
        float ly = step(_LineWidthFront, i.texcoord1.y);
        float hx = step(i.texcoord1.x, 1.0 - _LineWidthFront);
        float hy = step(i.texcoord1.y, 1.0 - _LineWidthFront);
 
        answer = lerp(_LineColorFront, _GridColorFront, lx*ly*hx*hy);
 
        return answer;
      }
      ENDCG
     }
  } 
  Fallback "Vertex Colored", 1
 }
 
 