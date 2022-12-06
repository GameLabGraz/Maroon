using System.Collections;
using GEAR.Localization;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static Tests.Utilities.Constants;

/*
 * Inspired by https://forum.unity.com/threads/play-mode-tests-scenehandling.751049/
 *
 * "End to end" testing
 */

// TODO move utility functions to Utilities and/or use those already there

namespace Tests.PlayModeTests.MenuTests
{
    using static PlaymodeTestUtils;
    public class MainMenuPcTests
    {
        private const string EnterLabLabel = "Menu Lab";

        [UnitySetUp]
        public IEnumerator Setup()
        {
            // Start from Main Menu for every following test
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(MainMenuScenePath, new LoadSceneParameters(LoadSceneMode.Single));

            var currentSceneName = SceneManager.GetActiveScene().name;
            Assert.AreEqual("MainMenu.pc", currentSceneName, "'MainMenu.pc' scene did not load");
        }
        
        // Manually matched menu labels and prefabs to click through and open submenus
        // Auto-generated combinations would require access of the necessary data from a static context outside of the test fixture
        public static readonly ButtonLabelMatchingMenuColumnSource[] TopLevelMenuPaths =
        {
            new ButtonLabelMatchingMenuColumnSource(EnterLabLabel, "preMenuColumnLaboratorySelection.prefab"),
            new ButtonLabelMatchingMenuColumnSource("Menu Audio", "preMenuColumnAudio.prefab"),
            new ButtonLabelMatchingMenuColumnSource("Menu Language", "preMenuColumnLanguage.prefab"),
            new ButtonLabelMatchingMenuColumnSource("Menu Credits", "preMenuColumnCredits.prefab")
        };
        
        [UnityTest, Description("Clicking on the top-level Main Menu entries must open the matching SubMenu")]
        public IEnumerator WhenClickTopLevelMenuItemThenSubmenuOpens([ValueSource(nameof(TopLevelMenuPaths))]ButtonLabelMatchingMenuColumnSource source)
        {
            // Get correct buttonLabel from LanguageManager
            string buttonLabel = LanguageManager.Instance.GetString(source.LanguageManagerButtonLabel);
            string expectedMenuColumn = source.ExpectedMenuColumn;

            // Click labeled button
            GetButtonViaText(buttonLabel).onClick.Invoke();
            yield return null;

            // Check correct menu has appeared
            var menuColumn = GameObject.Find(expectedMenuColumn);
            Assert.NotNull(menuColumn, $"SubMenu GameObject '{expectedMenuColumn}' did not appear after clicking Button '{buttonLabel}'");
        }

        // Manually defined menu "paths" to click through and load experiment scenes
        // Auto-generated paths would require access of the necessary data from a static context outside of the test fixture
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
        
        [UnityTest, Description("Clicking through 'Enter Lab -> Category -> Experiment' must load the associated experiment scene")]
        public IEnumerator WhenClickLabCategoryExperimentThenLoadScene([ValueSource(nameof(LaboratoryMenuPaths))] LabMenuPathSource source)
        {
            // Get correct buttonLabel from LanguageManager
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
            Assert.AreEqual(sceneName, currentSceneName, $"Scene '{sceneName}' did not load after clicking Buttons '{source.ToString()}'");
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
