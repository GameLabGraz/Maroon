using System.Collections;
using System.Linq;
using GEAR.Localization;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

// TODO move utility functions to Utilities and/or use those already there

namespace Tests.PlayModeTests.MenuTests
{
    using static PlaymodeTestUtils;
    /*
     * Check if its possible to generalize language tests, e.g. get available languages from LM
     * then compare new language objects to objects found with default language
     * for main menu and each submenu
     */
    public class MainMenuLanguageTests
    {
        private const string MainMenuScenePath = "Assets/Maroon/scenes/special/MainMenu.pc.unity";
        private const SystemLanguage DefaultLanguage = SystemLanguage.English;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            // Start from Main Menu for every following test
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(MainMenuScenePath, new LoadSceneParameters(LoadSceneMode.Single));

            var platformManager = GameObject.Find("PlatformManager");

            var currentSceneName = SceneManager.GetActiveScene().name;
            Assert.AreEqual("MainMenu.pc", currentSceneName, "'MainMenu.pc' scene did not load");
            
            Assert.AreEqual(DefaultLanguage, LanguageManager.Instance.CurrentLanguage,
                $"Default language is not set to '{DefaultLanguage.ToString()}'");
        }
        
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            // Destroy LM to clear its previous state (DontDestroyOnLoad workaround)
            Object.Destroy(FindInActiveGameObjectByName("LanguageManager"));
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator WhenSelectGermanLanguageInMenuThenLanguageManagerLanguageIsGerman()
        {
            // Find and click Language button
            string languageButtonLabel = LanguageManager.Instance.GetString("Menu Language", DefaultLanguage);
            GetButtonViaText(languageButtonLabel).onClick.Invoke();
            yield return null;
            
            // Find and click German button
            string germanButtonLabel = LanguageManager.Instance.GetString("German", DefaultLanguage);
            GetButtonViaText(germanButtonLabel).onClick.Invoke();
            yield return null;

            // Check if LanguageManager's language property is now German
            var expectedLanguage = SystemLanguage.German;
            var actualLanguage = LanguageManager.Instance.CurrentLanguage;
            
            Assert.AreEqual(expectedLanguage, actualLanguage,
                $"LanguageManager's 'CurrentLanguage' property is not {expectedLanguage.ToString()}");
        }
        
        [UnityTest]
        public IEnumerator WhenSelectGermanLanguageInMenuThenLanguageSubMenuIsGerman()
        {
            var buttonLabel = "Menu Language";
            var newLanguage = SystemLanguage.German;
            
            var button = GetButtonViaText(LanguageManager.Instance.GetString(buttonLabel, DefaultLanguage));
            button.onClick.Invoke();
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator WhenSelectGermanLanguageInMenuThenLanguageManagerSettingAndMenuLabelsChanged()
        {
            string[] topLevelButtonLabels = { "Menu Lab", "Menu Audio", "Menu Language", "Menu Credits", "Menu Exit" };
            
            // Find and store top level main menu buttons by their English label
            var preLanguageChangeButtons = topLevelButtonLabels.ToDictionary(
                buttonLabel => buttonLabel,
                buttonLabel => GetButtonViaText(LanguageManager.Instance.GetString(buttonLabel, DefaultLanguage))
            );

            // Find and click Language button
            string languageButtonLabel = LanguageManager.Instance.GetString("Menu Language", DefaultLanguage);
            GetButtonViaText(languageButtonLabel).onClick.Invoke();
            yield return null;
            
            // Find and click German button
            string germanButtonLabel = LanguageManager.Instance.GetString("German", DefaultLanguage);
            GetButtonViaText(germanButtonLabel).onClick.Invoke();
            yield return null;

            var expectedLanguage = SystemLanguage.German;
            
            // Find and store top level main menu buttons by their German label
            var postLanguageChangeButtons = topLevelButtonLabels.ToDictionary(
                buttonLabel => buttonLabel,
                buttonLabel => GetButtonViaText(LanguageManager.Instance.GetString(buttonLabel, expectedLanguage))
            );

            // Compare old English and new German labeled buttons if they are identical
            topLevelButtonLabels.ToList().ForEach(buttonLabel =>
                Assert.AreEqual(preLanguageChangeButtons[buttonLabel], postLanguageChangeButtons[buttonLabel])
            );

            /*
             * TODO
             * split language tests up, in case any of the steps fail it will be easier to debug
             * one test for languagemanager setting changed
             * one test for each toplevel button and its related submenu (maybe include main menu banner)
             *
             * More ideas:
             * make a new test suite for all menu language tests
             * get current language in setup and store as default language
             * test for changing it to English if everything is labeled correctly
             * test for changing it to German if everything is labeled correctly
             *
             * reasoning: independant of any future changes regarding the default language setting
             */
        }
    }
}
