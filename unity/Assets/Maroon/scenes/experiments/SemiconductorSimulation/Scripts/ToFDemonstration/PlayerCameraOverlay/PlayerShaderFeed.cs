using UnityEngine;

/// <summary>
/// static class that handles the shader values for Overlay Effect of the Demonstrative ToF capture.
/// </summary>
public static class PlayerShaderFeed 
{
    // Performance boost since it doesn't need to hash string into integer ID every frame.
    private static readonly int PlayerInvProjectionId = Shader.PropertyToID("_PlayerInvProjection");
    private static readonly int PlayerCameraToWorldId = Shader.PropertyToID("_PlayerCameraToWorld");
    private static readonly int playerClipToWorldId = Shader.PropertyToID("_PlayerClipToWorld");
    private static readonly int ScanViewProjId = Shader.PropertyToID("_ScanViewProj");
    private static readonly int ScanWorldToCameraId = Shader.PropertyToID("_ScanWorldToCamera");
    private static readonly int ScanColorTexId = Shader.PropertyToID("_ScanColorTex");
    private static readonly int ScanDepthTexId = Shader.PropertyToID("_ScanDepthTex");
    private static readonly int ModeId = Shader.PropertyToID("_Mode");
    private static readonly int DepthToleranceId = Shader.PropertyToID("_DepthTolerance");
    private static readonly int SkipFarDepthId = Shader.PropertyToID("_SkipFarDepth");
    private static readonly int ColourIntensityId = Shader.PropertyToID("_ColourIntensity");

    private static readonly int PlayerClipToScanClipId = Shader.PropertyToID("_PlayerClipToScanClip");
    private static readonly int PlayerClipToScanViewId = Shader.PropertyToID("_PlayerClipToScanView");

    /// <summary>
    /// Gets Values and pushes them to given material shader
    /// optimized for performance by limitating matrix calculations in shader.
    /// </summary>
    public static void UpdatePlayerWorldOverlayShaderValues(
        Material material,
        Camera playerCamera,
        Camera scanCamera,
        RenderTexture scanColorTex,
        RenderTexture scanDepthTex,
        ScanOverlaySettings settings)
    {
        if (material == null || playerCamera == null || scanCamera == null || settings == null)
        {
            return;
        }
        material.SetTexture(ScanColorTexId, scanColorTex);
        material.SetTexture(ScanDepthTexId, scanDepthTex);

        material.SetFloat(DepthToleranceId, settings.DepthTolerance);
        material.SetFloat(SkipFarDepthId, settings.SkipFarDistance);
        material.SetFloat(ColourIntensityId, settings.ColourIntensity);

        Matrix4x4 playerGPUProj = GL.GetGPUProjectionMatrix(playerCamera.projectionMatrix, false);
        Matrix4x4 playerInv = playerGPUProj.inverse;
        Matrix4x4 playerCameraToWorld = playerCamera.cameraToWorldMatrix;

        material.SetMatrix(playerClipToWorldId, playerCameraToWorld * playerInv);

        Matrix4x4 scanGPUProj = GL.GetGPUProjectionMatrix(scanCamera.projectionMatrix, false);
        Matrix4x4 scanVP = scanGPUProj * scanCamera.worldToCameraMatrix;
        Matrix4x4 playerClipToWorld = playerCameraToWorld * playerInv;
        material.SetMatrix(PlayerClipToScanClipId, scanVP * playerClipToWorld);
        material.SetMatrix(PlayerClipToScanViewId, scanCamera.worldToCameraMatrix * playerClipToWorld);
    }

    /// <summary>
    /// Gets Values and pushes them to given material shader
    /// A step by step conversion of the different camera spaces with debug outputs to visualize them.
    /// Used for debugging and testing the shader logic but unoptimized for build.
    /// </summary>
    public static void UpdatePlayerWorldOverlayShaderValuesDebugModes(
        Material material, 
        Camera playerCamera, 
        Camera scanCamera, 
        RenderTexture scanColorTex, 
        RenderTexture scanDepthTex, 
        ScanOverlaySettings settings)
    {
        if (material == null || playerCamera == null || scanCamera == null || settings == null)
        {
            return;
        }
        material.SetTexture(ScanColorTexId, scanColorTex);
        material.SetTexture(ScanDepthTexId, scanDepthTex);

        material.SetFloat(ModeId, settings.DebugMode);
        material.SetFloat(DepthToleranceId, settings.DepthTolerance);
        material.SetFloat(SkipFarDepthId, settings.SkipFarDistance);
        material.SetFloat(ColourIntensityId, settings.ColourIntensity);

        Matrix4x4 playerGPUProj = GL.GetGPUProjectionMatrix(playerCamera.projectionMatrix, false);
        Matrix4x4 playerInv = playerGPUProj.inverse;
        Matrix4x4 playerCameraToWorld = playerCamera.cameraToWorldMatrix;

        material.SetMatrix(PlayerInvProjectionId, playerInv);
        material.SetMatrix(PlayerCameraToWorldId, playerCameraToWorld);

        Matrix4x4 scanGPUProj = GL.GetGPUProjectionMatrix(scanCamera.projectionMatrix, false);
        Matrix4x4 scanVP = scanGPUProj * scanCamera.worldToCameraMatrix;

        material.SetMatrix(ScanViewProjId, scanVP);
        material.SetMatrix(ScanWorldToCameraId, scanCamera.worldToCameraMatrix);
    }
}