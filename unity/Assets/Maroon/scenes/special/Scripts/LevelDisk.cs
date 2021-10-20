using TMPro;
using UnityEngine;

public class LevelDisk : MonoBehaviour
{
    [Header("Loading Settings")]
    [SerializeField] private bool isLabScene = false;
    [SerializeField] private Maroon.CustomSceneAsset experimentScene;
    [SerializeField] private Maroon.SceneCategory category;

    [Header("Design Settings")]
    [SerializeField] private GameObject innerModel;
    [SerializeField] private TextMeshPro nameField;
    
    protected void Start()
    {

    }

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

    public void Setup(Material outsideMaterial, Material highlightImageMaterial, string displayName)
    {
        if (!innerModel)
            return;

        var rend = innerModel.GetComponent<Renderer>();
        if(!rend)
            return;
        
        Debug.Assert(rend.materials.Length == 2);
        var materials = rend.materials;
        materials[0] = outsideMaterial;
        materials[1] = highlightImageMaterial;
        rend.materials = materials;

        nameField.text = displayName;
    }
}
