using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Tests.Utilities.UtilityFunctions;

namespace Tests.EditModeTests.ContentValidation
{
    /// <summary>
    /// Runs content validation tests on build.
    /// When the build loads a specific scene all related content validation tests are started.
    /// The build process stops if any test should fail and a popup with all failed tests is shown.
    /// </summary>
    /// <remarks>
    /// To disable the tests during build, simply comment out the entire class :)
    /// </remarks>
    public class RunSceneValidationTestsDuringBuild : IProcessSceneWithReport
    {
        public int callbackOrder => 0;
        private readonly Regex _experimentNameRegex = new Regex(@"\w+\.(pc|vr)");

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            // Don't execute scene validation tests during playmode
            if (report == null)
                return;

            var scenePath = scene.path;

            // Call RunSceneTests delayed, otherwise Unity version 2022 crashes
            // Used to work with Unity 2021 and below
            EditorApplication.delayCall += () => { RunSceneTests(scenePath); };
        }

        public void RunSceneTests(string scenePath)
        {
            var experimentName = _experimentNameRegex.Match(scenePath).ToString();
            var sceneTestsGroupName = $"\"{experimentName}\".*";

            // Only test scenes in experiments folder
            if (!scenePath.Contains("experiments"))
                return;

            var results = new ResultCollector();
            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(results);

            api.Execute(new ExecutionSettings
            {
                runSynchronously = true,
                filters = new[]
                { new Filter
                    {
                        groupNames = new [] { sceneTestsGroupName },
                        testMode = TestMode.EditMode
                    }
                }
            });
            if (results != null && results.Result != null && results.Result.FailCount > 0)
            {
                ReportTestFailureWithPopup(results.Result);
                EditorApplication.ExecuteMenuItem("Window/General/Test Runner");
                throw new BuildFailedException($"{results.Result.FailCount} Scene Validation Tests failed! Check Test Runner window for more information");
            }

            if (results == null || results.Result == null)
            {
                Debug.Log($"{scenePath}: test ResultCollector.results is null.");
            }
            else
            {
                Debug.Log($"{scenePath}: {results.Result.PassCount} test{(results.Result.PassCount > 1 ? "s" : "")} passed ({results.Result.SkipCount} skipped).");
            }
        }

        private class ResultCollector : ICallbacks
        {
            public ITestResultAdaptor Result { get; private set; }

            public void RunStarted(ITestAdaptor testsToRun) { }
            public void TestStarted(ITestAdaptor test) { }
            public void TestFinished(ITestResultAdaptor result) { }

            public void RunFinished(ITestResultAdaptor result)
            {
                Result = result;
            }
        }
    }
}
