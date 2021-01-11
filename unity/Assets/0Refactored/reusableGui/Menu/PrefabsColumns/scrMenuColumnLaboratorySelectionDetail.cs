using GEAR.Localization.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrMenuColumnLaboratorySelectionDetail : MonoBehaviour
{

    // Scenes
    [SerializeField] private Maroon.CustomSceneAsset targetLabScenePC;

    [SerializeField] private Maroon.CustomSceneAsset targetLabSceneVR;

    // Buttons
    [SerializeField] private GameObject ButtonGo;

    [SerializeField] private GameObject ExperimentButtonsContainer;

    [SerializeField] private GameObject ExperimentButtonPrefab;

    private void Start()
    {
        // Do not init anything if no category is selected, because no lab can be built and no experiments will show up
        if(Maroon.SceneManager.Instance.ActiveSceneCategory == null)
        {
            return;
        }

        // Link go button action
        this.ButtonGo.GetComponent<Button>().onClick.AddListener(() => this.OnClickGo());

        // Get experiment scenes based on current category
        Maroon.CustomSceneAsset[] scenes = Maroon.SceneManager.Instance.ActiveSceneCategory.Scenes;

        // Create buttons based on category experiments
        for(int iScenes = 0; iScenes < scenes.Length; iScenes++)
        {
            // Extract category
            Maroon.CustomSceneAsset current_scene = scenes[iScenes];

            // Create new button, add to panel and scale
            GameObject newButton = Instantiate(this.ExperimentButtonPrefab, Vector3.zero, Quaternion.identity,
                this.ExperimentButtonsContainer.transform) as GameObject;
            newButton.transform.localScale = Vector3.one;

            // Set text
            // TODO: Make this work with localization
            Transform text = newButton.transform.Find("ContentContainer").transform.Find("Text (TMP)");
            text.GetComponent<LocalizedTMP>().Key = current_scene.SceneNameWithoutPlatformExtension;

            // Link function
            newButton.GetComponent<Button>().onClick.AddListener(() =>
                Maroon.SceneManager.Instance.LoadSceneIfInAnyCategory(current_scene));
        }
    }

    private void OnClickGo()
    {
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
