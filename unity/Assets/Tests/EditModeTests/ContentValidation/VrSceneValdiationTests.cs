using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;
using static Tests.Utilities.UtilityFunctions;
using static Tests.Utilities.Constants;

namespace Tests.EditModeTests.ContentValidation
{
    /// <summary>
    ///  Runs all contained test methods for all scenes provided by <see cref="VrScenesProvider"/>
    /// </summary>
    [TestFixtureSource(typeof(VrScenesProvider))]
    public class VrSceneValidationTests
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly string _experimentName;
        private readonly string _scenePath;
        
        private GameObject[] _gameObjectsFromExperimentPrefab;
        private string[] _objectNamesFromExperimentPrefab;
        
        public VrSceneValidationTests(string experimentName, string scenePath) =>
            (_experimentName, _scenePath) = (experimentName, scenePath);
        
        // Before running any tests, query ExperimentSetting prefab and load the scene if it's not yet loaded
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Get objects and names from ExperimentSetting prefab
            _gameObjectsFromExperimentPrefab = GetAllGameObjectsFromPrefab(ExperimentPrefabName + "." + TypeVR);
            _objectNamesFromExperimentPrefab = _gameObjectsFromExperimentPrefab.Select(x => x.name).ToArray();
            
            // Load scene
            LoadSceneIfNotYetLoaded(_scenePath);
        }
        
        [Test, Description("Must include a 'VRPlayer' Prefab tagged as 'Player'")]
        public void SceneHasVrPlayer()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "VRPlayer";
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
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            var expectedLayer = prefab.layer;
            
            // Check GameObject exists, is active and on correct layer
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            Assert.AreEqual(expectedLayer, gameObjectUnderTest.layer,
                $"GameObject '{gameObjectUnderTest.name}' is not set to layer '{expectedLayer}'");
        }
        
        [SkipTestForScenesWithReason("Whiteboard", ReasonItsOutdated)]
        [SkipTestForScenesWithReason("HuygensPrinciple", "scene intentionally has an inactive Experiment Table")]
        [SkipTestForScenesWithReason("FallingCoil, FaradaysLaw", "scene accidently(?) has two Experiment Tables!")] // TODO fixme
        [Test, Description("Must include an 'ExperimentTable' Prefab set to layer 'Inventary'")]
        public void SceneHasExperimentTable()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "Experiment_Table";
            SkipCheck(objectNameUnderTest);
            
            // Get prefab
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
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
        
        /*************
         * Utilities *
         *************/
        
        /// <summary>
        /// Provides scenes of type VR
        /// </summary>
        private class VrScenesProvider : ScenesProvider { protected override string sceneType => "vr"; }

        /// <summary>
        ///  Skips test if check is triggered
        /// </summary>
        /// <param name="objectNameUnderTest">name of object under test</param>
        /// <param name="callingMethodName">test method name (automatically provided through <see cref="CallerMemberNameAttribute"/>)</param>
        /// <remarks>
        /// Wrapper for utility function <see cref="SkipCheckBase"/> with shorter param list and fixture specific arguments
        /// </remarks>
        private void SkipCheck(string objectNameUnderTest, [CallerMemberName] string callingMethodName = null)
        {
            SkipCheckBase<VrSceneValidationTests>(TypeVR, _objectNamesFromExperimentPrefab,
                _experimentName, objectNameUnderTest, callingMethodName);
        }
    }
}
