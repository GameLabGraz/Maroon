using GEAR.Localization.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrMenuColumnLaboratorySelection : MonoBehaviour
{
    private scrMenu Menu;

    // Column
    [SerializeField] private GameObject ColumnLaboratorySelectionDetail;


    // Buttons
    [SerializeField] private GameObject CategoryButtonsContainer;

    [SerializeField] private GameObject CategoryButtonPrefab;

    [SerializeField] private Texture TextureIconButton;

    private void Start()
    {
        // Link scrMenu
        this.Menu = FindObjectOfType<scrMenu>();

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

            // Set text TODO: Make this work with Localization
            Transform text = newButton.transform.Find("ContentContainer").transform.Find("Text (TMP)");
            text.GetComponent<LocalizedTMP>().Key = current_category.Name;

            // Link function
            newButton.GetComponent<Button>().onClick.AddListener(() => this.SetCategoryAndOpenColumn(newButton, current_category));
        }

    }

    private void SetCategoryAndOpenColumn(GameObject pressedButton, Maroon.SceneCategory category)
    {
        // Set category
        Maroon.SceneManager.Instance.ActiveSceneCategory = category;

        // Load laboratory scene
        this.Menu.RemoveAllMenuColumnsButTwo();
        this.Menu.AddMenuColumn(this.ColumnLaboratorySelectionDetail);
        this.ClearButtonActiveIcons();
        this.SetButtonActiveIcon(pressedButton);
    }

    private void ClearButtonActiveIcons()
    {
        foreach(Transform button in this.CategoryButtonsContainer.transform)
        {
            button.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        }
    }

    private void SetButtonActiveIcon(GameObject btn)
    {
        btn.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
    }
}
