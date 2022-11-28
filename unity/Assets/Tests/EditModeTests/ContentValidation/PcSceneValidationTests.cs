using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using GEAR.Localization;
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
        
        [SkipTestFor("StateMachine")]
        [Test, Description("Must have a GameObject with enabled <Camera> component tagged as 'MainCamera'")]
        public void SceneHasMainCamera()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "MainCamera";
            ToSkipOrNotToSkip(objectNameUnderTest);

            // Get prefab and component
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            var prefabCameraComponent = GetComponentFromGameObject<Camera>(prefab);

            // Check GameObject exists and is active
            var cameraGameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(cameraGameObject);

            // Check Camera component is attached
            var cameraComponent = GetEnabledComponentFromGameObject<Camera>(cameraGameObject);
            
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
            // GameObject to be tested
            const string objectNameUnderTest = "UICamera";
            ToSkipOrNotToSkip(objectNameUnderTest);
            
            // Get prefab and component
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            var prefabCameraComponent = GetComponentFromGameObject<Camera>(prefab);
            
            // Check GameObject exists and is active
            var cameraGameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(cameraGameObject);
            
            // Check Camera component is attached
            var cameraComponent = GetEnabledComponentFromGameObject<Camera>(cameraGameObject);
            
            // Check UI camera's properties
            Assert.AreEqual(prefabCameraComponent.cullingMask, cameraComponent.cullingMask,
                $"Wrong culling mask for '{cameraComponent.GetType().Name}' component of '{cameraGameObject.name}'");

            Assert.AreEqual(prefabCameraComponent.orthographic, cameraComponent.orthographic,
                $"Wrong projection type for '{cameraComponent.GetType().Name}' component of '{cameraGameObject.name}'");
            
            Assert.AreEqual(prefabCameraComponent.nearClipPlane, cameraComponent.nearClipPlane,
                $"Wrong Clipping Plane Near value for '{cameraComponent.GetType().Name}' component of '{cameraGameObject.name}'");
            
            Assert.AreEqual(prefabCameraComponent.farClipPlane, cameraComponent.farClipPlane,
                $"Wrong Clipping Plane Far value for '{cameraComponent.GetType().Name}' component of '{cameraGameObject.name}'");
        }

        [Test, Description("Must have a GameObject named 'UI' with configured 'Canvas' component")]
        public void SceneHasUserInterface()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "UI";
            ToSkipOrNotToSkip(objectNameUnderTest);
            
            // Get prefab and components
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            var prefabCanvasComponent = GetComponentFromGameObject<Canvas>(prefab);
            var prefabCanvasScalerComponent = GetComponentFromGameObject<CanvasScaler>(prefab);
            var prefabGraphicRaycasterComponent = GetComponentFromGameObject<GraphicRaycaster>(prefab);
            
            // Check GameObject exists and is active
            var uiGameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(uiGameObject);
            
            // Check components are attached
            var canvasComponent = GetEnabledComponentFromGameObject<Canvas>(uiGameObject);
            var canvasScalerComponent = GetEnabledComponentFromGameObject<CanvasScaler>(uiGameObject);
            var graphicRaycasterComponent = GetEnabledComponentFromGameObject<GraphicRaycaster>(uiGameObject);
            
            // Check GameObject is set to correct layer
            Assert.AreEqual(prefab.layer, uiGameObject.layer);

            // Check Canvas component's properties
            Assert.AreEqual(prefabCanvasComponent.renderMode,canvasComponent.renderMode,
                $"Wrong Render Mode for '{canvasComponent.GetType().Name}' component of '{uiGameObject.name}'");
            Assert.AreEqual(prefabCanvasComponent.pixelPerfect,canvasComponent.pixelPerfect,
                $"Wrong Pixel Perfect setting for '{canvasComponent.GetType().Name}' component of '{uiGameObject.name}'");
            Assert.AreEqual(prefabCanvasComponent.worldCamera.name,canvasComponent.worldCamera.name,
                $"Wrong Camera for '{canvasComponent.GetType().Name}' component of '{uiGameObject.name}'");
            Assert.AreEqual(prefabCanvasComponent.planeDistance,canvasComponent.planeDistance,
                $"Wrong Plane Distance value for '{canvasComponent.GetType().Name}' component of '{uiGameObject.name}'");

            // Check Canvas Scaler component's properties
            Assert.AreEqual(prefabCanvasScalerComponent.uiScaleMode, canvasScalerComponent.uiScaleMode,
                $"Wrong UI Scale Mode for '{prefabCanvasScalerComponent.GetType().Name}' component of '{uiGameObject.name}'");
            Assert.AreEqual(prefabCanvasScalerComponent.referenceResolution, canvasScalerComponent.referenceResolution,
                $"Wrong Reference Resolution for '{prefabCanvasScalerComponent.GetType().Name}' component of '{uiGameObject.name}'");
            Assert.AreEqual(prefabCanvasScalerComponent.screenMatchMode, canvasScalerComponent.screenMatchMode,
                $"Wrong Screen Match Mode for '{prefabCanvasScalerComponent.GetType().Name}' component of '{uiGameObject.name}'");
            Assert.AreEqual(prefabCanvasScalerComponent.matchWidthOrHeight, canvasScalerComponent.matchWidthOrHeight,
                $"Wrong Match value for '{prefabCanvasScalerComponent.GetType().Name}' component of '{uiGameObject.name}'");
            Assert.AreEqual(prefabCanvasScalerComponent.referencePixelsPerUnit, canvasScalerComponent.referencePixelsPerUnit,
                $"Wrong Reference Pixels Per Unit for '{prefabCanvasScalerComponent.GetType().Name}' component of '{uiGameObject.name}'");
            
            // Check Graphics Raycaster component's properties
            Assert.AreEqual(prefabGraphicRaycasterComponent.blockingMask, graphicRaycasterComponent.blockingMask,
                $"Wrong Blocking Mask for '{prefabGraphicRaycasterComponent.GetType().Name}' component of '{uiGameObject.name}'");
            Assert.AreEqual(prefabGraphicRaycasterComponent.blockingObjects, graphicRaycasterComponent.blockingObjects,
                $"Wrong Blocking Objects for '{prefabGraphicRaycasterComponent.GetType().Name}' component of '{uiGameObject.name}'");
            Assert.AreEqual(prefabGraphicRaycasterComponent.ignoreReversedGraphics, graphicRaycasterComponent.ignoreReversedGraphics,
                $"Wrong ignoreReversedGraphics value for '{prefabGraphicRaycasterComponent.GetType().Name}' component of '{uiGameObject.name}'");

            // Check EventSystem exists
            var eventSystem = GameObject.Find("EventSystem");
            Assert.NotNull(eventSystem, "No 'EventSystem' GameObject found");
            Assert.AreEqual(uiGameObject.transform, eventSystem.transform.parent, "EventSystem is not a child GameObject of 'UI'");
        }
        
        [SkipTestFor("FaradaysLaw")] // TODO: experiment has two event systems!
        [Test, Description("Must have a GameObject named 'EventSystem'")]
        public void SceneHasEventSystem()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "EventSystem";
            ToSkipOrNotToSkip(objectNameUnderTest);
            
            // Check GameObject exists and is active
            var uiGameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(uiGameObject);
        }
        
        [SkipTestFor("CathodeRayTube", "CoulombsLaw")]
        [Test, Description("Must have a GameObject named 'PanelAssessment'")]
        public void SceneHasUiPanelAssessment()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PanelAssessment";
            ToSkipOrNotToSkip(objectNameUnderTest);
            
            // Check GameObject exists and is active
            var uiGameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(uiGameObject);
        }
        
        [SkipTestFor("Optics")]
        [Test, Description("Must have a GameObject named 'PanelControls'")]
        public void SceneHasUiPanelControls()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PanelControls";
            ToSkipOrNotToSkip(objectNameUnderTest);
            
            // Check GameObject exists and is active
            var uiGameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(uiGameObject);
        }
        
        [Test, Description("Must have a GameObject named 'PanelDialogue '")]
        public void SceneHasUiPanelDialogue()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PanelDialogue ";
            ToSkipOrNotToSkip(objectNameUnderTest);
            
            // Check GameObject exists and is active
            var uiGameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(uiGameObject);
        }
               
        [Test, Description("Must have a GameObject named 'PanelExit'")]
        public void SceneHasUiPanelExit()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PanelExit";
            ToSkipOrNotToSkip(objectNameUnderTest);
            
            // Check GameObject exists and is active
            var uiGameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(uiGameObject);
        }
        
        [Test, Description("Must have a GameObject named 'PanelOptions'")]
        public void SceneHasUiPanelOptions()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PanelOptions";
            ToSkipOrNotToSkip(objectNameUnderTest);
            
            // Check GameObject exists and is active
            var uiGameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(uiGameObject);
        }

        [Test, Description("Must have a GameObject named 'ExperimentRoom'")]
        public void SceneHasExperimentRoom()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "ExperimentRoom";
            ToSkipOrNotToSkip(objectNameUnderTest);

            // Check GameObject exists and is active
            var uiGameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(uiGameObject);
        }
        
        [SkipTestFor("Whiteboard")]
        [Test, Description("Must have a GameObject named 'SimulationController'")]
        public void SceneHasSimulationController()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "SimulationController";
            ToSkipOrNotToSkip(objectNameUnderTest);

            // Check GameObject exists and is active
            var uiGameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(uiGameObject);
        }
        
        [Test, Description("Must have a GameObject named 'LanguageManager'")]
        public void SceneHasLanguageManager()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "LanguageManager";
            ToSkipOrNotToSkip(objectNameUnderTest);

            // Check GameObject exists and is active
            var uiGameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(uiGameObject);
        }

        [Test, Description("Must have a GameObject named 'GlobalEntities'")]
        public void SceneHasGlobalEntities()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "GlobalEntities";
            ToSkipOrNotToSkip(objectNameUnderTest);

            // Check GameObject exists and is active
            var uiGameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(uiGameObject);
        }
        
        // Skip all experiments not using the updated template
        [SkipTestFor("CoulombsLaw", "HuygensPrinciple", "Pendulum", "VandeGraaffBalloon", "VandeGraaffGenerator", "Whiteboard")]
        [Test, Description("Must have a GameObject named 'PauseMenu'")]
        public void SceneHasPauseMenu()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PauseMenu";
            ToSkipOrNotToSkip(objectNameUnderTest);

            // Check GameObject exists and is active
            var uiGameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(uiGameObject);

            var canvasGameObject = uiGameObject.transform.Find("Canvas");
            Assert.False(canvasGameObject.gameObject.activeInHierarchy, $"{objectNameUnderTest}'s Canvas should be disabled by default!");
        }
        
        
        /**
         * Custom attribute (decorator) for tests
         * Provides a list of experiment names to skip the respective test
         */
        [AttributeUsage(AttributeTargets.Method)]
        private class SkipTestFor : Attribute
        {
            public string[] ExperimentNames { get; }

            public SkipTestFor(params string[] experimentNames) {
                ExperimentNames = experimentNames;
            }

            public static SkipTestFor GetAttributeCustom<T>(string methodName) where T : class
            {
                try
                {
                    return (SkipTestFor)typeof(T).GetMethod(methodName).GetCustomAttributes(typeof(SkipTestFor), false).FirstOrDefault();
                }
                catch(SystemException)
                {
                    return null;
                }
            }
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

            if (!_objectNamesFromExperimentPrefab.Any(x => x.ToUpper().Contains(objectNameUnderTest.ToUpper())))
                Assert.Ignore($"{PcExperimentPrefabName} contains no {objectNameUnderTest} - skipping test!");
        }
    }
}
