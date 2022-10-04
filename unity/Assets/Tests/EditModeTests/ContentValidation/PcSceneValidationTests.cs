using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GEAR.Localization;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tests.EditModeTests.ContentValidation
{
    public class PcScenesProvider : ScenesProvider
    {
        protected override Regex experimentNameRegex => new Regex(@"\w+\.pc");
        protected override string fileEnding => ".pc.unity";
    }
    
    // Runs OneTimeSetUp and all Tests once for each provided experiment scene path
    [TestFixtureSource(typeof(PcScenesProvider))]
    public class PcSceneValidationTests
    {
        private readonly string _experimentName;
        private readonly string _scenePath;
        private Scene _scene;

        public PcSceneValidationTests(string experimentName, string scenePath)
        {
            _experimentName = experimentName;
            _scenePath = scenePath;
        }
        
        // Load scene if not yet loaded before running the scene's tests
        [OneTimeSetUp]
        public void LoadScene()
        {
            _scene = SceneManager.GetSceneAt(0);
            if (SceneManager.sceneCount > 1 || _scene.path != _scenePath)
                _scene = EditorSceneManager.OpenScene(_scenePath, OpenSceneMode.Single);
        }
        
        [Test, Description("Must have a GameObject with enabled <Camera> component tagged as 'MainCamera'")]
        public void SceneHasMainCamera()
        {
            // Check GameObject exists
            var cameraGameObject = GameObject.FindWithTag("MainCamera");
            Assert.NotNull(cameraGameObject, "No GameObject tagged 'MainCamera' found");

            // Check Camera component and its settings
            var cameraComponent = cameraGameObject.GetComponent<Camera>();
            Assert.NotNull(cameraComponent, "No 'Camera' component in GameObject 'MainCamera'");
            
            Assert.True(cameraComponent.enabled, 
                "The 'Camera' component of 'MainCamera' is disabled");
        }
        
        [Test, Description("Must have a GameObject named 'UICamera' with configured <Camera> component")]
        public void SceneHasUICamera()
        {
            // Check GameObject exists
            var cameraGameObject = GameObject.Find("UICamera");
            Assert.NotNull(cameraGameObject, "No 'UICamera' GameObject found");

            // Check Camera component and its settings
            var cameraComponent = cameraGameObject.GetComponent<Camera>();
            Assert.NotNull(cameraComponent, "No 'Camera' component in GameObject 'UICamera'");
            
            Assert.True(cameraComponent.enabled,
                "The 'Camera' component of 'UICamera' is disabled");
            
            Assert.AreEqual(LayerMask.GetMask("UI"), cameraComponent.cullingMask,
                "Wrong culling mask for 'Camera' component of 'UICamera'");

            Assert.True(cameraComponent.orthographic,
                "Wrong projection type for 'Camera' component of 'UICamera'");
        }

        [Test, Description("Must have a GameObject named 'UI' with configured 'Canvas' component")]
        public void SceneHasUserInterface()
        {
            /* TODO food for thought
             * Is direct comparison against prefab a good idea? Could throw unexpected errors when a prefab is changed
             * in an impactful way.
             * On the other hand, if we check for hardcoded values and a prefab is changed, a test checking against it
             * will fail either way.
             * What could we do to mitigate broken scene tests? Run prefab tests before scene tests and stop test run!
             * This should reduce any possible confusion with broken prefab checks in scene tests.
             *
             * What about child objects like EventSystem?
             * Is it possible to compare Prefab's and GameObject's child?
             * Is there any other way to compare a GameObject with its Prefab? I haven't found a way yet.
             * Random idea, but couldn't get GUID of GameObject to compare with either.
             * 
             * Anyway, stopped here with a mix of prefab and hardcoded comparisons.
             */
            
            // Get prefab data
            var prefab = Utilities.GetPrefabByName("UI");
            var prefabCanvas = Utilities.GetComponentFromPrefab<Canvas>(prefab);
            var prefabCanvasScaler = Utilities.GetComponentFromPrefab<CanvasScaler>(prefab);

            // Check GameObject exists
            var uiGameObject = GameObject.Find("UI");
            Assert.NotNull(uiGameObject, "No 'UI' GameObject found");
            
            // Check GameObject is set to UI layer
            Assert.AreEqual(prefab.layer, uiGameObject.layer);
            //Assert.AreEqual(LayerMask.NameToLayer("UI"), uiGameObject.layer);
            
            // Check Canvas component and its settings
            var canvasComponent = uiGameObject.GetComponent<Canvas>();
            Assert.NotNull(canvasComponent, "No 'Canvas' component in GameObject 'UI'");
            Assert.AreEqual(prefabCanvas.renderMode,canvasComponent.renderMode);
            // Assert.AreEqual(RenderMode.ScreenSpaceCamera, canvasComponent.renderMode);

            // Check Canvas Scaler component and its settings
            var canvasScalerComponent = uiGameObject.GetComponent<CanvasScaler>();
            Assert.NotNull(canvasScalerComponent, "No 'Canvas Scaler' component in GameObject 'UI'");
            Assert.AreEqual(prefabCanvasScaler.uiScaleMode, canvasScalerComponent.uiScaleMode);
            //Assert.AreEqual(CanvasScaler.ScaleMode.ScaleWithScreenSize, canvasScalerComponent.uiScaleMode);

            // Check EventSystem exists
            var eventSystem = GameObject.Find("EventSystem");
            Assert.NotNull(eventSystem, "No 'EventSystem' GameObject found");
            Assert.AreEqual(uiGameObject.transform, eventSystem.transform.parent, "EventSystem is not a child GameObject of 'UI'");
        }
        
        [Test, Description("Must use the 'ExperimentRoom' Prefab")]
        public void SceneHasExperimentRoom()
        {
            // Check GameObject exists
            var experimentRoomGameObject = GameObject.Find("ExperimentRoom");
            Assert.NotNull(experimentRoomGameObject, "No 'ExperimentRoom' GameObject found");
        }
        
        [Test, Description("Must include the 'SimulationController' Prefab")]
        public void SceneHasSimulationController()
        {
            // List of scenes that skip the test
            var scenesToSkip = new List<string> { "Whiteboard" };
            
            // Skip listed scene(s)
            if (scenesToSkip.Any(x => _experimentName.ToUpper().Contains(x.ToUpper())))
                Assert.Ignore($"{_experimentName} scene has intentionally no SimulationController");

            // Check GameObject exists
            var simulationControllerGameObject = GameObject.Find("SimulationController");
            Assert.NotNull(simulationControllerGameObject, "No 'SimulationController' GameObject found");
        }
        
        [Test, Description("Must include the 'LanguageManager' Prefab")]
        public void SceneHasLanguageManager()
        {
            // Check GameObject exists
            var languageManagerGameObject = GameObject.Find("LanguageManager");
            Assert.NotNull(languageManagerGameObject, "No 'LanguageManager' GameObject found");
            
            // The package provides an assembly definition, enabling us to directly check for the script component
            var languageManagerScriptComponent = languageManagerGameObject.GetComponent<LanguageManager>();
            Assert.NotNull(languageManagerScriptComponent, "No 'LanguageManager' component in GameObject 'LanguageManager'");
        }
        
        [Test, Description("Must include the 'GlobalEntities' Prefab")]
        public void SceneHasGlobalEntities()
        {
            var globalEntitiesGameObject = GameObject.Find("GlobalEntities");
            Assert.NotNull(globalEntitiesGameObject, "No 'GlobalEntities' GameObject found");
        }
        
        [Test, Description("Must include the 'prePauseMenu' Prefab")]
        public void SceneHasPauseMenu()
        {
            var pauseMenuGameObject = GameObject.Find("prePauseMenu") ?? GameObject.Find("PauseMenu");
            Assert.NotNull(pauseMenuGameObject, "No 'prePauseMenu' or 'PauseMenu' GameObject found");
        }
    }
}
