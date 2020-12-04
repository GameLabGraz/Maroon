using UnityEngine;

namespace Maroon
{
    class SceneManager : MonoBehaviour
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Fields

        [SerializeField] private bool webGLEnableSceneLoadingViaURLParameter = false;
        [SerializeField] private bool webGLForceSceneFromURLParameter = false; // TODO: Implement
        [SerializeField] private string webGLSceneURLParameterName = "LoadScene";

        [SerializeField] private Maroon.SceneCategory[] sceneCategories;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Getters

        public Maroon.CustomSceneAsset[] getScenesFromAllCategories()
        {
            int numberOfScenes = 0;
            for(int iCategories = 0; iCategories < this.sceneCategories.Length; iCategories++)
            {
                numberOfScenes += this.sceneCategories[iCategories].Scenes.Length;
            }

            Maroon.CustomSceneAsset[] scenesFromAllCategories = new Maroon.CustomSceneAsset[numberOfScenes];
            for(int iCategories = 0; iCategories < this.sceneCategories.Length; iCategories++)
            {
                for(int iScenes = 0; iScenes < this.sceneCategories[iCategories].Scenes.Length; iScenes++)
                {
                    numberOfScenes--;
                    scenesFromAllCategories[numberOfScenes] = this.sceneCategories[iCategories].Scenes[iScenes];
                }
            }
          
            return scenesFromAllCategories;
        }

        public string[] getSceneNamesFromAllCategories()
        {
            Maroon.CustomSceneAsset[] scenesFromAllCategories = this.getScenesFromAllCategories();
            string[] sceneNamesFromAllCategories = new string[scenesFromAllCategories.Length];

            for(int iScenes = 0; iScenes < scenesFromAllCategories.Length; iScenes++)
            {
                sceneNamesFromAllCategories[iScenes] = scenesFromAllCategories[iScenes].SceneName;
            }

            return sceneNamesFromAllCategories;
        }

        public string[] getSceneNamesWithoutPlatformExtensionFromAllCategories()
        {
            Maroon.CustomSceneAsset[] scenesFromAllCategories = this.getScenesFromAllCategories();
            string[] sceneNamesFromAllCategories = new string[scenesFromAllCategories.Length];

            for(int iScenes = 0; iScenes < scenesFromAllCategories.Length; iScenes++)
            {
                sceneNamesFromAllCategories[iScenes] = scenesFromAllCategories[iScenes].SceneNameWithoutPlatformExtension;
            }

            return sceneNamesFromAllCategories;
        }

        public Maroon.SceneCategory getSceneCategoryByName(string categoryName)
        {
            // Check if category exists
            Maroon.SceneCategory categoryFound = null;
            for(int iCategories = 0; iCategories < this.sceneCategories.Length; iCategories++)
            {
                if(categoryName == this.sceneCategories[iCategories].Name)
                {
                    categoryFound = this.sceneCategories[iCategories];
                    break;
                }
            }

            // Return Category
            if(categoryFound != null)
            {
                return categoryFound;
            }
            return null;
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Methods

        // 
        public void Start()
        {
            if(this.webGLEnableSceneLoadingViaURLParameter)
            {
                this.loadSceneViaURLParameter();
            }
        }

        private void loadSceneViaURLParameter()
        {
            if(Maroon.TargetPlatformDetector.targetPlatform == "webgl")
            {
                string parameter = Maroon.WebGLUrlParameterReader.GetUrlParameter(this.webGLSceneURLParameterName);
                this.LoadSceneIfInAnyCategory(parameter + ".pc");
            }
        }

        public bool LoadSceneIfInAnyCategory(string sceneName, bool showLoadingScreen = false)
        {
            string[] validSceneNames = this.getSceneNamesFromAllCategories();

            if(System.Array.Exists(validSceneNames, element => element == sceneName))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                return true;
            }
            
            return false;
        }
    }
}
