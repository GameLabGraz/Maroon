using GEAR.Localization.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrMenuColumnLaboratory : MonoBehaviour
{
    // Scenes
    [SerializeField] private Maroon.CustomSceneAsset targetLabScenePC;

    [SerializeField] private Maroon.CustomSceneAsset targetLabSceneVR;

    // Buttons
    [SerializeField] private GameObject CategoryButtonsContainer;

    [SerializeField] private GameObject CategoryButtonPrefab;

    [SerializeField] private Texture TextureIconButton;

    private void Start()
    {
        // Get scene categories based on current platform
        Maroon.SceneType platformSceneType = Maroon.PlatformManager.Instance.SceneTypeBasedOnPlatform;
        List<Maroon.SceneCategory> categories = Maroon.SceneManager.Instance.getSceneCategories(platformSceneType);

        // Create buttons based on categories
        for (int iCategories = 0; iCategories < categories.Count; iCategories++)
        {
            // Extract category
            Maroon.SceneCategory current_category = categories[iCategories];

            // Create new button, add to panel and scale
            GameObject newButton = Instantiate(this.CategoryButtonPrefab, Vector3.zero, Quaternion.identity,
                                               this.CategoryButtonsContainer.transform) as GameObject;
            newButton.transform.localScale = Vector3.one;

            // Set text
            Transform text = newButton.transform.Find("ContentContainer").transform.Find("Text (TMP)");
            text.GetComponent<LocalizedTMP>().Key = current_category.Name;

            // Link function
            newButton.GetComponent<Button>().onClick.AddListener(() => this.SetCategoryAndLoadLaboratoryScene(current_category));
        }

    }

    private void SetCategoryAndLoadLaboratoryScene(Maroon.SceneCategory category)
    {
        // Set category
        Maroon.SceneManager.Instance.ActiveSceneCategory = category;

        // Load laboratory scene
        if(Maroon.PlatformManager.Instance.CurrentPlatformIsVR)
        {
            Maroon.SceneManager.Instance.LoadSceneIfInAnyCategory(this.targetLabSceneVR);
        }

        else
        {
            Maroon.SceneManager.Instance.LoadSceneIfInAnyCategory(this.targetLabScenePC);
        }
    }
}
