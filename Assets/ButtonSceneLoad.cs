using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;
using VRTK.UnityEventHelper;

public class ButtonSceneLoad : MonoBehaviour
{
    private VRTK_Button_UnityEvents _buttonEvents;
    public string SceneName;
    
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

        SceneManager.LoadScene(SceneName);
    }
}