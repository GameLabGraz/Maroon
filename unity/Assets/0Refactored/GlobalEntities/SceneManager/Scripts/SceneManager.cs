using UnityEngine;
using System.Collections.Generic;

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

        /// <summary>
        ///     Returns the name of the scene that is currently active.
        /// </summary>
        public string getCurrentSceneName()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }

        /// <summary>
        ///     Returns the name of the scene that is currently active, without the platform-specific extension (.vr or
        ///     .pc).
        /// </summary>
        public string getCurrentSceneNameWithoutPlatformExtension()
        {
            string currentSceneName = this.getCurrentSceneName();
            return currentSceneName.Substring(0, currentSceneName.LastIndexOf('.'));
        }

        /// <summary>
        ///     Returns true if the currently active scene is a virtual reality scene and has the .vr extension.
        ///     Returns false otherwise.
        /// </summary>
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

        /// <summary>
        ///     Searches all scene categories registered the SceneManager prefab and returns the category if the name
        ///     matches. Returns null if no category was found.
        /// </summary>
        /// <param name="categoryName">
        ///     The name of the category to be looked for.
        /// </param>
        /// <param name="sceneType">
        ///     Mixed allows any scene category type, others limit returned scenes to the given type.
        /// </param>
        public Maroon.SceneCategory getSceneCategoryByName(string categoryName, Maroon.SceneType sceneType =
                                                           Maroon.SceneType.Mixed)
        {
            // Check if category exists
            Maroon.SceneCategory categoryFound = null;
            for(int iCategories = 0; iCategories < this.sceneCategories.Length; iCategories++)
            {
                if(categoryName == this.sceneCategories[iCategories].Name)
                {
                    // Extract current category
                    Maroon.SceneCategory current_category = this.sceneCategories[iCategories];

                    // Select if all types allowed or if desired type eqal to current type
                    if((sceneType == Maroon.SceneType.Mixed) || (sceneType == current_category.SceneTypeInThisCategory))
                    {
                        categoryFound = current_category;
                        break;
                    }
                }
            }

            // Return category or null
            return categoryFound;
        }

        // -------------------------------------------------------------------------------------------------------------
        // Scenes

        /// <summary>
        ///     Aggregates all scenes registered to any category in the SceneManager prefab and returns them as an
        ///     Maroon.CustomSceneAsset list.
        /// </summary>
        /// <param name="sceneType">
        ///     Mixed allows any scene type, others limit returned scenes to the given type.
        /// </param>
        private List<Maroon.CustomSceneAsset> getScenesFromAllCategories(Maroon.SceneType sceneType =
                                                                         Maroon.SceneType.Mixed)
        {
            // Create list
            List<Maroon.CustomSceneAsset> scenesFromAllCategories = new List<Maroon.CustomSceneAsset>();

            // Aggregate all scenes from all categories
            for(int iCategories = 0; iCategories < this.sceneCategories.Length; iCategories++)
            {
                for(int iScenes = 0; iScenes < this.sceneCategories[iCategories].Scenes.Length; iScenes++)
                {
                    // Extract current scene
                    Maroon.CustomSceneAsset current_scene = this.sceneCategories[iCategories].Scenes[iScenes];

                    // Add if all types allowed or if desired type eqal to current type
                    if((sceneType == Maroon.SceneType.Mixed) || (sceneType == current_scene.SceneType))
                    {
                        scenesFromAllCategories.Add(current_scene);
                    }
                }
            }
        
            // Return result
            return scenesFromAllCategories;
        }

        /// <summary>
        ///     Aggregates all scenes registered to any category in the SceneManager prefab and returns their names as
        ///     a string array.
        /// </summary>
        /// <param name="sceneType">
        ///     Mixed allows any scene type, others limit returned scenes to the given type.
        /// </param>
        public string[] getSceneNamesFromAllCategories(Maroon.SceneType sceneType = Maroon.SceneType.Mixed)
        {
            // Get Maroon.CustomSceneAsset list containing all registered scenes
            List<Maroon.CustomSceneAsset> scenesFromAllCategories = this.getScenesFromAllCategories(sceneType);

            // Create array
            string[] sceneNamesFromAllCategories = new string[scenesFromAllCategories.Count];

            // Get scene names
            for(int iScenes = 0; iScenes < scenesFromAllCategories.Count; iScenes++)
            {
                sceneNamesFromAllCategories[iScenes] = scenesFromAllCategories[iScenes].SceneName;
            }

            // Return result
            return sceneNamesFromAllCategories;
        }

        /// <summary>
        ///     Aggregates all scenes registered to any category in the SceneManager prefab and returns their names as
        ///     a string array. The platform-specific extension .vr or .pc is removed from the name.
        /// </summary>
        /// <param name="sceneType">
        ///     Mixed allows any scene type, others limit returned scenes to the given type.
        /// </param>
        public string[] getSceneNamesWithoutPlatformExtensionFromAllCategories(Maroon.SceneType sceneType =
                                                                               Maroon.SceneType.Mixed)
        {
            // Get Maroon.CustomSceneAsset list containing all registered scenes
            List<Maroon.CustomSceneAsset> scenesFromAllCategories = this.getScenesFromAllCategories(sceneType);

            // Create array
            string[] sceneNamesFromAllCategories = new string[scenesFromAllCategories.Count];

            // Get scene names without platform-specific extensions
            for(int iScenes = 0; iScenes < scenesFromAllCategories.Count; iScenes++)
            {
                sceneNamesFromAllCategories[iScenes] = scenesFromAllCategories[iScenes].SceneNameWithoutPlatformExtension;
            }

            // Return result
            return sceneNamesFromAllCategories;
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Methods

        /// <summary>
        ///     Called by Unity. Initializes singleton instance and DontDestroyOnLoad (stays active on new scene load).
        ///     Checks if only VR scenes are in VR categories and only standard scenes are in standard categories.
        ///     Throws an exception if this is not the case. 
        /// </summary>
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

            // Check if only VR scenes are in VR categories and only standard scenes are in standard categories
            for(int iCategories = 0; iCategories < this.sceneCategories.Length; iCategories++)
            {
                // Extract current category
                Maroon.SceneCategory current_category = this.sceneCategories[iCategories];

                // Check if all scenes types match
                for(int iScenes = 0; iScenes < current_category.Scenes.Length; iScenes++)
                {
                    if(current_category.SceneTypeInThisCategory != current_category.Scenes[iScenes].SceneType)
                    {
                        throw new System.Exception("Category >" + current_category.Name +
                                                   "< contains a scene with mismatched type.");
                    }                    
                }
            }
        }

        /// <summary>
        ///     Loades a scene based on a scene name, a scene path or a Maroon.CustomSceneAsset. The
        ///     Maroon.CustomSceneAsset is equivalent to the scene path. The scene must be registered in the
        ///     SceneManager to be able to load it with this method.
        ///
        ///     TODO: Pipe all scene changes through this method so that this method can notify the NetworkManager
        ///     consistently.
        /// </summary>
        /// <param name="sceneNameOrPath">
        ///     The full path to the scene or the name of the scene. Also accepts a Maroon.CustomSceneAsset.
        /// </param>
        /// <param name="showLoadingScreen">
        ///     Enables a loading screen while loading the given scene. TODO: Not implemented. Implement this.
        /// </param>
        public bool LoadSceneIfInAnyCategory(string sceneNameOrPath, bool showLoadingScreen = false)
        {
            // Convert full path to scene name
            string sceneName = System.IO.Path.GetFileName(sceneNameOrPath);

            // Get valid scenes
            string[] validSceneNames = this.getSceneNamesFromAllCategories();

            // If valid, load scene and return true
            if(System.Array.Exists(validSceneNames, element => element == sceneName))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                return true;
            }
            
            // Else return false
            return false;
        }
    }

    /// <summary>
    ///     Describes types of scenes used in Maroon. Standard is used for PC, Mac and WebGL. VR is used for Virtual
    ///     Reality scenes. Mixed is used for scene categories or functions that work with both types.
    /// </summary>
    public enum SceneType
    {
        Standard,
        VR,
        Mixed
    }
}
