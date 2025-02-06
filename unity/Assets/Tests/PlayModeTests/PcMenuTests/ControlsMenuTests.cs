using Maroon.GlobalEntities.ControlsManager;
using System.Collections;
using System.Linq;
using GEAR.Localization;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using static Tests.Utilities.Constants;
using static Tests.Utilities.PlaymodeUtilities;
using static Tests.Utilities.UtilityFunctions;

namespace Tests.PlayModeTests.PcMenuTests
{
    /// <summary>
    /// Tests Controls Menu functionality by manipulating audio sliders in Main and Pause Menu
    /// </summary>
    [TestFixture(MainMenu)]
    [TestFixture(PauseMenu)]
    public class ControlsMenuTests
    {
        private readonly string _menuType;
        private bool _sceneLoaded;
        private Button _settingsButton;
        private Button _controlsButton;
        
        private const string MouseSensitivitySliderName = "preMenuButtonSliderMouseSensitivity";

        private Slider _mouseSensitivitySliderComponent;

        public ControlsMenuTests(string menuType)
        {
            _menuType = menuType;
            _sceneLoaded = false;
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            // Workaround for missing UnityOneTimeSetUp to load scene only once for faster test execution
            // This code is run only once for the whole test fixture
            // https://forum.unity.com/threads/add-coroutine-version-of-onetimesetup.890092/
            if (!_sceneLoaded)
            {
                // Load scene matching the TestFixture parameter
                if (_menuType == "MainMenu")
                {
                    yield return LoadSceneAndCheckItsLoadedCorrectly(MainMenuScenePath);
                }
                else if (_menuType == "PauseMenu")
                {
                    yield return LoadSceneAndCheckItsLoadedCorrectly(FallingCoilScenePath);
                }
                else
                {
                    Assert.Fail("Unknown parameter provided to TestFixture");
                }
                
                _sceneLoaded = true;
                
                // Enter playmode to enable proper testing
                yield return new EnterPlayMode();

                // Testing the Pause Menu requires activating it (usually done by pressing ESC)
                if (_menuType == "PauseMenu")
                {
                    // Workaround to pressing ESC: enable the Pause Menu Canvas
                    // Looked into Input System tests to try simulating ESC keypress but could not access InputSystem assemblies :(
                    // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.3/manual/Testing.html
                    var canvasGameObject = FindObjectByName("Canvas");
                    Assert.AreEqual(canvasGameObject.transform.parent.name, PauseMenu);
                    canvasGameObject.SetActive(true);
                    
                    // skip a frame and verify it's now active
                    yield return null;
                    AssertGameObjectIsActive(canvasGameObject);
                }

                // Find settings button
                string mainMenuSettingsButtonLabel = LanguageManager.Instance.GetString("Menu Settings");
                _settingsButton = GetButtonViaTextLabel(mainMenuSettingsButtonLabel);

                // Click Settings
                _settingsButton.onClick.Invoke();
                yield return null;

                // Find audio submenu button
                string mainMenuControlsButtonLabel = LanguageManager.Instance.GetString("Menu Controls");
                _controlsButton = GetButtonViaTextLabel(mainMenuControlsButtonLabel);
            }

            // The code below is run before every test case in the test fixture
            // Open/reset audio menu
            _controlsButton.onClick.Invoke();

            // Get Slider components
            _mouseSensitivitySliderComponent = GetComponentFromGameObjectOrItsChildrenByName<Slider>(MouseSensitivitySliderName);
        }
        
        [UnityTest, Order(1), Description("On opening audio menu, slider values and ControlsManager mouse sensitivity must match")]
        public IEnumerator WhenOpenControlsMenuInitialSliderValuesEqualMouseSensitivity()
        {
            Assert.AreEqual(_mouseSensitivitySliderComponent.value, ControlsManager.Instance.MouseSensitivity, 0.0, 
                $"Initial mouse sensitivity Slider and ControlsManager.MouseSensitivity values must be equal");
            
            yield return null;
        }
        
        [UnityTest, Order(2), Description("Change the mouse sensitivity slider's value, then the ControlManager's volume must match")]
        public IEnumerator WhenChangeMouseSensitivitySliderValue_ThenControlManagerMatches()
        {
            _mouseSensitivitySliderComponent.value = 50.0f;
            
            yield return null;
            
            Assert.AreEqual(ControlsManager.Instance.MouseSensitivity, _mouseSensitivitySliderComponent.value, 0.0, 
                $"After {MouseSensitivitySliderName} value change, unexpected ControlManager mouse Sensitivity");
        }
        
        [UnityTest, Order(3), Description("Change mouse sensitivity sliders' values, then reload menu, the ControlManager's mouse sensitivity must match")]
        public IEnumerator WhenChangeSliderValuesAndReloadSubMenu_ThenControlManagersMouseSensitivityMatch()
        {
            float expectedMouseSensitivity = 300.0f;

            _mouseSensitivitySliderComponent.value = expectedMouseSensitivity;
            
            yield return null;
            
            // Reload the menu: destroys the audio menu and its child objects (slider)
            _controlsButton.onClick.Invoke();
            
            yield return null;

            // Get newly created sliders
            _mouseSensitivitySliderComponent = GetComponentFromGameObjectOrItsChildrenByName<Slider>(MouseSensitivitySliderName);

            Assert.AreEqual(expectedMouseSensitivity, _mouseSensitivitySliderComponent.value, 0.0, 
                $"After Controls menu reload, unexpected slider '{MouseSensitivitySliderName}' value");
            
            Assert.AreEqual(expectedMouseSensitivity, ControlsManager.Instance.MouseSensitivity, 0.0, 
                $"After Controls menu reload, unexpected ControlsManager mouse sensitivity");
        }
    }
}