using System.Collections;
using System.Linq;
using GEAR.Localization;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

/*
 * Tests audio sliders of Main/Pause audio menu
 */

// TODO move utility functions to Utilities and/or use those already there

namespace Tests.PlayModeTests.MenuTests
{
    using static PlaymodeTestUtils;
    
    [TestFixture("MainMenu")]
    [TestFixture("PauseMenu")]
    public class AudioMenuTests
    {
        private readonly string _menuType;
        private bool _sceneLoaded;
        private Button _audioButton;
        
        private const string MusicAudioSourceName = "MusicSource";
        private const string FxAudioSourceName = "SoundEffectSource";
        private const string MusicSliderName = "preMenuButtonSliderMusic";
        private const string FxSliderName = "preMenuButtonSliderFx";

        private AudioSource _musicAudioSourceComponent;
        private AudioSource _fxAudioSourceComponent;
        private Slider _musicAudioSliderComponent;
        private Slider _fxAudioSliderComponent;

        public AudioMenuTests(string menuType)
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
                // Load scene
                if (_menuType == "MainMenu")
                {
                    yield return LoadMainMenuScene();
                }
                else if (_menuType == "PauseMenu")
                {
                    yield return LoadFallingCoilScene();
                }
                
                _sceneLoaded = true;
                
                // Enter playmode to initialize scene
                yield return new EnterPlayMode();

                // Pause menu requires activating the menu (usually done by pressing ESC)
                if (_menuType == "PauseMenu")
                {
                    // Workaround to pressing ESC: enable the Pause Menu Canvas
                    // Looked into Input System tests to try simulating ESC keypress but was not fruitful :(
                    // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.3/manual/Testing.html
                    var canvasGameObject = FindInActiveGameObjectByName("Canvas");
                    Assert.NotNull(canvasGameObject);
                    Assert.AreEqual(canvasGameObject.transform.parent.name, "PauseMenu");
                    canvasGameObject.SetActive(true);
                    yield return null;
                }
                
                // Find audio submenu button
                string mainMenuAudioButtonLabel = LanguageManager.Instance.GetString("Menu Audio");
                _audioButton = GetButtonViaText(mainMenuAudioButtonLabel);
            }
            
            // The code below is run before every test case in the test fixture
            // Open/reset audio menu
            _audioButton.onClick.Invoke();
            
            // Get AudioSource components
            _musicAudioSourceComponent = GetComponentFromGameObjectWithName<AudioSource>(MusicAudioSourceName);
            _fxAudioSourceComponent = GetComponentFromGameObjectWithName<AudioSource>(FxAudioSourceName);
            
            // Get Slider components
            _musicAudioSliderComponent = GetComponentInChildrenFromGameObjectWithName<Slider>(MusicSliderName);
            _fxAudioSliderComponent = GetComponentInChildrenFromGameObjectWithName<Slider>(FxSliderName);
        }
        
        [UnityTest, Order(1), Description("On opening audio menu, slider values and audio sources' volume must match")]
        public IEnumerator WhenOpenAudioMenuInitialSliderValuesEqualAudioSourceVolume()
        {
            // Note:
            // Would like to use https://docs.nunit.org/articles/nunit/writing-tests/assertions/multiple-asserts.html
            // But Assert.Multiple() is not (yet) supported by Unity test framework :(
            // Source: https://stackoverflow.com/a/52231966/7262963
            Assert.AreEqual(_musicAudioSliderComponent.value, _musicAudioSourceComponent.volume, 0.0, 
                $"Initial music Slider and AudioSource values must be equal");
            
            Assert.AreEqual(_fxAudioSliderComponent.value, _fxAudioSourceComponent.volume, 0.0, 
                $"Initial FX Slider and AudioSource values must be equal");
            yield return null;
        }
        
        [UnityTest, Order(2), Description("Change the music audio slider's value, then the music audio source's volume must match")]
        public IEnumerator WhenChangeMusicSliderValue_ThenMusicAudioSourceVolumeMatches()
        {
            _musicAudioSliderComponent.value = 0.6f;
            
            yield return null;
            
            Assert.AreEqual(_musicAudioSliderComponent.value, _musicAudioSourceComponent.volume, 0.0, 
                $"After {MusicSliderName} value change, unexpected audio source '{MusicAudioSourceName}' volume");
        }
        
        [UnityTest, Order(3), Description("Change the FX audio slider's value, then the FX audio source's volume must match")]
        public IEnumerator WhenChangeFxSliderValueThenFxAudioSourceVolumeMatches()
        {
            _fxAudioSliderComponent.value = 0.5f;
            
            yield return null;
            
            Assert.AreEqual(_fxAudioSliderComponent.value, _fxAudioSourceComponent.volume, 0.0, 
                $"After {FxSliderName} value change, unexpected audio source '{FxAudioSourceName}' volume");
        }
        
        [UnityTest, Order(4), Description("Change both audio slider's value independently, then both audio sources' volume must match their slider")]
        public IEnumerator WhenChangeBothSliderValuesIndependently_ThenBothAudioSourcesVolumeLevelsMatch()
        {
            _fxAudioSliderComponent.value = 0.1f;
            _musicAudioSliderComponent.value = 0.2f;
            
            Assert.AreEqual(_musicAudioSliderComponent.value, _musicAudioSourceComponent.volume, 0.0, 
                $"After changing both sliders, '{MusicSliderName}' value should match '{MusicAudioSourceName}' volume");
            
            Assert.AreEqual(_fxAudioSliderComponent.value, _fxAudioSourceComponent.volume, 0.0, 
                $"After changing both sliders, '{FxSliderName}' value should match '{FxAudioSourceName}' volume");
            yield return null;
        }
        
        
        [UnityTest, Order(5), Description("Change both audio sliders' values multiple times independently, then their audio sources' volumes must match")]
        public IEnumerator WhenChangeSliderValuesIndependentlyMultipleTimes_ThenBothAudioSourcesVolumeLevelsMatch()
        {
            float[] expectedMusicVolumeLevels = { 0f,  0.25f, 1/3f, 0.5f, 2/3f, 0.75f, 1f };
            float[] expectedFxVolumeLevels = expectedMusicVolumeLevels.Reverse().ToArray();

            for (int i = 0; i < expectedMusicVolumeLevels.Length; i++)
            {
                var expectedMusicVolume = expectedMusicVolumeLevels[i];
                var expectedFxVolume = expectedFxVolumeLevels[i];
                
                // Set slider value
                _musicAudioSliderComponent.value = expectedMusicVolume;
                _fxAudioSliderComponent.value = expectedFxVolume;
        
                yield return null;
                
                Assert.AreEqual(expectedMusicVolume, _musicAudioSliderComponent.value, 0.0, 
                    $"After audio volume change, unexpected slider '{MusicSliderName}' value");
                
                Assert.AreEqual(expectedMusicVolume, _musicAudioSourceComponent.volume, 0.0, 
                    $"After audio volume change, unexpected audio source '{MusicAudioSourceName}' volume");
                
                Assert.AreEqual(expectedFxVolume, _fxAudioSliderComponent.value, 0.0, 
                    $"After audio volume change, unexpected slider '{FxSliderName}' value");
                
                Assert.AreEqual(expectedFxVolume, _fxAudioSourceComponent.volume, 0.0, 
                    $"After audio volume change, unexpected audio source '{FxAudioSourceName}' volume");
            }
        }
        
        [UnityTest, Order(6), Description("Change audio sliders' values, then reload menu, the audio sources' volume must match")]
        public IEnumerator WhenChangeSliderValuesAndReloadSubMenu_ThenAudioSourcesVolumeLevelsMatch()
        {
            var expectedMusicVolume = 0.8f;
            var expectedFxVolume = 0.7f;

            _musicAudioSliderComponent.value = expectedMusicVolume;
            _fxAudioSliderComponent.value = expectedFxVolume;
            
            yield return null;
            
            // Reload the menu: destroys the audio menu and its child objects (slider)
            _audioButton.onClick.Invoke();
            
            yield return null;
            
            // Get newly created sliders
            _musicAudioSliderComponent = GetComponentInChildrenFromGameObjectWithName<Slider>(MusicSliderName);
            _fxAudioSliderComponent = GetComponentInChildrenFromGameObjectWithName<Slider>(FxSliderName);

            Assert.AreEqual(expectedMusicVolume, _musicAudioSliderComponent.value, 0.0, 
                $"After audio menu reload, unexpected slider '{MusicSliderName}' value");
            
            Assert.AreEqual(expectedMusicVolume, _musicAudioSourceComponent.volume, 0.0, 
                $"After audio menu reload, unexpected audio source '{MusicAudioSourceName}' volume");
            
            Assert.AreEqual(expectedFxVolume, _fxAudioSliderComponent.value, 0.0, 
                $"After audio menu reload, unexpected slider '{FxSliderName}' value");
            
            Assert.AreEqual(expectedFxVolume, _fxAudioSourceComponent.volume, 0.0, 
                $"After audio menu reload, unexpected audio source '{FxAudioSourceName}' volume");
        }
    }
}