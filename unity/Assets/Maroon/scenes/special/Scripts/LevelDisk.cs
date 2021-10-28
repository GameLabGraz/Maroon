using TMPro;
using UnityEngine;

public class LevelDisk : Disk
{
    [Header("Loading Settings")]
    [SerializeField] private bool isLabScene = false;
    [SerializeField] private Maroon.CustomSceneAsset experimentScene;
    [SerializeField] private Maroon.SceneCategory category;

    public void SetupLaboratory(Maroon.SceneCategory cat, Maroon.CustomSceneAsset scene)
    {
        isLabScene = true;
        experimentScene = scene;
        category = cat;
    }
    
    public void SetupExperiment(Maroon.CustomSceneAsset scene)
    {
        isLabScene = false;
        experimentScene = scene;
        category = null;
    }
    
    public bool HasScene()
    {
        if (isLabScene)
        {
            return experimentScene != null && category != null;
        }
        return experimentScene != null;
    }

    public void GoToScene()
    {
        if (!HasScene())
            return;

        Debug.Log("Go to " + (isLabScene? "Lab-":"") + "Scene: " + experimentScene.ScenePath);
        
        if (isLabScene)
        {
            Maroon.SceneManager.Instance.SetActiveSceneCategory(category.Name); //otherwise we have (thanks to c#) not the same pointer
        } 
        Maroon.SceneManager.Instance.LoadSceneRequest(experimentScene);
    }
}
