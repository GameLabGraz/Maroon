using UnityEngine;

public static class TargetPlatformDetector
{
    public static bool isVRPlatform = false;

    public static string targetPlatform = "undefined";
    
    static TargetPlatformDetector()
    {
        // Detect Scene type
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if(sceneName.Contains(".vr"))
        {
            TargetPlatformDetector.isVRPlatform = true;
        }
        else
        {
            TargetPlatformDetector.isVRPlatform = false;
        }

        // Detect Platform
        #if UNITY_EDITOR
            TargetPlatformDetector.targetPlatform = "editor";
        #elif UNITY_ANDROID
            TargetPlatformDetector.targetPlatform = "android";
        #elif UNITY_IOS 
            TargetPlatformDetector.targetPlatform = "ios";
        #elif UNITY_STANDALONE_OSX
            TargetPlatformDetector.targetPlatform = "mac";
        #elif UNITY_STANDALONE_WIN
            TargetPlatformDetector.targetPlatform = "pc";
        #endif    
    }
}


