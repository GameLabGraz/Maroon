using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using VRStandardAssets.Utils;

public class VRSceneLoader : VRMenuItem
{
    public string sceneName;

    protected override void onActivate()
    {
        base.onActivate();
        StartCoroutine(loadScene(sceneName));
    }

    public static IEnumerator loadScene(string name)
    {
        GameObject vrCameraExtension = GameObject.Find("VRCameraExtension");
        VRCameraFade vrCameraFade = null;
        if (vrCameraExtension != null)
            vrCameraFade = vrCameraExtension.GetComponent<VRCameraFade>();

        if (vrCameraFade != null)
        {
            yield return vrCameraFade.BeginFadeOut(.5f, false);
        }

        SceneManager.LoadScene(name);
    }
}
