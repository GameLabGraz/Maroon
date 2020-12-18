using UnityEngine;

namespace Maroon
{
    public class SceneManager : MonoBehaviour
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Fields

        private static SceneManager instance = null;
        [SerializeField] private Maroon.SceneCategory[] sceneCategories;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Getters and Properties

        public static SceneManager Instance
        {
            get { return SceneManager.instance; }
        }

        // -------------------------------------------------------------------------------------------------------------
        // Current Scene

        public string getCurrentSceneName()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }

        public string getCurrentSceneNameWithoutPlatformExtension()
        {
            string currentSceneName = this.getCurrentSceneName();
            return currentSceneName.Substring(0, currentSceneName.LastIndexOf('.'));
        }

        public bool currentSceneIsVR()
        {
            if(this.getCurrentSceneName().Contains(".vr"))
            {
                return true;
            }
            return false;
        }

        // -------------------------------------------------------------------------------------------------------------
        // Categories

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

        public Maroon.SceneCategory getVRSceneCategoryByName(string categoryName)
        {
            return null;
        }

        // -------------------------------------------------------------------------------------------------------------
        // Scenes
        private Maroon.CustomSceneAsset[] getScenesFromAllCategories()
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

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Methods

        private void Awake()
        {
            // Singleton
            if(SceneManager.instance == null)
            {
                SceneManager.instance = this;
            }
            else if(SceneManager.instance != this)
            {
                Destroy(this.gameObject);
            }

            // Keep alive
            DontDestroyOnLoad(this.gameObject);

            // Sanity Check VR vs Standard scenes in categories
            // TODO
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

    enum SceneType
    {
        Standard,
        VR
    }
}
