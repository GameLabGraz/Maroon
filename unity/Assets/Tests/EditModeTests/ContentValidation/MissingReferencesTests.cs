using System.Collections.Generic;
using System.Linq;
using Fury.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Tests.EditModeTests.ContentValidation
{
    public class MissingReferencesTests
    {
        [Test, Description("Tests if there are missing references in experiment scenes'")]
        public void TestMissingReferencesInExperimentScenes()
        {
            var missingReferenceFinder = ScriptableObject.CreateInstance<MissingReferenceFinder>();
            var results = new Dictionary<EditorBuildSettingsScene, List<Result>>();
            
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if(!scene.path.Contains("experiments"))
                    continue;

                EditorSceneManager.OpenScene(scene.path);
                missingReferenceFinder.SearchCurrentScene();

                if(missingReferenceFinder.results.Count > 0)
                    results.Add(scene, new List<Result>(missingReferenceFinder.results));
            }
            
            Assert.AreEqual(0, results.Count(), "There are experiment scenes with missing references:\r\n" +
                                                string.Join(",\r\n", results.Select(result => $"{result.Key.path}")));
        }

        [Test, Description("Tests if there are missing references in prefabs'")]
        public void TestMissingReferencesInPrefabs()
        {
            var missingReferenceFinder = ScriptableObject.CreateInstance<MissingReferenceFinder>();

            missingReferenceFinder.SearchAssetFolder();
            Assert.AreEqual(0, missingReferenceFinder.results.Count, "There are prefabs with missing references:\r\n" +
                                                                     string.Join(",\r\n", missingReferenceFinder.results.Select(result => result.address)));
        }
    }
}
