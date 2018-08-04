using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using VRStandardAssets.Utils;

public class VRSceneLoader : VRMenuItem
{
    public string sceneName;

    [SerializeField] private GazeOnly_SelectionRadial m_GazeOnly_SelectionRadial;

    public virtual void OnEnable()
    {
        if(m_GazeOnly_SelectionRadial)
            m_GazeOnly_SelectionRadial.OnSelectionComplete += onActivate;
    }

    public virtual void OnDisable()
    {
        if (m_GazeOnly_SelectionRadial)
            m_GazeOnly_SelectionRadial.OnSelectionComplete -= onActivate;
    }

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
