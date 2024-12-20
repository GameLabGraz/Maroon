using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using static Tests.Utilities.Constants;
using static Tests.Utilities.CustomAttributes;
using static Tests.Utilities.UtilityFunctions;

namespace Tests.EditModeTests.ContentValidation
{
    /// <summary>
    /// Content validation TestFixture for PC scenes.
    /// Contains all test methods for scenes provided by <see cref="PcScenesProvider"/>.
    /// </summary>
    [TestFixtureSource(typeof(PcScenesProvider))]
    public sealed class PcSceneValidationTests : SceneValidationBaseFixture<PcSceneValidationTests>
    {
        /// <summary>
        /// Derived constructor used by TestFixtureSource annotation to initialize attributes
        /// </summary>
        /// <param name="experimentName">Name of the experiment scene to be tested</param>
        /// <param name="scenePath">Relative path to scene starting from "Assets" folder</param>
        public PcSceneValidationTests(string experimentName, string scenePath) :
            base(experimentName, scenePath, TypePC) {}

        /// <summary>
        /// Called once per scene before any tests are started
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            BaseOneTimeSetup();
        }
        
        /* Tests start here! */

        [SkipTestForScenesWithReason("StateMachine, MinimumSpanningTree", "scene has a different camera setup")]
        [Test, Description("Must have a GameObject named 'MainCamera' with a configured <Camera> component")]
        public void SceneHasMainCamera()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "MainCamera";
            SkipCheck(objectNameUnderTest);

            // Get prefab and component
            var prefab = GameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            var prefabCameraComponent = GetComponentFromPrefab<Camera>(prefab);
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
            var prefab = GameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            var prefabCameraComponent = GetComponentFromPrefab<Camera>(prefab);
            
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
            var prefab = GameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            var prefabCanvasComponent = GetComponentFromPrefab<Canvas>(prefab);
            var prefabCanvasScalerComponent = GetComponentFromPrefab<CanvasScaler>(prefab);
            var prefabGraphicRaycasterComponent = GetComponentFromPrefab<GraphicRaycaster>(prefab);
            
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
        [SkipTestForScenesWithReason("PlanetarySystem", ReasonIntentionallyMissing)]
        [Test, Description("Must have a GameObject named 'EventSystem'")]
        public void SceneHasEventSystem()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "EventSystem";
            SkipCheck(objectNameUnderTest);
            
            // Get matching prefab
            var prefab = GameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            
            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            
            // Check GameObject is set to correct layer
            Assert.AreEqual(prefab.layer, gameObjectUnderTest.layer);
            
            // Check GameObject is child of UI
            Assert.AreEqual(prefab.transform.parent.name, gameObjectUnderTest.transform.parent.name,
                $"GameObject '{objectNameUnderTest}' is not a child GameObject of '{prefab.transform.parent.name}'");
        }
        
        [SkipTestForScenesWithReason("CathodeRayTube, PlanetarySystem", ReasonIntentionallyMissing)]
        [SkipTestForScenesWithReason("CoulombsLaw", ReasonItsOutdated)]
        [Test, Description("Must have a GameObject named 'PanelAssessment'")]
        public void SceneHasUiPanelAssessment()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PanelAssessment";
            SkipCheck(objectNameUnderTest);
            
            // Get matching prefab
            var prefab = GameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            
            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            
            // Check GameObject is set to correct layer
            Assert.AreEqual(prefab.layer, gameObjectUnderTest.layer);
            
            // Check GameObject is child of UI
            Assert.AreEqual(prefab.transform.parent.name, gameObjectUnderTest.transform.parent.name,
                $"GameObject '{objectNameUnderTest}' is not a child GameObject of '{prefab.transform.parent.name}'");
        }
        
        [SkipTestForScenesWithReason("Optics, PlanetarySystem", ReasonIntentionallyMissing)]
        [Test, Description("Must have a GameObject named 'PanelControls'")]
        public void SceneHasUiPanelControls()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PanelControls";
            SkipCheck(objectNameUnderTest);
            
            // Get matching prefab
            var prefab = GameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            
            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            
            // Check GameObject is set to correct layer
            Assert.AreEqual(prefab.layer, gameObjectUnderTest.layer);
            
            // Check GameObject is child of UI
            Assert.AreEqual(prefab.transform.parent.name, gameObjectUnderTest.transform.parent.name,
                $"GameObject '{objectNameUnderTest}' is not a child GameObject of '{prefab.transform.parent.name}'");
        }

        [SkipTestForScenesWithReason("PlanetarySystem", ReasonIntentionallyMissing)]
        [Test, Description("Must have a GameObject named 'PanelDialogue '")]
        public void SceneHasUiPanelDialogue()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PanelDialogue ";
            SkipCheck(objectNameUnderTest);
            
            // Get matching prefab
            var prefab = GameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            
            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            
            // Check GameObject is set to correct layer
            Assert.AreEqual(prefab.layer, gameObjectUnderTest.layer);
            
            // Check GameObject is child of UI
            Assert.AreEqual(prefab.transform.parent.name, gameObjectUnderTest.transform.parent.name,
                $"GameObject '{objectNameUnderTest}' is not a child GameObject of '{prefab.transform.parent.name}'");
        }

        [SkipTestForScenesWithReason("PlanetarySystem", ReasonIntentionallyMissing)]
        [Test, Description("Must have a GameObject named 'PanelExit'")]
        public void SceneHasUiPanelExit()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PanelExit";
            SkipCheck(objectNameUnderTest);
            
            // Get matching prefab
            var prefab = GameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            
            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            
            // Check GameObject is set to correct layer
            Assert.AreEqual(prefab.layer, gameObjectUnderTest.layer);
            
            // Check GameObject is child of UI
            Assert.AreEqual(prefab.transform.parent.name, gameObjectUnderTest.transform.parent.name,
                $"GameObject '{objectNameUnderTest}' is not a child GameObject of '{prefab.transform.parent.name}'");
        }

        [SkipTestForScenesWithReason("PlanetarySystem", ReasonIntentionallyMissing)]
        [Test, Description("Must have a GameObject named 'PanelOptions'")]
        public void SceneHasUiPanelOptions()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PanelOptions";
            SkipCheck(objectNameUnderTest);
            
            // Get matching prefab
            var prefab = GameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            
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
             "CoulombsLaw, HuygensPrinciple, Pendulum, PointWaveExperiment, TitrationExperiment, VandeGraaffBalloon, VandeGraaffGenerator, Whiteboard",
             ReasonItsOutdated)]
        [Test, Description("Must have a GameObject named 'PauseMenu'")]
        public void SceneHasPauseMenu()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "PauseMenu";
            SkipCheck(objectNameUnderTest);
            
            // Get matching prefab
            var prefab = GameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);

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
            var prefab = GameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            var expectedTag = prefab.tag;

            // Check GameObject exists, is active and tagged
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            Assert.AreEqual(expectedTag, gameObjectUnderTest.tag,
                $"GameObject '{gameObjectUnderTest.name}' is missing its tag '{expectedTag}'");
        }
        
        [SkipTestForScenesWithReason("Whiteboard", ReasonItsOutdated)]
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
    }
}
