using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        private Scene _scene;

        public VrSceneValidationTests(string experimentName, string scenePath)
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
        
        [Test, Description("Must include a 'PlayerVR' Prefab tagged as 'Player'")]
        public void SceneHasPlayer()
        {
            var playerGameObject = GameObject.Find("VRPlayer");
            Assert.NotNull(playerGameObject, "No 'Player' GameObject found");
            Assert.AreEqual("Player", playerGameObject.tag, "GameObject 'PlayerVR' not tagged as 'Player'");
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
        }
        
        [Test, Description("Must include the 'WhiteboardInteractive' Prefab")]
        public void SceneHasWhiteboardInteractive()
        {
            // List of scenes that skip the test
            var scenesToSkip = new List<string> { "FaradaysLaw" };
            
            // Skip listed scene(s)
            if (scenesToSkip.Any(x => _experimentName.ToUpper().Contains(x.ToUpper())))
                Assert.Ignore($"{_experimentName} scene has intentionally no 'WhiteBoardInteractive'");

            var whiteboardInteractiveGameObject = GameObject.Find("WhiteboardInteractive");
            Assert.NotNull(whiteboardInteractiveGameObject, "No 'WhiteboardInteractive' GameObject found");
        }
        
        [Test, Description("Must include the 'LanguageManager' Prefab")]
        public void SceneHasLanguageManager()
        {
            // List of scenes that skip the test
            var scenesToSkip = new List<string> { "Whiteboard" };
            
            // Skip listed scene(s)
            if (scenesToSkip.Any(x => _experimentName.ToUpper().Contains(x.ToUpper())))
                Assert.Ignore($"{_experimentName} scene has intentionally no 'LanguageManager'");

            var languageManagerGameObject = GameObject.Find("LanguageManager"); 
            Assert.NotNull(languageManagerGameObject);
        }
        
        [Test, Description("Must include the 'GlobalEntities' Prefab")]
        public void SceneHasGlobalEntities()
        {
            var globalEntitiesGameObject = GameObject.Find("GlobalEntities"); 
            Assert.NotNull(globalEntitiesGameObject, "No 'GlobalEntities' GameObject found");
        }
    }
}
