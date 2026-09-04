using UnityEngine;

public static class ToFSensorShaderFeed
{
    private static readonly int MaxDistanceId = Shader.PropertyToID("_MaxDistance");
    private static readonly int SensorClipToViewId = Shader.PropertyToID("_SensorClipToView");
    private static readonly int SensorWorldToViewId = Shader.PropertyToID("_SensorWorldToView");

    /// <summary>
    /// Sets the value for the shader of the given material
    /// </summary>
    public static void UpdateScanMaxDistanceShaderValues(float maxDistance, Material material)
    {
        if (material == null)
        {
            return;
        }
        material.SetFloat(MaxDistanceId, maxDistance);
    }

    /// <summary>
    /// Sets the camera transformation matrices for the absoprtion shader
    /// </summary>
    public static void UpdateScanPerspectiveShaderMatrices(Camera scanCamera, Material material)
    {
        if (material == null)
        {
            return;
        }

        Matrix4x4 sensorGPUProj = GL.GetGPUProjectionMatrix(scanCamera.projectionMatrix, false);
        material.SetMatrix(SensorClipToViewId, sensorGPUProj.inverse);

        material.HasProperty(SensorWorldToViewId);
        material.SetMatrix(SensorWorldToViewId, scanCamera.worldToCameraMatrix);
    }
}
