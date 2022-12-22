using System.Collections;
using System.Linq;
using GEAR.Localization;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using static Tests.Utilities.Constants;
using static Tests.Utilities.UtilityFunctions;

namespace Tests.PlayModeTests.PcMenuTests.LanguageMenuTests
{
    /// <summary>
    /// Tests Main Menu language switch of all buttons and labels
    /// </summary>
    [TestFixture]
    public class MainMenuLanguageTests : MenuLanguageTestBaseFixture
    {
        public MainMenuLanguageTests() : base(MainMenuScenePath) {}

        [UnityTest, Order(1), Description("On selecting a new language in the Main Menu, the LanguageManagers 'CurrentLanguage' property changes")]
        public IEnumerator WhenSelectGermanLanguageThenLanguageManagerIsGerman()
        {
            yield return OpenLanguageSubMenu();
            yield return ClickGermanLanguageButton();

            // Check if LanguageManager's language property is now German
            var actualLanguage = LanguageManager.Instance.CurrentLanguage;
            Assert.AreEqual(ExpectedLanguage, actualLanguage,
                $"LanguageManager's 'CurrentLanguage' property is not {ExpectedLanguage.ToString()}");
        }

        private static TestCaseData[] _mainMenuButtonKeysSource = new [] {
            new TestCaseData("Menu Lab").Returns(null),
            new TestCaseData("Menu Audio").Returns(null),
            new TestCaseData("Menu Language").Returns(null),
            new TestCaseData("Menu Credits").Returns(null),
            new TestCaseData("Menu Exit").Returns(null)
        };
        
        [UnityTest, Order(2), TestCaseSource(nameof(_mainMenuButtonKeysSource)),
         Description("On selecting a new language in the Main Menu, the Main Menu's buttons match")]
        public IEnumerator WhenSelectGermanLanguageThenMainMenuButtonsAreGerman(string buttonKey)
        {
            // Find and store top level main menu button
            var button = GetButtonViaTextLabel(LanguageManager.Instance.GetString(buttonKey, DefaultLanguage));
            
            yield return OpenLanguageSubMenu();
            yield return ClickGermanLanguageButton();
            
            var expectedButtonLabel = LanguageManager.Instance.GetString(buttonKey, ExpectedLanguage);
            var actualButtonLabel = button.GetComponentInChildren<TextMeshProUGUI>().text;
            Assert.AreEqual(expectedButtonLabel, actualButtonLabel,
                $"Button is wrong after changing language to {ExpectedLanguage}");
        }

        [UnityTest, Order(3), Description("On selecting a new language in the Main Menu, the main menu's title matches")]
        public IEnumerator WhenSelectGermanLanguageThenMainMenuTitleIsGerman()
        {
            TextMeshProUGUI mainMenuTitle = GameObject.Find("preMenuColumnMainMenu(Clone)").GetComponentsInChildren<TextMeshProUGUI>()
                .First(x => x.text == LanguageManager.Instance.GetString("Menu Main Menu", DefaultLanguage));
            
            yield return OpenLanguageSubMenu();
            yield return ClickGermanLanguageButton();
            
            var expectedTitleLabel = LanguageManager.Instance.GetString("Menu Main Menu", ExpectedLanguage);
            var actualTitleLabel = mainMenuTitle.text;
            Assert.AreEqual(expectedTitleLabel, actualTitleLabel,
                $"Main Menu Title is wrong after changing language to {ExpectedLanguage}");
        }

        
        private static TestCaseData[] _checkSubMenuTitlesSource = new [] {
            new TestCaseData("Menu Lab", "preMenuColumnLaboratorySelection").Returns(null),
            new TestCaseData("Menu Audio", "preMenuColumnAudio").Returns(null),
            new TestCaseData("Menu Language", "preMenuColumnLanguage").Returns(null),
            new TestCaseData("Menu Credits", "preMenuColumnCredits").Returns(null)
        };
        
        [UnityTest, Order(4), TestCaseSource(nameof(_checkSubMenuTitlesSource)),
         Description("On selecting a new language in the Main Menu, the sub menu's title matches")]
        public IEnumerator WhenSelectGermanLanguageThenSubMenuTitlesAreGerman(string titleKey, string menuPrefab)
        {
            yield return OpenLanguageSubMenu();
            yield return ClickGermanLanguageButton();
         
            // Find and click sourced Main Menu button
            string sourceButtonLabel = LanguageManager.Instance.GetString(titleKey, ExpectedLanguage);
            GetButtonViaTextLabel(sourceButtonLabel).onClick.Invoke();
            yield return null;
            
            // Get SubMenu's title
            TextMeshProUGUI sourcedSubmenuTitle = GameObject.Find(menuPrefab + "(Clone)").GetComponentsInChildren<TextMeshProUGUI>()
                .First(x => x.text == LanguageManager.Instance.GetString(titleKey, ExpectedLanguage));
            
            var expectedTitleLabel = LanguageManager.Instance.GetString(titleKey, ExpectedLanguage);
            var actualTitleLabel = sourcedSubmenuTitle.text;
            
            Assert.AreEqual(expectedTitleLabel, actualTitleLabel, $"Title is wrong after changing language to {ExpectedLanguage}");
        }

        [UnityTest, Order(5), Description("On selecting a new language in the Main Menu, the Audio sub menu's labels match")]
        public IEnumerator WhenSelectGermanLanguageThenAudioSubMenuIsGerman()
        {
            yield return OpenLanguageSubMenu();
            yield return ClickGermanLanguageButton();
            
            // Find and click Main Menu Audio button
            string audioButtonLabel = LanguageManager.Instance.GetString("Menu Audio", ExpectedLanguage);
            GetButtonViaTextLabel(audioButtonLabel).onClick.Invoke();
            yield return null;

            var expectedMusicSliderLabel = LanguageManager.Instance.GetString("Menu Music", ExpectedLanguage);
            var actualMusicSliderLabel = GetComponentFromGameObjectOrItsChildrenByName<TextMeshProUGUI>("preMenuButtonSliderMusic").text;
            
            var expectedFxSliderLabel = LanguageManager.Instance.GetString("Menu Sound", ExpectedLanguage);
            var actualFxSliderLabel = GetComponentFromGameObjectOrItsChildrenByName<TextMeshProUGUI>("preMenuButtonSliderFx").text;
            
            Assert.AreEqual(expectedMusicSliderLabel, actualMusicSliderLabel,
                $"Music Slider label is wrong after changing language to {ExpectedLanguage}");
            
            Assert.AreEqual(expectedFxSliderLabel, actualFxSliderLabel,
                $"FX Slider label is wrong after changing language to {ExpectedLanguage}");
        }
        
        [UnityTest, Order(6), Description("On selecting a new language in the Main Menu, the Language sub menu's labels match")]
        public IEnumerator WhenSelectGermanLanguageThenLanguageSubMenuIsGerman()
        {
            yield return OpenLanguageSubMenu();
            yield return ClickGermanLanguageButton();
            
            // Find English and German buttons
            string germanButtonLabel = LanguageManager.Instance.GetString("German", ExpectedLanguage);
            string englishButtonLabel = LanguageManager.Instance.GetString("English", ExpectedLanguage);
            Button germanButton = GetButtonViaTextLabel(germanButtonLabel);
            Button englishButton = GetButtonViaTextLabel(englishButtonLabel);
            
            var expectedGermanButtonLabel = LanguageManager.Instance.GetString("German", ExpectedLanguage);
            var actualGermanButtonLabel = germanButton.GetComponentInChildren<TextMeshProUGUI>().text;
            
            var expectedEnglishButtonLabel = LanguageManager.Instance.GetString("English", ExpectedLanguage);
            var actualEnglishButtonLabel = englishButton.GetComponentInChildren<TextMeshProUGUI>().text;

            Assert.AreEqual(expectedGermanButtonLabel, actualGermanButtonLabel,
                $"German Language Button label is wrong after changing language to {ExpectedLanguage}");
            
            Assert.AreEqual(expectedEnglishButtonLabel, actualEnglishButtonLabel,
                $"English Language Button label is wrong after changing language to {ExpectedLanguage}");
        }
    }
}
