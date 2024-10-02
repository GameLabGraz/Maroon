using UnityEngine;
using System.Collections.Generic;

namespace Maroon.GlobalEntities
{
    /// <summary>
    ///     Handles tasks related to scenes in Maroon, including finding scene categories, scene assets and loading
    ///     scenes.
    /// </summary>
    public class SceneManager : MonoBehaviour, GlobalEntity
    {
        private static SceneManager _instance = null;

        /// TODO
        [SerializeField] private Maroon.CustomSceneAsset _sceneMainMenuPC = null;

        [SerializeField] private Maroon.CustomSceneAsset _sceneMainMenuVR = null;

        [SerializeField] private Maroon.SceneCategory[] _sceneCategories = null;

        public bool ShowDebugMessages = true;
        
        private Maroon.SceneCategory _activeSceneCategory;

        private Stack<Maroon.CustomSceneAsset> _sceneHistory = new Stack<Maroon.CustomSceneAsset>();

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Properties, Getters and Setters

        // -------------------------------------------------------------------------------------------------------------
        // Singleton

        /// <summary>
        ///     The SceneManager instance
        /// </summary>
        public static SceneManager Instance => SceneManager._instance;
        MonoBehaviour GlobalEntity.Instance => Instance;

        // -------------------------------------------------------------------------------------------------------------
        // Categories

        /// <summary>
        ///     The SceneCategory that is currently active.
        ///     For example if the player is in the physics lab/category, the physics category should be set to active.
        /// </summary>
        public SceneCategory ActiveSceneCategory
        {
            get => this._activeSceneCategory;

            set
            {
                // Only set if it exists in categories array
                if(System.Array.Exists(this._sceneCategories, element => element == value))
                {
                    this._activeSceneCategory = value;
                }
            }
        }

        public void SetActiveSceneCategory(string categoryName)
        {
            var cat = getSceneCategoryByName(categoryName);
            Debug.Assert(cat != null, "Scene Category '" + categoryName + "' not found.");
            ActiveSceneCategory = cat;
        }

        // -------------------------------------------------------------------------------------------------------------
        // Active Scene

        /// <summary>
        ///     The name of the scene that is currently active.
        /// </summary>
        public string ActiveSceneName => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        /// <summary>
        ///     The name of the scene that is currently active, without the platform-specific extension (.vr or .pc).
        /// </summary>
        public string ActiveSceneNameWithoutPlatformExtension => this.ActiveSceneName.Substring(0, this.ActiveSceneName.LastIndexOf('.'));

        /// <summary>
        ///     Maroon.PlatformManager has the properties CurrentPlatformIsVR and SceneTypeBasedOnPlatform. You might
        ///     want to use that to get info about the current build platform and VR state. This is only valid for the
        ///     active scene. This method returns true if the currently active scene is a virtual reality scene and has
        ///     the .vr extension. Returns false otherwise.
        /// </summary>
        public bool activeSceneIsVR()
        {
            return this.ActiveSceneName.Contains(".vr");
        }

        // -------------------------------------------------------------------------------------------------------------
        // Categories

        /// <summary>
        ///     Returns a list of all scene categories registered in the SceneManager prefab and returns the category
        ///     if the name matches. Returns empty list if no category was found.
        /// </summary>
        /// <param name="sceneType">
        ///     Mixed allows any scene category type, others limit returned categories to the given type.
        /// </param>
        /// <param name="includeHidden">
        ///     Hidden categories are excluded by default. Set to true to include hidden categories.
        /// </param>
        public List<Maroon.SceneCategory> getSceneCategories(SceneType sceneType = SceneType.Mixed,
                                                             bool includeHidden = false)
        {
            // Create list
            List<Maroon.SceneCategory> categories = new List<Maroon.SceneCategory>();

            // Add categories
            for(int iCategories = 0; iCategories < this._sceneCategories.Length; iCategories++)
            {
                // Extract current category
                Maroon.SceneCategory current_category = this._sceneCategories[iCategories];

                // Skip hidden categories based on setting
                if((!includeHidden) && (current_category.HiddenCategory))
                {
                    continue;
                }

                // Select if all types allowed or if desired type eqal to current type
                if((sceneType == SceneType.Mixed) || (sceneType == current_category.SceneTypeInThisCategory))
                {
                    categories.Add(current_category);
                }
            }

            // Return categories
            return categories;
        }

        /// <summary>
        ///     Searches all scene categories registered in the SceneManager prefab and returns the category if the
        ///     name matches. Returns null if no category was found.
        /// </summary>
        /// <param name="categoryName">
        ///     The name of the category to be looked for.
        /// </param>
        /// <param name="sceneType">
        ///     Mixed allows any scene category type, others limit returned categories to the given type.
        /// </param>
        public Maroon.SceneCategory getSceneCategoryByName(string categoryName, SceneType sceneType =
                                                           SceneType.Mixed)
        {
            // Check if category exists
            Maroon.SceneCategory categoryFound = null;
            for(int iCategories = 0; iCategories < this._sceneCategories.Length; iCategories++)
            {
                if(categoryName == this._sceneCategories[iCategories].Name)
                {
                    // Extract current category
                    SceneCategory current_category = this._sceneCategories[iCategories];

                    // Select if all types allowed or if desired type eqal to current type
                    if((sceneType == SceneType.Mixed) || (sceneType == current_category.SceneTypeInThisCategory))
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
        private List<Maroon.CustomSceneAsset> getScenesFromAllCategories(SceneType sceneType =
                                                                         SceneType.Mixed)
        {
            // Create list
            List<Maroon.CustomSceneAsset> scenesFromAllCategories = new List<Maroon.CustomSceneAsset>();

            // Aggregate all scenes from all categories
            for(int iCategories = 0; iCategories < this._sceneCategories.Length; iCategories++)
            {
                for(int iScenes = 0; iScenes < this._sceneCategories[iCategories].Scenes.Length; iScenes++)
                {
                    // Extract current scene
                    Maroon.CustomSceneAsset current_scene = this._sceneCategories[iCategories].Scenes[iScenes];

                    // Add if all types allowed or if desired type eqal to current type
                    if((sceneType == SceneType.Mixed) || (sceneType == current_scene.SceneType))
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
        public string[] getSceneNamesFromAllCategories(SceneType sceneType = SceneType.Mixed)
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
        public string[] getSceneNamesWithoutPlatformExtensionFromAllCategories(SceneType sceneType =
                                                                               SceneType.Mixed)
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

        // -------------------------------------------------------------------------------------------------------------
        // Initialization

        /// <summary>
        ///     Called by Unity. Initializes singleton instance and DontDestroyOnLoad (stays active on new scene load).
        ///     Checks if only VR scenes are in VR categories and only standard scenes are in standard categories.
        ///     Throws an exception if this is not the case. 
        /// </summary>
        private void Awake()
        {
            // Singleton
            if(SceneManager._instance == null)
            {
                SceneManager._instance = this;
            }
            else if(SceneManager._instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            // Keep alive
            DontDestroyOnLoad(this.gameObject);

            // Check if only VR scenes are in VR categories and only standard scenes are in standard categories
            for(int iCategories = 0; iCategories < this._sceneCategories.Length; iCategories++)
            {
                // Extract current category
                Maroon.SceneCategory current_category = this._sceneCategories[iCategories];

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

        // -------------------------------------------------------------------------------------------------------------
        // Scenes

        /// <summary>
        ///     Returns a Maroon.CustomSceneAsset based on a scene name or a scene path. The scene must be registered
        ///     in one of the categories in the SceneManager to be able to convert it with this method. Returns null
        ///     if no Maroon.CustomSceneAsset is found
        /// </summary>
        /// <param name="scene">
        ///     The full path to the scene or the name of the scene.
        /// </param>
        public Maroon.CustomSceneAsset GetSceneAssetBySceneName(string sceneNameOrPath)
        {
            // Convert full path to scene name
            string sceneName = System.IO.Path.GetFileName(sceneNameOrPath);

            // Find and return Maroon.CustomSceneAsset
            Maroon.CustomSceneAsset sceneAsset;
            sceneAsset = this.getScenesFromAllCategories().Find(element => sceneName == element.SceneName);
            return sceneAsset;
        }

        // -------------------------------------------------------------------------------------------------------------
        // Scene Navigation

        /// <summary>
        ///     Loades a scene based on a Maroon.CustomSceneAsset. The scene must be registered in one of the
        ///     categories in the SceneManager to be able to load it with this method. Always use this method for 
        ///     loading a new scene, it updates the scene history and checks if the platform is correct.
        /// </summary>
        /// <param name="scene">
        ///     A Maroon.CustomSceneAsset to be loaded.
        /// </param>
        /// <param name="showLoadingScreen">
        ///     Enables a loading screen while loading the scene. TODO: Not implemented. Implement this.
        /// </param>
        public bool LoadSceneRequest(Maroon.CustomSceneAsset scene, bool showLoadingScreen = false)
        {
            // Check if scene is valid
            if(!LoadSceneValidate(scene))
            {
                return false;
            }

            LoadSceneExecute(scene, showLoadingScreen);

            return true;
        }

        /// <summary>
        ///     Loades the Main Menu scene according to the current platform.
        /// </summary>
        /// <param name="showLoadingScreen">
        ///     Enables a loading screen while loading the scene.
        /// </param>
        public void LoadMainMenu(bool showLoadingScreen = false)
        {
            // Return VR main menu for VR platform
            if(PlatformManager.Instance.CurrentPlatformIsVR)
            {
                this.LoadSceneRequest(this._sceneMainMenuVR);
            }
            else
            {
                // Return PC main menu
                this.LoadSceneRequest(this._sceneMainMenuPC);
            }
        }

        /// <summary>
        ///     The name of the scene that was loaded before the currently active scene. If no previous scene
        ///     is available, the Main Menu scene according to the current platform.
        /// </summary>
        public void LoadPreviousScene()
        {
            // If there is no previous scene available, load main menu
            if(this._sceneHistory.Count < 2 && !PlatformManager.Instance.CurrentPlatformIsVR)
            {
                if(ShowDebugMessages)
                    Debug.Log("[SceneManager] sceneHistory count < 2 and current Platform is not VR: Loading Main Menu");
                this.LoadMainMenu();
                return;
            }

            // If previous scene available, remove current scene and load previous scene
            Debug.Assert(_sceneHistory.Count > 0);
            var currentScene = this._sceneHistory.Pop();
            if (ShowDebugMessages)
            {
                Debug.Log("[SceneManager] Removing current scene '" + currentScene.SceneName + "' from stack");
                if(_sceneHistory.Count != 0)
                    Debug.Log("[SceneManager] Loading scene '" + _sceneHistory.Peek().SceneName + "' from top of the stack");
                else 
                    Debug.Log("[SceneManager] Stack is empty -> loading Main Menu");
            }

            if (_sceneHistory.Count > 0)
                this.LoadSceneRequest(this._sceneHistory.Peek());
            else
                this.LoadMainMenu();
        }


        // -------------------------------------------------------------------------------------------------------------
        // Scene Navigation: Helper Methods

        /// <summary>
        ///     Validates if a scene exists and checks if it can be loaded based on current scene and platform.
        /// </summary>
        /// <param name="scene">
        ///     A Maroon.CustomSceneAsset to be loaded.
        /// </param>
        public bool LoadSceneValidate(Maroon.CustomSceneAsset scene)
        {
            // If scene to be loaded doesn't exist in one of the categories
            if(!System.Array.Exists(this.getScenesFromAllCategories().ToArray(), element => element.SceneName == scene.SceneName))
            {
                // TODO: Convert to warning after fixing warnings
                Debug.Log("WARNING: Tried to load scene that does not exist in categories."); 
                return false;
            }

            // If scene to be loaded has wrong platform
            if(scene.IsVirtualRealityScene != PlatformManager.Instance.CurrentPlatformIsVR)
            {
                // TODO: Convert to warning after fixing warnings
                Debug.Log("WARNING: Tried to load scene that does not match platform type."); 
                return false;
            }

            // Scene exists and it is ok to load it
            return true;
        }

        // -------------------------------------------------------------------------------------------------------------
        // Scene Navigation: Dangerous Methods

        // DO NOT USE THIS FUNCTION, use LoadSceneRequest instead, unless you have a good reason to do so and know what your are
        public bool LoadSceneExecute(Maroon.CustomSceneAsset scene, bool showLoadingScreen = false)
        {
            // Check if scene is valid
            if(!LoadSceneValidate(scene))
            {
                return false;
            }
            
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
            AddToSceneHistory(scene);

            // Add scene change to history
            return true;
        }


        // DO NOT USE THIS FUNCTION, use LoadSceneRequest instead, unless you have a good reason to do so and know what your are
        public bool LoadSceneLocalOnlyExecuteForce(Maroon.CustomSceneAsset scene, bool showLoadingScreen = false)
        {
            // Check if scene is valid
            if(!LoadSceneValidate(scene))
            {
                if (ShowDebugMessages)
                {
                    Debug.Log("[SceneManager]: Scene " + scene.SceneName + "could not be loaded or validated.");
                }
                return false;
            }    
        
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
            AddToSceneHistory(scene);
            return true;
        }

        // Called by SceneManager to update scene history
        public void AddToSceneHistory(Maroon.CustomSceneAsset scene)
        {
            if(_sceneHistory.Count == 0 || scene.SceneName != _sceneHistory.Peek().SceneName)
            {
                if(ShowDebugMessages)
                    Debug.Log("[SceneManager] Push Scene to history: " + scene.SceneName);
                this._sceneHistory.Push(scene);
            }
        }

        // Called when the game should be closed
        public void ExitApplication()
        {
            if(ShowDebugMessages)
                Debug.Log("[SceneManager] Exit Application");
            Application.Quit();
        }
        
    }
}
