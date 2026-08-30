Shader "Unlit/VectorFieldGridCalculationShader"
{
    SubShader
    {
        Lighting Off

        Pass
        {
            CGPROGRAM

            #include "UnityCustomRenderTexture.cginc"
            #include "UnityCG.cginc"
            #include "ElectricFieldShaderUtils.cginc"

            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag

            uniform uint _GridResolutionX;
            uniform uint _GridResolutionY;
            uniform uint _GridResolutionZ;
            uniform float4 _GridMin;
            uniform float _CellSize;

            float4 frag (v2f_customrendertexture fragInput) : SV_Target
            {
                float2 uv = fragInput.localTexcoord.xy;
                uint2 pixelCoord;
                pixelCoord.x = (uint)(uv.x * _CustomRenderTextureWidth);
                pixelCoord.y = (uint)(uv.y * _CustomRenderTextureHeight);

                uint linearIndex = pixelCoord.x + pixelCoord.y * (uint)_CustomRenderTextureWidth;
                if (linearIndex > _GridResolutionX * _GridResolutionY * _GridResolutionZ) return float4(0, 1, 0, 0);

                // Find cell index in grid
                uint3 cellIndex = uint3(
                    (linearIndex % _GridResolutionX), 
                    (linearIndex / _GridResolutionX) % _GridResolutionY,
                    (linearIndex / (_GridResolutionX * _GridResolutionY)) % _GridResolutionZ);

                // Evaluate field at grid center
                float3 pos = _GridMin.xyz + (((float3) cellIndex) + 0.5) * _CellSize;
                float voltage = 0.0;
                float3 fieldValue = float3(0, 0, 0);
                evaluateField(pos, voltage, fieldValue);

                return float4(fieldValue.x, fieldValue.y, fieldValue.z, voltage);
            }
            ENDCG
        }
    }
}
