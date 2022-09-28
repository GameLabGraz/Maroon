using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestionnaireSceneLoader: MonoBehaviour
{
    [SerializeField] private Maroon.CustomSceneAsset targetScene;


    public void LoadScene()
    {
        if (targetScene == null) return;
        SceneManager.LoadScene(targetScene.SceneName);
    }
}
