using System.Collections;
using System.Linq;
using GEAR.Localization;
using NUnit.Framework;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using static Tests.Utilities.Constants;
using static Tests.Utilities.UtilityFunctions;

namespace Tests.PlayModeTests.MenuTests
{
    /*
     * Check if its possible to generalize language tests, e.g. get available languages from LM
     * then compare new language objects to objects found with default language
     * for main menu and each submenu
     */
    // TODO do same as in audio test: main and pause menu?
    public class MainMenuLanguageTests
    {
        private SystemLanguage _defaultLanguage;
        private SystemLanguage _expectedLanguage = SystemLanguage.German;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            // Start from Main Menu for every following test
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(MainMenuScenePath, new LoadSceneParameters(LoadSceneMode.Single));
            
            var currentSceneName = SceneManager.GetActiveScene().name;
            Assert.AreEqual(MainMenuPcSceneName, currentSceneName, $"Scene '{MainMenuPcSceneName}' did not load");

            _defaultLanguage = LanguageManager.Instance.CurrentLanguage;
            Assume.That(_defaultLanguage, Is.EqualTo(SystemLanguage.English),
                $"LanguageManager's default language is not English. However, the following tests are based on this assumption");
        }
        
        [TearDown]
        public void TearDown()
        {
            // Destroy DontDestroyOnLoad'ed objects to prevent tests possibly affecting each other
            Object.Destroy(FindObjectByName("LanguageManager"));
            Object.Destroy(FindObjectByName("GlobalEntities"));
        }
        
        [UnityTest, Order(1), Description("On selecting a new language in the menu, the LanguageManagers 'CurrentLanguage' property changes")]
        public IEnumerator WhenSelectGermanLanguageThenLanguageManagerIsGerman()
        {
            // Find and click Main Menu Language button
            string languageButtonLabel = LanguageManager.Instance.GetString("Menu Language", _defaultLanguage);
            GetButtonViaText(languageButtonLabel).onClick.Invoke();
            yield return null;
            
            // Find and click Sub Menu Language's German button
            string germanButtonLabel = LanguageManager.Instance.GetString("German", _defaultLanguage);
            GetButtonViaText(germanButtonLabel).onClick.Invoke();
            yield return null;

            // Check if LanguageManager's language property is now German
            var actualLanguage = LanguageManager.Instance.CurrentLanguage;
            Assert.AreEqual(_expectedLanguage, actualLanguage,
                $"LanguageManager's 'CurrentLanguage' property is not {_expectedLanguage.ToString()}");
        }

        private static TestCaseData[] _checkMainMenuButtonsSource = new [] {
            new TestCaseData("Menu Lab").Returns(null),
            new TestCaseData("Menu Audio").Returns(null),
            new TestCaseData("Menu Language").Returns(null),
            new TestCaseData("Menu Credits").Returns(null),
            new TestCaseData("Menu Exit").Returns(null)
        };
        
        [UnityTest, Order(2), TestCaseSource(nameof(_checkMainMenuButtonsSource)),
         Description("On selecting a new language in the menu, the main menu's buttons match")]
        public IEnumerator WhenSelectGermanLanguageThenMainMenuButtonsChanged(string buttonKey)
        {   
            // Find and store top level main menu button
            var button = GetButtonViaText(LanguageManager.Instance.GetString(buttonKey, _defaultLanguage));
            
            // Find and click Main Menu Language button
            string languageButtonLabel = LanguageManager.Instance.GetString("Menu Language", _defaultLanguage);
            GetButtonViaText(languageButtonLabel).onClick.Invoke();
            yield return null;
            
            // Find and click Sub Menu Language's German button
            string germanButtonLabel = LanguageManager.Instance.GetString("German", _defaultLanguage);
            GetButtonViaText(germanButtonLabel).onClick.Invoke();
            yield return null;
            
            var expectedButtonLabel = LanguageManager.Instance.GetString(buttonKey, _expectedLanguage);
            var actualButtonLabel = button.GetComponentInChildren<TextMeshProUGUI>().text;
            Assert.AreEqual(expectedButtonLabel, actualButtonLabel);
        }

        [UnityTest, Order(3), Description("On selecting a new language in the menu, the main menu's title matches")]
        public IEnumerator WhenSelectGermanLanguageThenMainMenuTitleChanged()
        {
            TextMeshProUGUI mainMenuTitle = GameObject.Find("preMenuColumnMainMenu(Clone)").GetComponentsInChildren<TextMeshProUGUI>()
                .First(x => x.text == LanguageManager.Instance.GetString("Menu Main Menu", _defaultLanguage));
            
            // Find and click Main Menu Language button
            string languageButtonLabel = LanguageManager.Instance.GetString("Menu Language", _defaultLanguage);
            GetButtonViaText(languageButtonLabel).onClick.Invoke();
            yield return null;
            
            // Find and click Sub Menu Language's German button
            string germanButtonLabel = LanguageManager.Instance.GetString("German", _defaultLanguage);
            GetButtonViaText(germanButtonLabel).onClick.Invoke();
            yield return null;
            
            var expectedTitleLabel = LanguageManager.Instance.GetString("Menu Main Menu", _expectedLanguage);
            var actualTitleLabel = mainMenuTitle.text;
            Assert.AreEqual(expectedTitleLabel, actualTitleLabel,
                $"Main Menu Title is wrong after changing language to German");
        }

        
        private static TestCaseData[] _checkSubMenuTitlesSource = new [] {
            new TestCaseData("Menu Lab", "preMenuColumnLaboratorySelection").Returns(null),
            new TestCaseData("Menu Audio", "preMenuColumnAudio").Returns(null),
            new TestCaseData("Menu Language", "preMenuColumnLanguage").Returns(null),
            new TestCaseData("Menu Credits", "preMenuColumnCredits").Returns(null)
        };
        
        [UnityTest, Order(4), TestCaseSource(nameof(_checkSubMenuTitlesSource)),
         Description("On selecting a new language in the menu, the sub menu's title matches")]
        public IEnumerator WhenSelectGermanLanguageThenSubMenuTitlesChanged(string titleKey, string menuPrefab)
        {
            // Find and click Main Menu Language button
            string languageButtonLabel = LanguageManager.Instance.GetString("Menu Language", _defaultLanguage);
            GetButtonViaText(languageButtonLabel).onClick.Invoke();
            yield return null;
 
            // Find and click Sub Menu Language's German button
            string germanButtonLabel = LanguageManager.Instance.GetString("German", _defaultLanguage);
            GetButtonViaText(germanButtonLabel).onClick.Invoke();
            yield return null;
         
            // Find and click sourced Main Menu button
            string sourceButtonLabel = LanguageManager.Instance.GetString(titleKey, _expectedLanguage);
            GetButtonViaText(sourceButtonLabel).onClick.Invoke();
            yield return null;
            
            // Get SubMenu's title
            TextMeshProUGUI sourcedSubmenuTitle = GameObject.Find(menuPrefab + "(Clone)").GetComponentsInChildren<TextMeshProUGUI>()
                .First(x => x.text == LanguageManager.Instance.GetString(titleKey, _expectedLanguage));
            
            var expectedTitleLabel = LanguageManager.Instance.GetString(titleKey, _expectedLanguage);
            var actualTitleLabel = sourcedSubmenuTitle.text;
            
            Assert.AreEqual(expectedTitleLabel, actualTitleLabel,
                $"Title is wrong after changing language to German");
        }

        [UnityTest, Order(5), Description("On selecting a new language in the menu, the Audio sub menu's labels match")]
        public IEnumerator WhenSelectGermanLanguageThenAudioSubMenuIsGerman()
        {
            // Find and click Main Menu Language button
            string languageButtonLabel = LanguageManager.Instance.GetString("Menu Language", _defaultLanguage);
            GetButtonViaText(languageButtonLabel).onClick.Invoke();
            yield return null;
            
            // Find and click Sub Menu Language's German button
            string germanButtonLabel = LanguageManager.Instance.GetString("German", _defaultLanguage);
            GetButtonViaText(germanButtonLabel).onClick.Invoke();
            yield return null;
            
            // Find and click Main Menu Audio button
            string audioButtonLabel = LanguageManager.Instance.GetString("Menu Audio", _expectedLanguage);
            GetButtonViaText(audioButtonLabel).onClick.Invoke();
            yield return null;

            var expectedMusicSliderLabel = LanguageManager.Instance.GetString("Menu Music", _expectedLanguage);
            var actualMusicSliderLabel = GetComponentFromGameObjectOrItsChildrenByName<TextMeshProUGUI>("preMenuButtonSliderMusic").text;
            
            var expectedFxSliderLabel = LanguageManager.Instance.GetString("Menu Sound", _expectedLanguage);
            var actualFxSliderLabel = GetComponentFromGameObjectOrItsChildrenByName<TextMeshProUGUI>("preMenuButtonSliderFx").text;
            
            Assert.AreEqual(expectedMusicSliderLabel, actualMusicSliderLabel,
                $"Music Slider label is wrong after changing language to German");
            
            Assert.AreEqual(expectedFxSliderLabel, actualFxSliderLabel,
                $"FX Slider label is wrong after changing language to German");
        }
        
        [UnityTest, Order(6), Description("On selecting a new language in the menu, the Language sub menu's labels match")]
        public IEnumerator WhenSelectGermanLanguageThenLanguageSubMenuIsGerman()
        {
            // Find and click Main Menu Language button
            string languageButtonLabel = LanguageManager.Instance.GetString("Menu Language", _defaultLanguage);
            GetButtonViaText(languageButtonLabel).onClick.Invoke();
            yield return null;
            
            // Find English and German buttons and the submenu's title
            string germanButtonLabel = LanguageManager.Instance.GetString("German", _defaultLanguage);
            string englishButtonLabel = LanguageManager.Instance.GetString("English", _defaultLanguage);
            Button germanButton = GetButtonViaText(germanButtonLabel);
            Button englishButton = GetButtonViaText(englishButtonLabel);

            GetButtonViaText(germanButtonLabel).onClick.Invoke();
            yield return null;

            var expectedGermanButtonLabel = LanguageManager.Instance.GetString("German", _expectedLanguage);
            var actualGermanButtonLabel = germanButton.GetComponentInChildren<TextMeshProUGUI>().text;
            
            var expectedEnglishButtonLabel = LanguageManager.Instance.GetString("English", _expectedLanguage);
            var actualEnglishButtonLabel = englishButton.GetComponentInChildren<TextMeshProUGUI>().text;

            Assert.AreEqual(expectedGermanButtonLabel, actualGermanButtonLabel,
                $"German Language Button label is wrong after changing language to German");
            
            Assert.AreEqual(expectedEnglishButtonLabel, actualEnglishButtonLabel,
                $"English Language Button label is wrong after changing language to German");
        }
    }
}
