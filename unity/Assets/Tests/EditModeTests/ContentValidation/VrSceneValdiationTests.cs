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
        
        [Test, Description("Must include a 'PlayerVR' Prefab tagged as 'Player'")]
        public void SceneHasVrPlayer()
        {
            // GameObject to be tested
            const string objectNameUnderTest = "VRPlayer";
            SkipCheck(objectNameUnderTest);
            
            var prefab = _gameObjectsFromExperimentPrefab.First(go => go.name == objectNameUnderTest);
            var expectedTag = prefab.tag;
            
            // Check GameObject exists, is active and tagged
            var gameObjectUnderTest = FindObjectByName(objectNameUnderTest);
            AssertGameObjectIsActive(gameObjectUnderTest);
            Assert.AreEqual(expectedTag, gameObjectUnderTest.tag,
                $"GameObject '{gameObjectUnderTest.name}' is missing its tag '{expectedTag}'");
        }

        [Test, Description("Must include an 'IndoorWorld' Prefab")]
        public void SceneHasIndoorWorld()
        {
            var indoorWorldGameObject = GameObject.Find("Indoor World") ?? GameObject.Find("IndoorWorld");
            Assert.NotNull(indoorWorldGameObject, "No 'IndoorWorld' or 'Indoor World' GameObject found");
        }
        
        [Test, Description("Must include a 'preDoorVR' Prefab")]
        public void SceneHasDoorMesh()
        {
            var doorGameObject = GameObject.Find("preDoorVR") ?? GameObject.Find("Door");
            Assert.NotNull(doorGameObject, "No 'preDoorVR' or 'Door' GameObject found");
            // TODO stopped here
        }
        
        [SkipTestFor("FaradaysLaw")]
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
            SkipCheckLong<PcSceneValidationTests>(VrExperimentPrefabName, _objectNamesFromExperimentPrefab,
                _experimentName, objectNameUnderTest, callingMethodName);
        }
    }
}
