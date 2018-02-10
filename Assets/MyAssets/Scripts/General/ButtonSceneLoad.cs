using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;
using VRTK.UnityEventHelper;

public class ButtonSceneLoad : MonoBehaviour
{
    private VRTK_Button_UnityEvents _buttonEvents;
    public string SceneName;
    public SceneLoad SceneLoading = SceneLoad.Async;

    public enum SceneLoad
    {
        Async,
        Synchron,
        AsyncAdditive,
        SynchronAdditive
    }

    private void Start()
    {
        _buttonEvents = GetComponent<VRTK_Button_UnityEvents>();
        if (_buttonEvents == null)
            _buttonEvents = gameObject.AddComponent<VRTK_Button_UnityEvents>();

        _buttonEvents.OnPushed.AddListener(HandlePush);
    }

    private void HandlePush(object sender, Control3DEventArgs e)
    {
        VRTK_Logger.Info("Pushed");

        switch (SceneLoading)
        {
            case SceneLoad.Async:
                SceneManager.LoadSceneAsync(SceneName);
                break;
            case SceneLoad.Synchron:
                SceneManager.LoadScene(SceneName);
                break;
            case SceneLoad.AsyncAdditive:
                SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
                break;
            case SceneLoad.SynchronAdditive:
                SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
                break;
        }
    }
}