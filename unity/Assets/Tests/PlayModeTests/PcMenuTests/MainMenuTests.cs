using System.Collections;
using GEAR.Localization;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEditor;
using static Tests.Utilities.Constants;
using static Tests.Utilities.PlaymodeUtilities;
using static Tests.Utilities.UtilityFunctions;

/*
 * Inspired by https://forum.unity.com/threads/play-mode-tests-scenehandling.751049/
 *
 * "End to end" testing
 */

namespace Tests.PlayModeTests.PcMenuTests
{
    /// <summary>
    /// Tests Main Menu functionality by navigating and loading laboratories and experiments 
    /// </summary>
    public class MainMenuTests
    {
        private const string EnterLabLabel = "Menu Lab";

        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return LoadSceneAndCheckItsLoadedCorrectly(MainMenuScenePath);
        }
        
        [TearDown]
        public void TearDown()
        {
            DestroyPermanentObjects();
        }
        
        // Manually matched menu labels and prefabs to click through and open submenus
        // Auto-generated combinations would require access of the necessary data from a static context outside of the test fixture
        private static readonly ButtonLabelMatchingMenuColumnSource[] TopLevelMenuPaths =
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
            GetButtonViaTextLabel(buttonLabel).onClick.Invoke();
            yield return null;

            // Check correct menu has appeared
            var menuColumn = GameObject.Find(expectedMenuColumn);
            Assert.NotNull(menuColumn, $"SubMenu GameObject '{expectedMenuColumn}' did not appear after clicking Button '{buttonLabel}'");
        }

        // Manually defined menu "paths" to click through and load lab or experiment scenes
        // Auto-generated paths would require access of the necessary data from a static context outside of the test fixture
        private static readonly LabMenuPathSource[] ExperimentMenuPaths =
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
            new LabMenuPathSource("Computer Science", "Sorting"),
            new LabMenuPathSource("Computer Science", "StateMachine"),
            new LabMenuPathSource("Computer Science", "PathFinding")
        };
        
        [UnityTest, Description("Clicking through 'Enter Lab -> Category -> Experiment' must load the associated scene")]
        [Timeout(1800000)]
        public IEnumerator WhenClickLabCategoryExperimentThenLoadScene([ValueSource(nameof(ExperimentMenuPaths))] LabMenuPathSource source)
        {
            // Get correct buttonLabel from LanguageManager
            string labsButtonLabel = LanguageManager.Instance.GetString(EnterLabLabel);
            string categoryButtonLabel = source.Category;
            string experimentButtonLabel = source.Experiment;
            string sceneName = experimentButtonLabel + ".pc";

            GetButtonViaTextLabel(labsButtonLabel).onClick.Invoke();
            yield return null;
            
            GetButtonViaTextLabel(categoryButtonLabel).onClick.Invoke();
            yield return null;
            
            GetButtonViaTextLabel(experimentButtonLabel).onClick.Invoke();
            yield return null;
            
            var currentSceneName = SceneManager.GetActiveScene().name;
            Assert.AreEqual(sceneName, currentSceneName, $"Scene '{sceneName}' did not load after clicking Buttons '{source.ToString()}'");
        }
        
        // Manually defined menu "paths" to click through and load lab or experiment scenes
        // Auto-generated paths would require access of the necessary data from a static context outside of the test fixture
        private static readonly LabMenuPathSource[] LabMenuPaths =
        {
            new LabMenuPathSource("Physics", "Lab"),
            new LabMenuPathSource("Chemistry", "Lab"),
            new LabMenuPathSource("Computer Science", "Lab"),
        };
        
        [UnityTest, Description("Clicking through 'Enter Lab -> Category -> Go to ... Lab' must load the associated scene")]
        public IEnumerator WhenClickLabCategoryGoToLabThenLoadScene([ValueSource(nameof(LabMenuPaths))] LabMenuPathSource source)
        {
            // Get correct buttonLabel from LanguageManager
            string labsButtonLabel = LanguageManager.Instance.GetString(EnterLabLabel);
            string categoryButtonLabel = source.Category;
            string experimentButtonLabel = $"{categoryButtonLabel} {source.Experiment}";
            string sceneName = "Laboratory.pc";
            
            GetButtonViaTextLabel(labsButtonLabel).onClick.Invoke();
            yield return null;
            
            GetButtonViaTextLabel(categoryButtonLabel).onClick.Invoke();
            yield return null;
            
            // Get button that contains experimentButtonLabel (instead of fully matching it)
            GetButtonViaTextLabel(experimentButtonLabel, (a, b) => a.Contains(b)).onClick.Invoke();
            yield return null;
            
            var currentSceneName = SceneManager.GetActiveScene().name;
            Assert.AreEqual(sceneName, currentSceneName, $"Scene '{sceneName}' did not load after clicking Buttons '{source.ToString()}'");
        }
        
        /// <summary>
        /// A data source for test case <see cref="WhenClickLabCategoryExperimentThenLoadScene"/>
        /// used together with (for <see cref="ValueSourceAttribute"/>).
        /// Provides Lab paths in Main Menu to click through: Category -> Experiment
        /// </summary>
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
        
        /// <summary>
        /// A data source for test case <see cref="WhenClickTopLevelMenuItemThenSubmenuOpens"/>
        /// used together with (for <see cref="ValueSourceAttribute"/>).
        /// Provides toplevel Main Menu buttons to navigate through
        /// </summary>
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
