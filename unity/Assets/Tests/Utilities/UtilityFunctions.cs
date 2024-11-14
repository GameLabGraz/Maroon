using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;
using UnityEditor.TestTools.TestRunner.Api;
using static Tests.Utilities.Constants;

namespace Tests.Utilities
{
    /// <summary>
    /// Collection of test related utility functions 
    /// </summary>
    public static class UtilityFunctions
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
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new []{"Assets/Maroon"}))
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
        /// Wrapper of GetComponent specifically for use with Prefabs.
        /// </summary>
        /// <param name="prefab">The prefab to get the Component from</param>
        /// <typeparam name="T">Type of Component to get</typeparam>
        /// <returns>Component of Type T</returns>
        /// <remarks>triggers a test failure if no Component was found</remarks>
        public static T GetComponentFromPrefab<T>(GameObject prefab)
        {
            string name = prefab.gameObject.name;
            var component = prefab.GetComponent<T>();
            Assert.NotNull(component, $"Prefab '{name}' has no '{typeof(T).Name}' component - faulty test?");

            return component;
        }

        /// <summary>
        /// Get a <see cref="Button"/> component from GameObject where a child has a matching TextMeshPro label with buttonText.
        /// Useful for clicking a button in menus for test cases.
        /// </summary>
        /// <param name="buttonTextLabel">The Button to get</param>
        /// <param name="comparisonMethod">Optional (Default: String.Equals) - string comparison method for button text matching</param>
        /// <returns>the Button with given buttonText</returns>
        /// <remarks>triggers a test failure if no or more than one button was found</remarks>
        public static Button GetButtonViaTextLabel(string buttonTextLabel, Func<string, string, bool> comparisonMethod=null)
        {
            Button buttonToReturn = null;
            comparisonMethod ??= String.Equals;

            var buttonGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            
            foreach (var buttonGameObject in buttonGameObjects)
            {
                var buttonComponent = buttonGameObject.GetComponent<Button>();
                var textComponent = buttonGameObject.GetComponentInChildren<TextMeshProUGUI>();
                
                if (buttonComponent && textComponent && comparisonMethod(textComponent.text,buttonTextLabel))
                {
                    if (buttonToReturn != null)
                    {
                        Assert.Fail($"Found more than one Button with Text '{buttonTextLabel}'");
                    }
                    buttonToReturn = buttonGameObject.GetComponent<Button>();
                }
            }
            
            Assert.NotNull(buttonToReturn, $"Could not find any Button with Text '{buttonTextLabel}'");
            
            return buttonToReturn;
        }

        // public static bool ContainsWrapper(string a, string b) => return a.Contains(b);

        /// <summary>
        /// Get any active or inactive GameObject by name
        /// </summary>
        /// <param name="name">name of GameObject to find</param>
        /// <returns>the found GameObject</returns>
        /// <remarks>triggers a test failure if anything goes wrong, e.g. wrong number of GameObjects found</remarks>
        public static GameObject FindObjectByName(string name)
        {
            var namedObjectsInScene = new List<GameObject>();

            foreach (var go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (!EditorUtility.IsPersistent(go.transform.root.gameObject) &&
                    !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave) &&
                    go.name.Equals(name))
                    namedObjectsInScene.Add(go);
            }
            Assert.True(namedObjectsInScene.Count > 0, $"Could not find gameObject named '{name}'");
            Assert.AreEqual(1, namedObjectsInScene.Count, $"Found multiple ({namedObjectsInScene.Count}) gameObjects named '{name}'");

            return namedObjectsInScene.First();
        }

        /// <summary>
        /// Triggers a test failure if <see cref="gameObject"/> or any of its parent(s) is inactive
        /// </summary>
        /// <param name="gameObject"></param>
        public static void AssertGameObjectIsActive(GameObject gameObject)
        {
            Assert.True(gameObject.activeSelf, $"The GameObject '{gameObject.name}' should be active (activeSelf)");
            Assert.True(gameObject.activeInHierarchy, $"The GameObject '{gameObject.name}' should be active (activeInHierarchy)");
        }
        
        /// <summary>
        /// Wrapper around <see cref="GetComponent"/> with narrowed type options and assertions.
        /// Gets a <see cref="Behaviour"/> from a GameObject, assert its existence and checks whether it's enabled.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <typeparam name="T">the type of <see cref="Behaviour"/> to retrieve</typeparam>
        /// <returns>the component of Type <see cref="T"/> matching the criteria</returns>
        /// <remarks>triggers a test failure if anything goes wrong, e.g. component is not found or disabled</remarks>
        public static T GetEnabledComponentFromGameObject<T>(GameObject gameObject) where T:Behaviour {
            var component = gameObject.GetComponent<T>();
            
            Assert.NotNull(component, $"No '{nameof(T)}' component in GameObject '{gameObject.name}'");
            Assert.True(component.enabled, $"The '{nameof(T)}' component of '{gameObject.name}' should be enabled");

            return component;
        }
        
        /// <summary>
        /// Gets a <see cref="Component"/> of type T from GameObject with name gameObjectName or any of its children 
        /// </summary>
        /// <param name="gameObjectName">name of the GameObject</param>
        /// <typeparam name="T">the type of <see cref="Component"/> to retrieve</typeparam>
        /// <returns>the first found component of Type <see cref="T"/></returns>
        /// <remarks>triggers a test failure if anything goes wrong, e.g. GameObject not found</remarks>
        public static T GetComponentFromGameObjectOrItsChildrenByName<T>(string gameObjectName) where T:Component
        {
            var gameObject = GameObject.Find(gameObjectName);
            Assert.NotNull(gameObject, $"Could not find '{gameObjectName}' GameObject");
            
            var component = gameObject.GetComponent<T>() ?? gameObject.GetComponentInChildren<T>();
            Assert.NotNull(component, $"Could not find '{typeof(T).Name}' component in GameObject '{gameObject.name}' or any of its children");

            return component;
        }

        /// <summary>
        /// Gets all child GameObjects from a given GameObject up to a specified hierarchy depth 
        /// </summary>
        /// <param name="targetGo">target GameObject to get children from</param>
        /// <param name="maxHierarchyDepth">the max depth to move through the GameObject's child hierarchy</param>
        /// <returns>GameObject array filled with all GameObjects that are part of the GameObject's hierarchy except the root</returns>
        public static GameObject[] GetChildrenFromGameObject(GameObject targetGo, int maxHierarchyDepth)
        {
            var gameObjectsFromPrefab = new List<GameObject>();
            AddDescendantsUntilDepth(targetGo.transform, gameObjectsFromPrefab, maxHierarchyDepth);
            
            return gameObjectsFromPrefab.ToArray();
        }
        
        /// <summary>
        /// Recursion helper method for <see cref="GetChildrenFromGameObject"/>. It moves through the object hierarchy
        /// until either <see cref="maxDepth"/> is reached or all children are added to the provided list
        /// </summary>
        /// <param name="parent">the parent GameObject, e.g. the prefab</param>
        /// <param name="list">all found child GameObjects are added to this list</param>
        /// <param name="maxDepth">the max depth to move through the prefab's object hierarchy</param>
        /// <param name="depth">the starting depth, defaults to 1</param>
        private static void AddDescendantsUntilDepth(Transform parent, ICollection<GameObject> list, int maxDepth, int depth=1)
        {
            if (depth > maxDepth)
                return;
            
            foreach (Transform child in parent)
            {
                if (!child.gameObject.name.Contains("==="))
                    list.Add(child.gameObject);
                AddDescendantsUntilDepth(child, list, maxDepth, depth+1);
            }
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

        /// <summary>
        /// Gets the path of the GameObject in the scene hierarchy.
        /// </summary>
        /// <returns>The path to the gameObject</returns>
        /// based on https://discussions.unity.com/t/how-can-i-get-the-full-path-to-a-gameobject/412/2
        public static string GetScenePath(this GameObject gameObject)
        {
            char delimiter = '/';
            string path = delimiter + gameObject.name;
            while (gameObject.transform.parent != null)
            {
                gameObject = gameObject.transform.parent.gameObject;
                path = delimiter + gameObject.name + path;
            }
            return path;
        }
    }
}
