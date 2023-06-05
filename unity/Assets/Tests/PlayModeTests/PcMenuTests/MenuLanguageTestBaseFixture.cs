using System.Collections;
using GEAR.Localization;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Tests.Utilities.Constants;
using static Tests.Utilities.PlaymodeUtilities;
using static Tests.Utilities.UtilityFunctions;

namespace Tests.PlayModeTests.PcMenuTests
{
    public abstract class MenuLanguageTestBaseFixture
    {
        /// <summary>
        /// Expected default language on LanguageManager instantiation
        /// </summary>
        private const SystemLanguage ExpectedDefaultLanguage = SystemLanguage.English;

        /// <summary>
        /// Expected language to test against
        /// </summary>
        protected const SystemLanguage ExpectedLanguage = SystemLanguage.German;
        
        /// <summary>
        /// Default language of LanguageManager on scene start - assumed to be English
        /// </summary>
        protected SystemLanguage DefaultLanguage;

        /// <summary>
        /// Target scene to load for running tests
        /// </summary>
        private readonly string _scenePathToLoad;
        
        protected MenuLanguageTestBaseFixture(string scenePathToLoad) => _scenePathToLoad = scenePathToLoad;
        
        /// <summary>
        /// Load appropriate scene, verify default language, prepare pause menu tests
        /// </summary>
        /// <returns></returns>
        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return LoadSceneAndCheckItsLoadedCorrectly(_scenePathToLoad);
                
            DefaultLanguage = LanguageManager.Instance.CurrentLanguage;
            if (!DefaultLanguage.Equals(ExpectedDefaultLanguage))
            {
                Assert.Ignore("This test is based on the assumption that LanguageManager's " +
                              $"default language is {ExpectedDefaultLanguage}." +
                              "\nThis test is therefore inconclusive and will be skipped.");
            }
            
            // Main menu test prep stops here
            if (_scenePathToLoad != FallingCoilScenePath) yield break;
            
            // Testing the Pause Menu requires activating it (usually done by pressing ESC)
            // Workaround to pressing ESC: enable the Pause Menu Canvas
            // Looked into Input System tests to try simulating ESC keypress but could not access InputSystem assemblies :(
            // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.3/manual/Testing.html
            // Enter playmode to allow for activating the Pause Menu Canvas
            yield return new EnterPlayMode();
            
            var canvasGameObject = FindObjectByName("Canvas");
            Assert.AreEqual(canvasGameObject.transform.parent.name, PauseMenu);
            canvasGameObject.SetActive(true);
            
            // skip a frame and verify it's now active
            yield return null;
            AssertGameObjectIsActive(canvasGameObject);
        }
        
        [TearDown]
        public void TearDown()
        {
            DestroyPermanentObjects();
        }

        /// <summary>
        /// Find and click Menu's Language button
        /// </summary>
        protected IEnumerator OpenLanguageSubMenu()
        {
            yield return ClickButtonByLanguageManagerKey("Menu Language", DefaultLanguage);
        }

        /// <summary>
        /// Find and click Sub Menu Language's German button
        /// </summary>
        protected IEnumerator ClickGermanLanguageButton()
        {
            yield return ClickButtonByLanguageManagerKey("German", DefaultLanguage);
        }
    }
}