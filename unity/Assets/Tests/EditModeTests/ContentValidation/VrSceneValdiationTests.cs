using System.Linq;
using NUnit.Framework;
using static Tests.Utilities.Constants;
using static Tests.Utilities.CustomAttributes;
using static Tests.Utilities.UtilityFunctions;

namespace Tests.EditModeTests.ContentValidation
{
    /// <summary>
    /// Content validation TestFixture for VR scenes.
    /// Contains all test methods for scenes provided by <see cref="VrScenesProvider"/>.
    /// </summary>
    [TestFixtureSource(typeof(VrScenesProvider))]
    public sealed class VrSceneValidationTests : SceneValidationBaseFixture<VrSceneValidationTests>
    {
        /// <summary>
        /// Derived constructor used by TestFixtureSource annotation to initialize attributes
        /// </summary>
        /// <param name="experimentName">Name of the experiment scene to be tested</param>
        /// <param name="scenePath">Relative path to scene starting from "Assets" folder</param>
        public VrSceneValidationTests(string experimentName, string scenePath) :
            base(experimentName, scenePath, TypeVR) {}

        /// <summary>
        /// Called once per scene before any tests are started
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            BaseOneTimeSetup();
        }
        
        /* Tests start here! */
        
        [Test, Description("Must include a 'VRPlayer' Prefab tagged as 'Player'")]
        public void SceneHasVrPlayer()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "VRPlayer";
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
        
        [Test, Description("Must include a 'Teleporting' Prefab")]
        public void SceneHasTeleporting()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "Teleporting";
            SkipCheck(objectNameUnderTest);

            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
        }

        [SkipTestForScenesWithReason("Whiteboard", ReasonItsOutdated)]
        [Test, Description("Must include an 'ExperimentRoom' Prefab set to layer 'Inventary'")]
        public void SceneHasExperimentRoom()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "ExperimentRoom";
            SkipCheck(objectNameUnderTest);
            
            // Get prefab
            var prefab = GameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            var expectedLayer = prefab.layer;
            
            // Check GameObject exists, is active and on correct layer
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            Assert.AreEqual(expectedLayer, gameObjectUnderTest.layer,
                $"GameObject '{gameObjectUnderTest.name}' is not set to layer '{expectedLayer}'");
        }
        
        [SkipTestForScenesWithReason("Whiteboard", ReasonItsOutdated)]
        [SkipTestForScenesWithReason("HuygensPrinciple", ReasonIntentionallyMissing)]
        [SkipTestForScenesWithReason("FallingCoil, FaradaysLaw", "scene accidently(?) has two Experiment Tables!")] // TODO fixme
        [Test, Description("Must include an 'ExperimentTable' Prefab set to layer 'Inventary'")]
        public void SceneHasExperimentTable()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "Experiment_Table";
            SkipCheck(objectNameUnderTest);
            
            // Get prefab
            var prefab = GameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            var expectedLayer = prefab.layer;
            
            // Check GameObject exists, is active and on correct layer
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            Assert.AreEqual(expectedLayer, gameObjectUnderTest.layer,
                $"GameObject '{gameObjectUnderTest.name}' is not set to layer '{expectedLayer}'");
        }

        [SkipTestForScenesWithReason("Whiteboard", ReasonItsOutdated)]
        [Test, Description("Must include a 'DoorVR' Prefab")]
        public void SceneHasDoorVr()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "DoorVR";
            SkipCheck(objectNameUnderTest);

            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
        }
        
        [SkipTestForScenesWithReason("Whiteboard", ReasonItsOutdated)]
        [Test, Description("Must include a 'TeleportPoints' Prefab")]
        public void SceneHasTeleportPoints()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "TeleportPoints";
            SkipCheck(objectNameUnderTest);

            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            
            Assert.True(gameObjectUnderTest.transform.childCount > 0,
                $"GameObject '{gameObjectUnderTest.name}' has no 'TeleportPoint' child objects");
        }
        
        [Test, Description("Must include the 'WhiteboardInteractive' Prefab")]
        public void SceneHasWhiteboardInteractive()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "WhiteboardInteractive";
            SkipCheck(objectNameUnderTest);

            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
        }
        
        [SkipTestForScenesWithReason("Whiteboard", ReasonItsOutdated)]
        [Test, Description("Must include the 'QuestManager' Prefab")]
        public void SceneHasQuestManager()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "QuestManager";
            SkipCheck(objectNameUnderTest);

            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
        }

        [SkipTestForScenesWithReason("Whiteboard", ReasonItsOutdated)]
        [Test, Description("Must include the 'VR_Controls' Prefab")]
        public void SceneHasVrControls()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "VR_Controls";
            SkipCheck(objectNameUnderTest);

            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
        }
        
        [SkipTestForScenesWithReason("Whiteboard", ReasonItsOutdated)]
        [Test, Description("Must include the 'ShelveWithDrawer' Prefab")]
        public void SceneHasShelveWithDrawer()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "ShelveWithDrawer";
            SkipCheck(objectNameUnderTest);

            // Check GameObject exists and is active
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
        }
        
        [SkipTestForScenesWithReason("Whiteboard", ReasonItsOutdated)]
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
        
        [SkipTestForScenesWithReason("Whiteboard", ReasonItsOutdated)]
        [Test, Description("Must have a GameObject named 'SimulationController'")]
        public void SceneHasSimulationController()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "SimulationController";
            SkipCheck(objectNameUnderTest);

            // Check GameObject exists and is active
            var gameObject = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObject);
        }
    }
}
