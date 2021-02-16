using GEAR.Localization.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrMenuColumnLaboratorySelectionDetail : MonoBehaviour
{

    // Scenes
    [SerializeField] private Maroon.CustomSceneAsset targetLabScenePC;

    [SerializeField] private Maroon.CustomSceneAsset targetLabSceneVR;

    // Title
    [SerializeField] private GameObject Title;

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

        // Update Title
        // TODO: Make this work with localization
        Title.transform.GetComponent<LocalizedTMP>().Key = Maroon.SceneManager.Instance.ActiveSceneCategory.Name;

        // Link go button action
        this.ButtonGo.GetComponent<Button>().onClick.AddListener(() => this.OnClickGo());

        // Get experiment scenes based on current category
        Maroon.CustomSceneAsset[] scenes = Maroon.SceneManager.Instance.ActiveSceneCategory.Scenes;
        ButtonGo.transform.Find("ContentContainer").transform.Find("Text (TMP)").GetComponent<LocalizedTMP>().Key =
            "Go to " + Maroon.SceneManager.Instance.ActiveSceneCategory.Name + " Lab";

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
                Maroon.SceneManager.Instance.LoadSceneRequest(current_scene));
        }
    }

    private void OnClickGo()
    {
        if(Maroon.PlatformManager.Instance.CurrentPlatformIsVR)
        {
            Maroon.SceneManager.Instance.LoadSceneRequest(this.targetLabSceneVR);
        }

        else
        {
            Maroon.SceneManager.Instance.LoadSceneRequest(this.targetLabScenePC);
        }
    }
}
