using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class VRApplicationQuitter : VRMenuItem
{
    protected override void onActivate()
    {
        base.onActivate();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
