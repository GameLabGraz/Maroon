using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using GEAR.Localization;
using NUnit.Framework;
using Tests.Utilities;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Tests.Utilities.Utilities;

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

        private const string ExperimentPrefabName = "ExperimentSetting.pc";
        private string[] _objectNamesFromExperimentPrefab;
        private List<GameObject> _gameObjectsFromExperimentPrefab;

        public PcSceneValidationTests(string experimentName, string scenePath)
        {
            _experimentName = experimentName;
            _scenePath = scenePath;
        }

        // Before running any tests, query ExperimentSetting prefab and load the scene if it's not yet loaded
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Get "mandatory" object names from ExperimentSetting prefab
            var experimentSettingPrefab = GetPrefabByName(ExperimentPrefabName);
            _gameObjectsFromExperimentPrefab = new List<GameObject>();
            AddDescendantsUntilDepth(experimentSettingPrefab.transform, _gameObjectsFromExperimentPrefab, 3);
            
            _objectNamesFromExperimentPrefab = _gameObjectsFromExperimentPrefab
                .Where(child => !child.name.Contains("="))
                .Select(x => x.name).ToArray();

            // Load scene if necessary
            var scene = SceneManager.GetSceneAt(0);
            if (SceneManager.sceneCount > 1 || scene.path != _scenePath)
            {
                EditorSceneManager.OpenScene(_scenePath, OpenSceneMode.Single);
            }
        }
        
        [SkipTestFor("StateMachine")]
        [Test, Description("Must have a GameObject with enabled <Camera> component tagged as 'MainCamera'")]
        public void SceneHasMainCamera()
        {
            const string objectNameUnderTest = "MainCamera";

            ToSkipOrNotToSkip(objectNameUnderTest);

            GameObject prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            Camera prefabCameraComponent = prefab.GetComponent<Camera>();

            // Check GameObject exists and is active
            var cameraGameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(cameraGameObject);

            // Check Camera component is attached
            var cameraComponent = GetAndValidateComponentFromGameObject<Camera>(cameraGameObject);
            
            // Check camera component's properties
            Assert.AreEqual(prefabCameraComponent.orthographic, cameraComponent.orthographic,
                $"Wrong projection type for '{cameraComponent.GetType().Name}' component of '{cameraGameObject.name}'");
            
            Assert.AreEqual(prefabCameraComponent.cullingMask, cameraComponent.cullingMask,
                $"Wrong cullingMask for '{cameraComponent.GetType().Name}' component of '{cameraGameObject.name}'");
            
            Assert.AreEqual(prefabCameraComponent.fieldOfView, cameraComponent.fieldOfView, 5f,
                $"Wrong fieldOfView for '{cameraComponent.GetType().Name}' component of '{cameraGameObject.name}'");
        }
        
        [Test, Description("Must have a GameObject named 'UICamera' with configured <Camera> component")]
        public void SceneHasUICamera()
        {
            const string objectUnderTest = "UICamera";
            // Check GameObject exists and is active
            var cameraGameObject = FindObjectByName(objectUnderTest);
            AssertGameObjectIsActive(cameraGameObject);
            
            // Check Camera component is attached
            var cameraComponent = GetAndValidateComponentFromGameObject<Camera>(cameraGameObject);
            
            // Check UI camera's properties
            Assert.AreEqual(LayerMask.GetMask("UI"), cameraComponent.cullingMask,
                $"Wrong culling mask for '{cameraComponent.name}' component of '{cameraGameObject.name}'");

            Assert.True(cameraComponent.orthographic,
                $"Wrong projection type for '{cameraComponent.name}' component of '{cameraGameObject.name}'");
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
            var prefab = GetPrefabByName("UI");
            var prefabCanvas = GetComponentFromPrefab<Canvas>(prefab);
            var prefabCanvasScaler = GetComponentFromPrefab<CanvasScaler>(prefab);

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
        
        /**
         * Checks whether a test case should be skipped if either is true:
         * 1.) The test case is decorated with the custom attribute [SkipTestFor("SomeExperimentName")] and the experiment matches
         * 2.) The objectNameUnderTest is not part of the experiment prefab
         */
        private void ToSkipOrNotToSkip(string objectNameUnderTest, [CallerMemberName] string callingMethodName = null)
        {
            // Get instance of the attribute
            var myCustomAttribute = SkipTestFor.GetAttributeCustom<PcSceneValidationTests>(callingMethodName);
            if (myCustomAttribute != null)
            {
                if (myCustomAttribute.ExperimentNames.Any(x => _experimentName.ToUpper().Contains(x.ToUpper())))
                    Assert.Ignore($"{_experimentName} scene intentionally has no '{objectNameUnderTest}'");
            }

            if (!_objectNamesFromExperimentPrefab.Contains(objectNameUnderTest))
                Assert.Ignore($"{ExperimentPrefabName} contains no {objectNameUnderTest} - skipping test!");
        }
    }
}
