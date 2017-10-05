Shader "Custom/WaterSurfaceShader" {

	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_HeightMin("Height Min", Float) = -1
		_HeightMax("Height Max", Float) = 1
		_ColorMin("Tint Color At Min", Color) = (0,0,0,1)
		_ColorMax("Tint Color At Max", Color) = (1,1,1,1)
	}

		SubShader{
		Pass{

		CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		float _HeightMax;
	float _HeightMin;
	fixed4 _ColorMin;
	fixed4 _ColorMax;

	struct Input
	{
		float3 worldPos;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		fixed3 color : COLOR0;
	};

	v2f vert(appdata_base v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		float3 pos = v.vertex.xyz;
		float h = (_HeightMax - pos.y) / (_HeightMax - _HeightMin);

		
		o.color = lerp(_ColorMax.rgba, _ColorMin.rgba, h);

		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		return fixed4(i.color, 1);
	}
		ENDCG

	}
	}
}