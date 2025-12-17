using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using static Tests.Utilities.Constants;

namespace Tests.Utilities.Editor
{
    /// <summary>
    /// Collection of test related utility functions 
    /// </summary>
    public static class EditorUtilityFunctions
    {
        /// <summary>
        /// Get a prefab by name from anywhere in 'Assets/Maroon/'
        /// </summary>
        /// <param name="name">name of the Prefab</param>
        /// <returns>the first prefab found with the given name</returns>
        /// <remarks>triggers a test failure if no prefab of given name was found</remarks>
        public static GameObject GetPrefabByName(string name)
        {
            // Look for prefab's filesystem path
            Regex regex = new Regex($"/{name}\\.prefab");
            string path = "";
            bool found = false;
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Maroon" }))
            {
                path = AssetDatabase.GUIDToAssetPath(guid);
                if (regex.IsMatch(path))
                {
                    found = true;
                    break;
                }
            }
            Assert.True(found, $"No prefab with name {name} found - faulty test?");

            // Load prefab from filesystem path
            var prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath(path);
            Assert.NotNull(prefab, $"No prefab in path '{path}' found - faulty test?");
            Assert.AreEqual(name, prefab.name, $"Expected name '{name}' doesn't match prefab name '{prefab.name}' - faulty test?");

            return prefab;
        }

        /// <summary>
        /// Recursion helper to retrieve all failed test names from TestRunner results
        /// </summary>
        /// <param name="result">the test results</param>
        /// <returns>string array containing all failed test names</returns>
        public static IEnumerable<string> GetFailedTestNames(ITestResultAdaptor result)
        {
            if (result.HasChildren)
                return result.Children.SelectMany(GetFailedTestNames);

            return result.TestStatus == TestStatus.Failed ? new[] { result.Name } : Array.Empty<string>();
        }

        /// <summary>
        /// Gathers all failed test names and displays them in a simple popup
        /// </summary>
        /// <param name="result">the test results</param>
        public static void ReportTestFailureWithPopup(ITestResultAdaptor result)
        {
            var failedTestNames = string.Join("\n", GetFailedTestNames(result).Select(t => $"\t• {t}"));
            EditorUtility.DisplayDialog(GuiPopupTitle, $"{result.FailCount} test{(result.FailCount > 1 ? "s" : "")} failed:\n{failedTestNames}\n\n" +
                                                       "Check Test Runner window for more information", "Ok");
        }
    }
}
