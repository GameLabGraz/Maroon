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
    /// Tests Pause Menu language switch of all buttons and labels
    /// </summary>
    [TestFixture]
    public class PauseMenuLanguageTests : MenuLanguageTestBaseFixture
    {
        public PauseMenuLanguageTests() : base(FallingCoilScenePath) {}

        [UnityTest, Order(1), Description("On selecting a new language in the Pause Menu, the LanguageManagers 'CurrentLanguage' property changes")]
        public IEnumerator WhenSelectGermanLanguageThenLanguageManagerIsGerman()
        {
            yield return OpenLanguageSubMenu();
            yield return ClickGermanLanguageButton();

            // Check if LanguageManager's language property is now German
            var actualLanguage = LanguageManager.Instance.CurrentLanguage;
            Assert.AreEqual(ExpectedLanguage, actualLanguage,
                $"LanguageManager's 'CurrentLanguage' property is not {ExpectedLanguage.ToString()}");
        }

        private static TestCaseData[] _pauseMenuButtonKeysSource = new [] {
            new TestCaseData("Menu Audio").Returns(null),
            new TestCaseData("Menu Language").Returns(null),
            new TestCaseData("Menu Main Menu").Returns(null),
            new TestCaseData("Menu Resume").Returns(null)
        };
        
        [UnityTest, Order(2), TestCaseSource(nameof(_pauseMenuButtonKeysSource)),
         Description("On selecting a new language in the Pause Menu, the Pause Menu's buttons match")]
        public IEnumerator WhenSelectGermanLanguageThenPauseMenuButtonsAreGerman(string buttonKey)
        {
            yield return OpenLanguageSubMenu();
            yield return ClickGermanLanguageButton();
            
            // Find top level pause menu button
            var button = GetButtonViaTextLabel(LanguageManager.Instance.GetString(buttonKey, ExpectedLanguage));
            
            // Check if button text language matches
            var expectedButtonLabel = LanguageManager.Instance.GetString(buttonKey, ExpectedLanguage);
            var actualButtonLabel = button.GetComponentInChildren<TextMeshProUGUI>().text;
            Assert.AreEqual(expectedButtonLabel, actualButtonLabel);
        }

        [UnityTest, Order(3), Description("On selecting a new language in the Pause Menu, the main menu's title matches")]
        public IEnumerator WhenSelectGermanLanguageThenPauseMenuTitleIsGerman()
        {
            yield return OpenLanguageSubMenu();
            yield return ClickGermanLanguageButton();
            
            // Find pause menu title
            var labelKeyUnderTest = "Menu Pause Menu";
            TextMeshProUGUI pauseMenuTitle = GameObject.Find("PauseMenu").GetComponentsInChildren<TextMeshProUGUI>()
                .First(x => x.text == LanguageManager.Instance.GetString(labelKeyUnderTest, ExpectedLanguage));

            // Check if pause menu title language matches
            var expectedTitleLabel = LanguageManager.Instance.GetString(labelKeyUnderTest, ExpectedLanguage);
            var actualTitleLabel = pauseMenuTitle.text;
            Assert.AreEqual(expectedTitleLabel, actualTitleLabel,
                $"Pause Menu Title is wrong after changing language to {ExpectedLanguage}");
        }
        
        private static TestCaseData[] _checkSubMenuTitlesSource = new [] {
            new TestCaseData("Menu Audio", "preMenuColumnAudio").Returns(null),
            new TestCaseData("Menu Language", "preMenuColumnLanguage").Returns(null),
        };
        
        [UnityTest, Order(4), TestCaseSource(nameof(_checkSubMenuTitlesSource)),
         Description("On selecting a new language in the Pause Menu, the sub menu's title matches")]
        public IEnumerator WhenSelectGermanLanguageThenSubMenuTitlesAreGerman(string titleKey, string menuPrefab)
        {
            yield return OpenLanguageSubMenu();
            yield return ClickGermanLanguageButton();
         
            // Find and click sourced Pause Menu button
            string sourceButtonLabel = LanguageManager.Instance.GetString(titleKey, ExpectedLanguage);
            GetButtonViaTextLabel(sourceButtonLabel).onClick.Invoke();
            yield return null;
            
            // Get SubMenu's title
            TextMeshProUGUI sourcedSubmenuTitle = GameObject.Find(menuPrefab + "(Clone)").GetComponentsInChildren<TextMeshProUGUI>()
                .First(x => x.text == LanguageManager.Instance.GetString(titleKey, ExpectedLanguage));
            
            var expectedTitleLabel = LanguageManager.Instance.GetString(titleKey, ExpectedLanguage);
            var actualTitleLabel = sourcedSubmenuTitle.text;
            
            Assert.AreEqual(expectedTitleLabel, actualTitleLabel,
                $"Title is wrong after changing language to {ExpectedLanguage}");
        }
        
        [UnityTest, Order(5), Description("On selecting a new language in the Pause Menu, the Audio sub menu's labels match")]
        public IEnumerator WhenSelectGermanLanguageThenAudioSubMenuIsGerman()
        {
            yield return OpenLanguageSubMenu();
            yield return ClickGermanLanguageButton();
            
            // Find and click Pause Menu Audio button
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
        
        [UnityTest, Order(6), Description("On selecting a new language in the Pause Menu, the Language sub menu's labels match")]
        public IEnumerator WhenSelectGermanLanguageThenLanguageSubMenuIsGerman()
        {
            yield return OpenLanguageSubMenu();
            yield return ClickGermanLanguageButton();
            
            var expectedGermanButtonLabel = LanguageManager.Instance.GetString("German", ExpectedLanguage);
            var expectedEnglishButtonLabel = LanguageManager.Instance.GetString("English", ExpectedLanguage);
            
            // Find language selection buttons for English and German
            Button germanButton = GetButtonViaTextLabel(expectedGermanButtonLabel);
            Button englishButton = GetButtonViaTextLabel(expectedEnglishButtonLabel);
            
            // Get translated text labels from Buttons
            var actualGermanButtonLabel = germanButton.GetComponentInChildren<TextMeshProUGUI>().text;
            var actualEnglishButtonLabel = englishButton.GetComponentInChildren<TextMeshProUGUI>().text;

            Assert.AreEqual(expectedGermanButtonLabel, actualGermanButtonLabel,
                $"German Language Button label is wrong after changing language to {ExpectedLanguage}");
            Assert.AreEqual(expectedEnglishButtonLabel, actualEnglishButtonLabel,
                $"English Language Button label is wrong after changing language to {ExpectedLanguage}");
        }
    }
}
