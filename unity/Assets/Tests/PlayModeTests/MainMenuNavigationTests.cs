using System.Collections;
using System.Linq;
using GEAR.Localization;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

/*
 * Inspired by https://forum.unity.com/threads/play-mode-tests-scenehandling.751049/
 *
 * "End to end" testing
 */

namespace Tests.PlayModeTests
{
    using static PlaymodeTestUtils;
    public class MainMenuPcNavigationTests // TODO change name to MainMenuPcTests (didnt do it on laptop, recompiling takes forever)
    {
        private const string MainMenuScenePath = "Assets/Maroon/scenes/special/MainMenu.pc.unity";
        private const string PrefabsColumnsPath = "Assets/Maroon/reusableGui/Menu/PrefabsColumns/";
        private const SystemLanguage DefaultLanguage = SystemLanguage.English;

        private const string EnterLabLabel = "Menu Lab";

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
        
        public static readonly ButtonLabelMatchingMenuColumnSource[] TopLevelMenuPaths =
        {
            new ButtonLabelMatchingMenuColumnSource(EnterLabLabel, "preMenuColumnLaboratorySelection.prefab"),
            new ButtonLabelMatchingMenuColumnSource("Menu Audio", "preMenuColumnAudio.prefab"),
            new ButtonLabelMatchingMenuColumnSource("Menu Language", "preMenuColumnLanguage.prefab"),
            new ButtonLabelMatchingMenuColumnSource("Menu Credits", "preMenuColumnCredits.prefab")
        };
        
        [UnityTest]
        public IEnumerator WhenClickTopLevelMenuItemThenOpenIt([ValueSource(nameof(TopLevelMenuPaths))]ButtonLabelMatchingMenuColumnSource source)
        {
            string buttonLabel = LanguageManager.Instance.GetString(source.LanguageManagerButtonLabel);
            string expectedMenuColumn = source.ExpectedMenuColumn;
            Debug.Log($"expectedMenuColumn: {expectedMenuColumn}");
            
            // Click labeled button
            GetButtonViaText(buttonLabel).onClick.Invoke();
            yield return null;

            // Check correct menu has appeared
            var menuColumn = GameObject.Find(expectedMenuColumn);
            Assert.NotNull(menuColumn, $"Could not find '{buttonLabel}' menu Gameobject '{expectedMenuColumn}'");
        }

        // TODO Test "selected button" idea:
        // draft: check if select icon activates on button click (small square with arrow inside)
        // 1. preliminary check that no button has the icon active
        // 2. then check clicked button if icon is active
        
        // TODO Try to use PcScenesProvider instead! edit: makes no sense, as its missing categories :/
        // Can I access the menu somehow? Probably not since the logic is stored in Maroon scripts that arent accessible from Playmode tests
        public static readonly LabMenuPathSource[] LaboratoryMenuPaths =
        {
            new LabMenuPathSource("Physics", "CathodeRayTube"),
            new LabMenuPathSource("Physics", "CoulombsLaw"),
            new LabMenuPathSource("Physics", "FallingCoil"),
            new LabMenuPathSource("Physics", "FaradaysLaw"),
            new LabMenuPathSource("Physics", "HuygensPrinciple"),
            new LabMenuPathSource("Physics", "PointWaveExperiment"),
            new LabMenuPathSource("Physics", "Pendulum"),
            new LabMenuPathSource("Physics", "VandeGraaffBalloon"),
            new LabMenuPathSource("Physics", "VandeGraaffGenerator"),
            new LabMenuPathSource("Physics", "Optics"),
            new LabMenuPathSource("Physics", "3DMotionSimulation"),
            new LabMenuPathSource("Physics", "Whiteboard"),
            new LabMenuPathSource("Chemistry", "TitrationExperiment"),
            new LabMenuPathSource("Computer Science", "Sorting")
        };
        
        [UnityTest]
        public IEnumerator WhenClickLabCategoryExperimentThenLoadScene([ValueSource(nameof(LaboratoryMenuPaths))] LabMenuPathSource source)
        {
            string labsButtonLabel = LanguageManager.Instance.GetString(EnterLabLabel);
            string categoryButtonLabel = source.Category;
            string experimentButtonLabel = source.Experiment;
            string sceneName = experimentButtonLabel + ".pc";

            GetButtonViaText(labsButtonLabel).onClick.Invoke();
            yield return null;
            
            GetButtonViaText(categoryButtonLabel).onClick.Invoke();
            yield return null;
            
            GetButtonViaText(experimentButtonLabel).onClick.Invoke();
            yield return null;
            
            var currentSceneName = SceneManager.GetActiveScene().name;
            Assert.AreEqual(sceneName, currentSceneName, $"Scene '{sceneName}' did not load");
        }

        
        public readonly struct LabMenuPathSource
        {
            public LabMenuPathSource(string category, string experiment)
            {
                Category = category;
                Experiment = experiment;
            }

            public string Category { get; }
            public string Experiment { get; }

            public override string ToString() => $"{Category} -> {Experiment}";
        }
        
        public readonly struct ButtonLabelMatchingMenuColumnSource
        {
            public ButtonLabelMatchingMenuColumnSource(string languageManagerButtonLabel, string expectedMenuColumn)
            {
                LanguageManagerButtonLabel = languageManagerButtonLabel;
                ExpectedMenuColumn = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabsColumnsPath + expectedMenuColumn).name + "(Clone)";
            }

            public string LanguageManagerButtonLabel { get; }
            public string ExpectedMenuColumn { get; }

            public override string ToString() => $"{LanguageManagerButtonLabel}";
        }
    }
}
