using PlatformControls.BaseControls;
using UnityEngine.SceneManagement;

public class ExitButton : EnterScene
{
    private void Start()
    {
        var sceneIndex = SceneUtility.GetBuildIndexByScenePath("Laboratory.pc");
        gameObject.SetActive(sceneIndex >= 0);
    }
}
