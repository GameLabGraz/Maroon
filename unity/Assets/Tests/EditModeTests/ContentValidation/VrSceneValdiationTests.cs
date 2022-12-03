using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Tests.Utilities.Utilities;

namespace Tests.EditModeTests.ContentValidation
{
    public class VrScenesProvider : ScenesProvider
    {
        protected override Regex experimentNameRegex => new Regex(@"\w+\.vr");
        protected override string fileEnding => ".vr.unity";
    }

    // Runs OneTimeSetUp and all Tests once for each provided experiment scene path
    [TestFixtureSource(typeof(VrScenesProvider))]
    public class VrSceneValidationTests
    {
        private readonly string _experimentName;
        private readonly string _scenePath;

        private const string VrExperimentPrefabName = "ExperimentSetting.vr";
        
        private List<GameObject> _gameObjectsFromExperimentPrefab;
        private string[] _objectNamesFromExperimentPrefab;

        public VrSceneValidationTests(string experimentName, string scenePath)
        {
            _experimentName = experimentName;
            _scenePath = scenePath;
        }
        
        // Before running any tests, query ExperimentSetting prefab and load the scene if it's not yet loaded
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Get "mandatory" object names from ExperimentSetting prefab
            var experimentSettingPrefab = GetPrefabByName(VrExperimentPrefabName);
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

        [SkipTestForScenesWithReason("Whiteboard", "scene is not using the up-to-date 'ExperimentSetting.vr' prefab")]
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
        
        [SkipTestForScenesWithReason("Whiteboard", "scene is not using the up-to-date 'ExperimentSetting.vr' prefab")]
        [SkipTestForScenesWithReason("FallingCoil", "scene accidently(?) has two Experiment Tables!")] // TODO fixme
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

        [SkipTestForScenesWithReason("Whiteboard", "scene is not using the up-to-date 'ExperimentSetting.vr' prefab")]
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
        
        [SkipTestForScenesWithReason("Whiteboard", "scene is not using the up-to-date 'ExperimentSetting.vr' prefab")]
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
        
        [SkipTestForScenesWithReason("Whiteboard", "scene is not using the up-to-date 'ExperimentSetting.vr' prefab")]
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

        [SkipTestForScenesWithReason("Whiteboard", "scene is not using the up-to-date 'ExperimentSetting.vr' prefab")]
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
        
        [SkipTestForScenesWithReason("Whiteboard", "scene is not using the up-to-date 'ExperimentSetting.vr' prefab")]
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
        
        [SkipTestForScenesWithReason("Whiteboard", "scene is not using the up-to-date 'ExperimentSetting.vr' prefab")]
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
        
        [SkipTestForScenesWithReason("Whiteboard", "scene is not using the up-to-date 'ExperimentSetting.vr' prefab")]
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

        /**
         * Wrapper for utility function with shorter param list
         * Skips test if check is triggered
         */
        private void SkipCheck(string objectNameUnderTest, [CallerMemberName] string callingMethodName = null)
        {
            SkipCheckLong<VrSceneValidationTests>(VrExperimentPrefabName, _objectNamesFromExperimentPrefab,
                _experimentName, objectNameUnderTest, callingMethodName);
        }
    }
}
