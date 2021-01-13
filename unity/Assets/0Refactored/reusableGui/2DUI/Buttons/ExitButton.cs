using PlatformControls.BaseControls;
using UnityEngine.SceneManagement;

public class ExitButton : EnterScene
{
    private void Start()
    {
        var laboratoryScene = SceneManager.GetSceneByName("Laboratory.pc");
        gameObject.SetActive(laboratoryScene.IsValid());
    }
}
