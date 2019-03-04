using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NativeVideoPlayer {

    private static System.IntPtr _Activity;
    private static System.IntPtr _VideoPlayerClass;

    private static System.IntPtr VideoPlayerClass
    {
        get
        {
            if (_VideoPlayerClass == System.IntPtr.Zero)
            {
                try 
                {
                    System.IntPtr myVideoPlayerClass = AndroidJNI.FindClass("com/oculus/videoplayer/MyVideoPlayer");

                    if (myVideoPlayerClass != System.IntPtr.Zero)
                    {
                        _VideoPlayerClass = AndroidJNI.NewGlobalRef(myVideoPlayerClass);

                        AndroidJNI.DeleteLocalRef(myVideoPlayerClass);
                    }
                }
                catch(System.Exception ex)
                {
                    Debug.LogError("Failed to find MyVideoPlayer class");
                    Debug.LogException(ex);
                }
            }
            return _VideoPlayerClass;
        }
    }

    private static System.IntPtr Activity
    {
        get
        {
            if (_Activity == System.IntPtr.Zero)
            {
                System.IntPtr unityPlayerClass = AndroidJNI.FindClass("com/unity3d/player/UnityPlayer");
                System.IntPtr currentActivityField = AndroidJNI.GetStaticFieldID(unityPlayerClass, "currentActivity", "Landroid/app/Activity;");
                System.IntPtr activity = AndroidJNI.GetStaticObjectField(unityPlayerClass, currentActivityField);

                _Activity = AndroidJNI.NewGlobalRef(activity);

                AndroidJNI.DeleteLocalRef(activity);
                AndroidJNI.DeleteLocalRef(unityPlayerClass);
            }
            return _Activity;
        }
    }

    private static System.IntPtr playVideoMethodId;
    private static System.IntPtr stopMethodId;
    private static System.IntPtr resumeMethodId;
    private static System.IntPtr pauseMethodId;

    public static bool IsAvailable
    {
        get
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return VideoPlayerClass != System.IntPtr.Zero;
#else
            return false;
#endif
        }
    }

    public static void PlayVideo(string path, System.IntPtr surfaceObj)
    {
        if (playVideoMethodId == System.IntPtr.Zero)
        {
            playVideoMethodId = AndroidJNI.GetStaticMethodID(VideoPlayerClass, "playVideo", "(Landroid/content/Context;Ljava/lang/String;Landroid/view/Surface;)V");
        }

        System.IntPtr filePathJString = AndroidJNI.NewStringUTF(path);

        AndroidJNI.CallStaticVoidMethod(VideoPlayerClass, playVideoMethodId, new jvalue[] { new jvalue { l = Activity }, new jvalue { l = filePathJString }, new jvalue { l = surfaceObj } });

        AndroidJNI.DeleteLocalRef(filePathJString);
    }

    public static void Stop()
    {
        if (stopMethodId == System.IntPtr.Zero)
        {
            stopMethodId = AndroidJNI.GetStaticMethodID(VideoPlayerClass, "stop", "()V");
        }

        AndroidJNI.CallStaticVoidMethod(VideoPlayerClass, stopMethodId, new jvalue[0]);
    }

    public static void Play()
    {
        if (resumeMethodId == System.IntPtr.Zero)
        {
            resumeMethodId = AndroidJNI.GetStaticMethodID(VideoPlayerClass, "resume", "()V");
        }

        AndroidJNI.CallStaticVoidMethod(VideoPlayerClass, resumeMethodId, new jvalue[0]);        
    }

    public static void Pause()
    {
        if (pauseMethodId == System.IntPtr.Zero)
        {
            pauseMethodId = AndroidJNI.GetStaticMethodID(VideoPlayerClass, "pause", "()V");
        }

        AndroidJNI.CallStaticVoidMethod(VideoPlayerClass, pauseMethodId, new jvalue[0]);        
    }
}
