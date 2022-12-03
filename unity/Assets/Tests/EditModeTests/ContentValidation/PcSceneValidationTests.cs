using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NUnit.Framework;
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

        private const string PcExperimentPrefabName = "ExperimentSetting.pc";
        
        private List<GameObject> _gameObjectsFromExperimentPrefab;
        private string[] _objectNamesFromExperimentPrefab;

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
            var experimentSettingPrefab = GetPrefabByName(PcExperimentPrefabName);
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
        
        [SkipTestForScenesWithReason("StateMachine", "scene has a different camera setup")]
        [Test, Description("Must have a GameObject named 'MainCamera' with a configured <Camera> component")]
        public void SceneHasMainCamera()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "MainCamera";
            SkipCheck(objectNameUnderTest);

            // Get prefab and component
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            var prefabCameraComponent = GetComponentFromGameObject<Camera>(prefab);
            var expectedTag = prefab.tag;

            // Check GameObject exists, is active and tagged
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            Assert.AreEqual(expectedTag, gameObjectUnderTest.tag,
                $"GameObject '{gameObjectUnderTest.name}' is missing its tag '{expectedTag}'");

            // Check Camera component is attached
            var cameraComponent = GetEnabledComponentFromGameObject<Camera>(gameObjectUnderTest);
            
            // Check camera component's properties
            Assert.AreEqual(prefabCameraComponent.orthographic, cameraComponent.orthographic,
                $"Wrong projection type for <{cameraComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
            
            Assert.AreEqual(prefabCameraComponent.cullingMask, cameraComponent.cullingMask,
                $"Wrong cullingMask for <{cameraComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
            
            Assert.AreEqual(prefabCameraComponent.fieldOfView, cameraComponent.fieldOfView, 5f,
                $"Wrong fieldOfView for <{cameraComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
        }
        
        [Test, Description("Must have a GameObject named 'UICamera' with a configured <Camera> component")]
        public void SceneHasUICamera()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "UICamera";
            SkipCheck(objectNameUnderTest);
            
            // Get prefab and component
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            var prefabCameraComponent = GetComponentFromGameObject<Camera>(prefab);
            
            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            
            // Check Camera component is attached
            var cameraComponent = GetEnabledComponentFromGameObject<Camera>(gameObjectUnderTest);
            
            // Check UI camera's properties
            Assert.AreEqual(prefabCameraComponent.cullingMask, cameraComponent.cullingMask,
                $"Wrong culling mask for <{cameraComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");

            Assert.AreEqual(prefabCameraComponent.orthographic, cameraComponent.orthographic,
                $"Wrong projection type for <{cameraComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
            
            Assert.AreEqual(prefabCameraComponent.nearClipPlane, cameraComponent.nearClipPlane,
                $"Wrong Clipping Plane Near value for <{cameraComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
            
            Assert.AreEqual(prefabCameraComponent.farClipPlane, cameraComponent.farClipPlane,
                $"Wrong Clipping Plane Far value for <{cameraComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
        }

        [Test, Description("Must have a GameObject named 'UI' with configured <Canvas> component")]
        public void SceneHasUserInterface() 
        {
            // GameObject to be tested
            const string objectNameUnderTest = "UI";
            SkipCheck(objectNameUnderTest);
            
            // Get prefab and components
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            var prefabCanvasComponent = GetComponentFromGameObject<Canvas>(prefab);
            var prefabCanvasScalerComponent = GetComponentFromGameObject<CanvasScaler>(prefab);
            var prefabGraphicRaycasterComponent = GetComponentFromGameObject<GraphicRaycaster>(prefab);
            
            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            
            // Check components are attached
            var canvasComponent = GetEnabledComponentFromGameObject<Canvas>(gameObjectUnderTest);
            var canvasScalerComponent = GetEnabledComponentFromGameObject<CanvasScaler>(gameObjectUnderTest);
            var graphicRaycasterComponent = GetEnabledComponentFromGameObject<GraphicRaycaster>(gameObjectUnderTest);
            
            // Check GameObject is set to correct layer
            Assert.AreEqual(prefab.layer, gameObjectUnderTest.layer);

            // Check Canvas component's properties
            Assert.AreEqual(prefabCanvasComponent.renderMode,canvasComponent.renderMode,
                $"Wrong Render Mode for <{canvasComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
            Assert.AreEqual(prefabCanvasComponent.pixelPerfect,canvasComponent.pixelPerfect,
                $"Wrong Pixel Perfect setting for <{canvasComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
            Assert.AreEqual(prefabCanvasComponent.worldCamera.name,canvasComponent.worldCamera.name,
                $"Wrong Camera for <{canvasComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
            Assert.AreEqual(prefabCanvasComponent.planeDistance,canvasComponent.planeDistance,
                $"Wrong Plane Distance value for <{canvasComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");

            // Check Canvas Scaler component's properties
            Assert.AreEqual(prefabCanvasScalerComponent.uiScaleMode, canvasScalerComponent.uiScaleMode,
                $"Wrong UI Scale Mode for <{prefabCanvasScalerComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
            Assert.AreEqual(prefabCanvasScalerComponent.referenceResolution, canvasScalerComponent.referenceResolution,
                $"Wrong Reference Resolution for <{prefabCanvasScalerComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
            Assert.AreEqual(prefabCanvasScalerComponent.screenMatchMode, canvasScalerComponent.screenMatchMode,
                $"Wrong Screen Match Mode for <{prefabCanvasScalerComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
            Assert.AreEqual(prefabCanvasScalerComponent.matchWidthOrHeight, canvasScalerComponent.matchWidthOrHeight,
                $"Wrong Match value for <{prefabCanvasScalerComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
            Assert.AreEqual(prefabCanvasScalerComponent.referencePixelsPerUnit, canvasScalerComponent.referencePixelsPerUnit,
                $"Wrong Reference Pixels Per Unit for <{prefabCanvasScalerComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
            
            // Check Graphics Raycaster component's properties
            Assert.AreEqual(prefabGraphicRaycasterComponent.blockingMask, graphicRaycasterComponent.blockingMask,
                $"Wrong Blocking Mask for <{prefabGraphicRaycasterComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
            Assert.AreEqual(prefabGraphicRaycasterComponent.blockingObjects, graphicRaycasterComponent.blockingObjects,
                $"Wrong Blocking Objects for <{prefabGraphicRaycasterComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
            Assert.AreEqual(prefabGraphicRaycasterComponent.ignoreReversedGraphics, graphicRaycasterComponent.ignoreReversedGraphics,
                $"Wrong ignoreReversedGraphics value for <{prefabGraphicRaycasterComponent.GetType().Name}> component of '{gameObjectUnderTest.name}'");
        }
        
        [SkipTestForScenesWithReason("FaradaysLaw", "scene accidently(?) has two EventSystems!")] // TODO fixme
        [Test, Description("Must have a GameObject named 'EventSystem'")]
        public void SceneHasEventSystem()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "EventSystem";
            SkipCheck(objectNameUnderTest);
            
            // Get matching prefab
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            
            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            
            // Check GameObject is set to correct layer
            Assert.AreEqual(prefab.layer, gameObjectUnderTest.layer);
            
            // Check GameObject is child of UI
            Assert.AreEqual(prefab.transform.parent.name, gameObjectUnderTest.transform.parent.name,
                $"GameObject '{objectNameUnderTest}' is not a child GameObject of '{prefab.transform.parent.name}'");
        }
        
        [SkipTestForScenesWithReason("CathodeRayTube", "scene intentionally(?) has no Assessment Panel")]
        [SkipTestForScenesWithReason("CoulombsLaw", "scene is not using the up-to-date 'ExperimentSetting.pc' prefab")]
        [Test, Description("Must have a GameObject named 'PanelAssessment'")]
        public void SceneHasUiPanelAssessment()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PanelAssessment";
            SkipCheck(objectNameUnderTest);
            
            // Get matching prefab
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            
            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            
            // Check GameObject is set to correct layer
            Assert.AreEqual(prefab.layer, gameObjectUnderTest.layer);
            
            // Check GameObject is child of UI
            Assert.AreEqual(prefab.transform.parent.name, gameObjectUnderTest.transform.parent.name,
                $"GameObject '{objectNameUnderTest}' is not a child GameObject of '{prefab.transform.parent.name}'");
        }
        
        [SkipTestForScenesWithReason("Optics", "scene intentionally disabled the Controls Panels")]
        [Test, Description("Must have a GameObject named 'PanelControls'")]
        public void SceneHasUiPanelControls()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PanelControls";
            SkipCheck(objectNameUnderTest);
            
            // Get matching prefab
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            
            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            
            // Check GameObject is set to correct layer
            Assert.AreEqual(prefab.layer, gameObjectUnderTest.layer);
            
            // Check GameObject is child of UI
            Assert.AreEqual(prefab.transform.parent.name, gameObjectUnderTest.transform.parent.name,
                $"GameObject '{objectNameUnderTest}' is not a child GameObject of '{prefab.transform.parent.name}'");
        }
        
        [Test, Description("Must have a GameObject named 'PanelDialogue '")]
        public void SceneHasUiPanelDialogue()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PanelDialogue ";
            SkipCheck(objectNameUnderTest);
            
            // Get matching prefab
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            
            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            
            // Check GameObject is set to correct layer
            Assert.AreEqual(prefab.layer, gameObjectUnderTest.layer);
            
            // Check GameObject is child of UI
            Assert.AreEqual(prefab.transform.parent.name, gameObjectUnderTest.transform.parent.name,
                $"GameObject '{objectNameUnderTest}' is not a child GameObject of '{prefab.transform.parent.name}'");
        }
               
        [Test, Description("Must have a GameObject named 'PanelExit'")]
        public void SceneHasUiPanelExit()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PanelExit";
            SkipCheck(objectNameUnderTest);
            
            // Get matching prefab
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            
            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            
            // Check GameObject is set to correct layer
            Assert.AreEqual(prefab.layer, gameObjectUnderTest.layer);
            
            // Check GameObject is child of UI
            Assert.AreEqual(prefab.transform.parent.name, gameObjectUnderTest.transform.parent.name,
                $"GameObject '{objectNameUnderTest}' is not a child GameObject of '{prefab.transform.parent.name}'");
        }
        
        [Test, Description("Must have a GameObject named 'PanelOptions'")]
        public void SceneHasUiPanelOptions()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PanelOptions";
            SkipCheck(objectNameUnderTest);
            
            // Get matching prefab
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            
            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            
            // Check GameObject is set to correct layer
            Assert.AreEqual(prefab.layer, gameObjectUnderTest.layer);
            
            // Check GameObject is child of UI
            Assert.AreEqual(prefab.transform.parent.name, gameObjectUnderTest.transform.parent.name,
                $"GameObject '{objectNameUnderTest}' is not a child GameObject of '{prefab.transform.parent.name}'");
        }
        
        [SkipTestForScenesWithReason(
             "CoulombsLaw, HuygensPrinciple, Pendulum, VandeGraaffBalloon, VandeGraaffGenerator, Whiteboard",
             "scene is not using the up-to-date 'ExperimentSetting.pc' prefab")]
        [Test, Description("Must have a GameObject named 'PauseMenu'")]
        public void SceneHasPauseMenu()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PauseMenu";
            SkipCheck(objectNameUnderTest);
            
            // Get matching prefab
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);

            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);

            // Check the Pause Menu is hidden by default 
            var canvasGameObject = gameObjectUnderTest.transform.Find("Canvas");
            Assert.False(canvasGameObject.gameObject.activeInHierarchy, $"{objectNameUnderTest}'s '{canvasGameObject.name}' should be disabled by default!");
        }

        [Test, Description("Must have a GameObject named 'ExperimentRoom'")]
        public void SceneHasExperimentRoom()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "ExperimentRoom";
            SkipCheck(objectNameUnderTest);
            
            // Get prefab
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            var expectedTag = prefab.tag;

            // Check GameObject exists, is active and tagged
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            Assert.AreEqual(expectedTag, gameObjectUnderTest.tag,
                $"GameObject '{gameObjectUnderTest.name}' is missing its tag '{expectedTag}'");
        }
        
        [SkipTestForScenesWithReason("Whiteboard", "scene is not using the up-to-date 'ExperimentSetting.pc' prefab")]
        [Test, Description("Must have a GameObject named 'SimulationController'")]
        public void SceneHasSimulationController()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "SimulationController";
            SkipCheck(objectNameUnderTest);

            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
        }
        
        [Test, Description("Must have a GameObject named 'LanguageManager'")]
        public void SceneHasLanguageManager()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "LanguageManager";
            SkipCheck(objectNameUnderTest);

            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
        }

        [Test, Description("Must have a GameObject named 'GlobalEntities'")]
        public void SceneHasGlobalEntities()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "GlobalEntities";
            SkipCheck(objectNameUnderTest);

            // Check GameObject exists and is active
            var gameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObject);
        }

        /**
         * Wrapper for utility function with shorter param list
         * Skips test if check is triggered
         */
        private void SkipCheck(string objectNameUnderTest, [CallerMemberName] string callingMethodName = null)
        {
            SkipCheckLong<PcSceneValidationTests>(PcExperimentPrefabName, _objectNamesFromExperimentPrefab,
                _experimentName, objectNameUnderTest, callingMethodName);
        }
    }
}
