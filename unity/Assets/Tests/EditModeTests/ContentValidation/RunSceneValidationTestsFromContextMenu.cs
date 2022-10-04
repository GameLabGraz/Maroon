using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.TestTools.TestRunner;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests.EditModeTests.ContentValidation
{
    [InitializeOnLoad]
    public class RunSceneValidationTestsFromContextMenu : ScriptableObject, ICallbacks 
    {
        static RunSceneValidationTestsFromContextMenu()
        {
            // Pass scene as context
            SceneHierarchyHooks.addItemsToSceneHeaderContextMenu += (menu, scene) =>
            {
                // Only add to context menu for experiment scenes
                if (scene.path.Contains("experiments"))
                    menu.AddItem(new GUIContent("Run validation tests"), false, DoRunTests, scene);
            };
        }

        static void DoRunTests(object userData)
        {
            CreateInstance<RunSceneValidationTestsFromContextMenu>()
                .StartTestRun((Scene) userData);
        }

        private static TestRunnerApi _runnerApi;
        private static TestRunnerApi RunnerApi =>
            _runnerApi ? _runnerApi : (_runnerApi = CreateInstance<TestRunnerApi>());
        
        private readonly Regex _experimentNameRegex = new Regex(@"\w+\.(pc|vr)");

        private void StartTestRun(Scene scene)
        {
            Debug.Log($"Starting test run for scene {scene.path}");
            
            // Not unloaded by Resources.UnloadUnusedAssets, will be destroyed in RunFinished callback
            hideFlags = HideFlags.HideAndDontSave;
            
            // Test results don't show up after re-compile when no Test Runner window is opened - force refresh
            if (!EditorWindow.HasOpenInstances<TestRunnerWindow>())
            {
                EditorApplication.ExecuteMenuItem("Window/General/Test Runner");
                if (EditorWindow.HasOpenInstances<TestRunnerWindow>())
                    EditorWindow.GetWindow<TestRunnerWindow>().Close();
            }

            // filter by test group name
            var scenePath = scene.path;
            var experimentName = _experimentNameRegex.Match(scenePath).ToString();
            var sceneTestsGroupName = $"Tests\\.ContentValidation\\.(Pc|Vr)SceneValidationTests\\(\"{experimentName}\",.*";

            RunnerApi.Execute(new ExecutionSettings
            {
                filters = new []
                {
                    new Filter()
                    {
                        groupNames = new[] { sceneTestsGroupName },
                        testMode = TestMode.EditMode
                    }
                }
            });
        }

        public void OnEnable()
        {
            RunnerApi.RegisterCallbacks(this);
        }

        public void OnDisable()
        {
            RunnerApi.UnregisterCallbacks(this);
        }

        public void RunStarted(ITestAdaptor testsToRun) { }

        public void TestStarted(ITestAdaptor test) { }

        public void TestFinished(ITestResultAdaptor result) { }

        public void RunFinished(ITestResultAdaptor result)
        {
            var title = "Validation test result";
            
            if (!result.HasChildren)
            {
                EditorUtility.DisplayDialog(title, "No tests found.", "Ok");
            }
            else if (result.FailCount == 0)
            {
                EditorUtility.DisplayDialog(title, $"All {result.PassCount} validation tests passed.", "Ok");
            }
            else
            {
                IEnumerable<string> GetFailedTestNames(ITestResultAdaptor test)
                {
                    if (test.HasChildren)
                        return test.Children.SelectMany(GetFailedTestNames);

                    return test.TestStatus == TestStatus.Failed ? new[] { test.Name } : Array.Empty<string>();
                }

                var failedTestNames = string.Join("\n", GetFailedTestNames(result).Select(t => $"\t{t}"));
                EditorUtility.DisplayDialog(title, $"{result.FailCount} validation tests failed:\n{failedTestNames}", "Ok");
                
                EditorApplication.ExecuteMenuItem("Window/General/Test Runner");
            }

            DestroyImmediate(this);
        }
    }
}
