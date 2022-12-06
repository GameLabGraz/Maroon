using System.Text.RegularExpressions;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests.EditModeTests.ContentValidation
{
    /// <summary>
    /// Runs content validation tests on build.
    /// When the build loads a specific scene all related content validation tests are started.
    /// The build process stops if any test should fail.
    /// </summary>
    ///
    /// TODO test this! also with test runner closed to see if we need a "force refresh" similar to context menu
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
            var experimentName = _experimentNameRegex.Match(scenePath).ToString();
            var sceneTestsGroupName = $"\"{experimentName}\".*";

            // Only test scenes in experiments folder
            if (!scenePath.Contains("experiments"))
                return;
            
            var results = new ResultCollector("SceneValidationTests");
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
            
            if (results.Failed)
                throw new BuildFailedException($"{results.Result.FailCount} scene validation tests failed!");
            
            Debug.Log($"{results.Result.PassCount} scene validation tests passed for {scenePath} ({results.Result.SkipCount} skipped).");
        }
    
        private class ResultCollector : ICallbacks
        {
            public ResultCollector(string tag)
            {
                
            }
            public ITestResultAdaptor Result { get; private set; }
            public bool Failed { get; private set; }
    
            public void RunStarted(ITestAdaptor testsToRun) { }
            public void TestStarted(ITestAdaptor test) { }
            public void TestFinished(ITestResultAdaptor result) { }
            
            public void RunFinished(ITestResultAdaptor result)
            {
                Result = result;
                if (Result.FailCount > 0)
                {
                    Failed = true;
                }
            }
    
            
        }
    }
}
