using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Tests.Utilities
{
    public static class UtilityFunctions
    {
        public static GameObject GetPrefabByName(string name)
        {
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
            
            var prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath(path);

            Assert.AreEqual(name, prefab.name, $"Expected name '{name}' doesn't match prefab name '{prefab.name}' - faulty test?");
            
            return prefab;
        }

        /**
         * Returns a component from a GameObject if it's not null otherwise an Assert fails
         */
        public static T GetComponentFromGameObject<T>(GameObject prefab)
        {
            string name = ((Func<GameObject, string>)((a) => a.name))(prefab);
            var component = prefab.GetComponent<T>();
            Assert.NotNull(component, $"'{name}' prefab has no '{typeof(T).Name}' component - faulty test?");

            return component;
        }

        /*
         * Returns a Button component matching the buttonText string
         */
        public static Button GetButtonViaText(string buttonText)
        {
            Button buttonToReturn = null;

            var buttonGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            
            foreach (var buttonGameObject in buttonGameObjects)
            {
                var buttonComponent = buttonGameObject.GetComponent<Button>();
                var textComponent = buttonGameObject.GetComponentInChildren<TextMeshProUGUI>();
                
                if (buttonComponent && textComponent && textComponent.text == buttonText)
                {
                    if (buttonToReturn != null)
                    {
                        Assert.Fail($"Found more than one Button with Text '{buttonText}'");
                    }
                    buttonToReturn = buttonGameObject.GetComponent<Button>();
                }
            }
            
            Assert.NotNull(buttonToReturn, $"Could not find any Button with Text '{buttonText}'");
            
            return buttonToReturn;
        }

        /*
         * Asserts existence of component type T in gameObjectName and returns it 
         */
        public static T GetComponentFromGameObjectWithName<T>(string gameObjectName)
        {
            var gameObject = GameObject.Find(gameObjectName);
            Assert.NotNull(gameObject, $"Could not find '{gameObjectName}' GameObject");
            
            var component = gameObject.GetComponent<T>();
            Assert.NotNull(component, $"Could not find '{typeof(T).Name}' component in GameObject '{gameObject.name}'");

            return component;
        }
        
        /*
         * Asserts existence of component type T in any of gameObjectName's children and returns it 
         */
        public static T GetComponentInChildrenFromGameObjectWithName<T>(string gameObjectName)
        {
            var gameObject = GameObject.Find(gameObjectName);
            Assert.NotNull(gameObject, $"Could not find '{gameObjectName}' GameObject");

            var component = gameObject.GetComponentInChildren<T>();
            Assert.NotNull(component, $"Could not find '{typeof(T).Name}' component in GameObject '{gameObject.name}'");

            return component;
        }
        
        /**
         * Find any (in-)active GameObject by name
         */
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
        
        /**
         * Find any (in-)active GameObject by tag
         */
        public static GameObject FindObjectByTag(string tag)
        {
            List<GameObject> taggedObjectsInScene = new List<GameObject>();

            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (!EditorUtility.IsPersistent(go.transform.root.gameObject) &&
                    !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave) &&
                    go.CompareTag(tag))
                    taggedObjectsInScene.Add(go);
            }
            Assert.AreEqual(1, taggedObjectsInScene.Count, $"Found multiple ({taggedObjectsInScene.Count}) gameObjects tagged '{tag}'");

            GameObject found = taggedObjectsInScene.First();
            Assert.NotNull(found, $"No GameObject tagged '{tag}' found");
            
            return found;
        }

        /**
         * Asserts that the GameObject and all of its parent GameObjects in scene hierarchy are active
         */
        public static void AssertGameObjectIsActive(GameObject gameObject)
        {
            Assert.True(gameObject.activeSelf, $"The GameObject '{gameObject.name}' should be active (activeSelf)");
            Assert.True(gameObject.activeInHierarchy, $"The GameObject '{gameObject.name}' should be active (activeInHierarchy)");
        }
        
        /*
         * Get a Component (Behaviour) from a GameObject
         * Assert its existence and whether it's enabled
         */
        public static T GetEnabledComponentFromGameObject<T>(GameObject gameObject) where T:Behaviour {
            var component = gameObject.GetComponent<T>();
            
            Assert.NotNull(component, $"No '{nameof(T)}' component in GameObject '{gameObject.name}'");
            Assert.True(component.enabled, $"The '{nameof(T)}' component of '{gameObject.name}' should be enabled");

            return component;
        }

        public static void LoadSceneIfNotYetLoaded(string scenePath)
        {
            // Load scene if necessary
            var scene = SceneManager.GetSceneAt(0);
            if (SceneManager.sceneCount > 1 || scene.path != scenePath)
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            }
        }

        /// <summary>
        /// Gets all child GameObjects from a given prefab name up to a specified hierarchy depth 
        /// </summary>
        /// <param name="prefabName">name of the prefab</param>
        /// <param name="maxHierarchyDepth">the max depth to move through the prefab's object hierarchy</param>
        /// <returns>GameObject array filled with all GameObjects that are part of the prefab except the root</returns>
        /// <remarks>triggers a test failure if anything goes wrong, e.g. prefab is not found</remarks>
        public static GameObject[] GetObjectsFromPrefabOfDepth(string prefabName, int maxHierarchyDepth)
        {
            var experimentSettingPrefab = GetPrefabByName(prefabName);
            var allGameObjectsFromPrefab = new List<GameObject>();
            AddDescendantsUntilDepth(experimentSettingPrefab.transform, allGameObjectsFromPrefab, maxHierarchyDepth);
            
            return allGameObjectsFromPrefab.ToArray();
        }
        
        /// <summary>
        /// Recursion helper method for <see cref="GetObjectsFromPrefabOfDepth"/>. It moves through the object hierarchy
        /// until either <see cref="maxDepth"/> is reached or all children are added to the provided list
        /// </summary>
        /// <param name="parent">the parent GameObject, e.g. the prefab</param>
        /// <param name="list">all found child GameObjects are added to this list</param>
        /// <param name="maxDepth">the max depth to move through the prefab's object hierarchy</param>
        /// <param name="depth">the starting depth, defaults to 1</param>
        private static void AddDescendantsUntilDepth(Transform parent, ICollection<GameObject> list, int maxDepth=int.MaxValue, int depth=1)
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
        /// Use on test methods, where multiple uses per test case are possible.
        /// </summary>
        /// <param name="experimentNames">One or more experiment names in a comma separated string with optional spaces</param>
        /// <param name="reasonToSkip">Explanation why test case is skipped for specified experiment</param>
        /// <example>
        /// <code>
        /// [SkipTestFor("FallingCoil, "CoulombsLaw", "we prefer darkness")]
        /// [Test, Description("...")]
        /// public void sceneHasLights() { ... }
        /// </code>
        /// results in the test case "sceneHasLights" being skipped for FallingCoil and CoulombsLaw with the TestRunner info displaying "we prefer darkness" 
        /// </example>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public sealed class SkipTestForScenesWithReasonAttribute : Attribute
        {
            public string[] ExperimentNames { get; }
            public string ReasonToSkip { get; }

            public SkipTestForScenesWithReasonAttribute(string experimentNames, string reasonToSkip) {
                ExperimentNames = experimentNames.Split(',').Select(experimentName => experimentName.Trim()).ToArray();
                ReasonToSkip = reasonToSkip;
            }
            
            /// <summary>
            /// Get all attached <c>SkipTestForScenesWithReason</c> attributes of an annotated test method
            /// </summary>
            /// <typeparam name="T">The annotated test method's test fixture class</typeparam>
            /// <param name="methodName">The annotated test method's name</param>
            /// <returns>All attached custom attribute objects in an array</returns>
            public static SkipTestForScenesWithReasonAttribute[] GetAttributeCustom<T>(string methodName) where T : class
            {
                try
                {
                    return (SkipTestForScenesWithReasonAttribute[])typeof(T).GetMethod(methodName).GetCustomAttributes(typeof(SkipTestForScenesWithReasonAttribute), false);
                }
                catch(SystemException)
                {
                    return null;
                }
            }
        }
        
        /// <summary>
        /// Checks if either is true and the test case should be skipped:
        /// <list type="number">
        /// <item>
        /// <description>The test case is annotated with [<see cref="SkipTestForScenesWithReasonAttribute"/>] and the experiment name matches</description>
        /// </item>
        /// <item>
        /// <description>The <paramref name="nameOfObjectUnderTest"/> is not part of the experimentSettings prefab</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Don't call this directly, use a test fixture's associated wrapper method <c>SkipCheck</c> instead
        /// </remarks>
        /// <param name="sceneType">Type of scene: either PC or VR</param>
        /// <param name="objectNamesFromExperimentPrefab">Contains object names from experimentSetting prefab</param>
        /// <param name="currentExperimentName">Name of the current experiment under test</param>
        /// <param name="nameOfObjectUnderTest">Name of the object under test. It is matched against <paramref name="objectNamesFromExperimentPrefab"/></param>
        /// <param name="callingMethodName">The test method's name</param>
        public static void SkipCheckBase<T>(string sceneType, IEnumerable<string> objectNamesFromExperimentPrefab, string currentExperimentName,
            string nameOfObjectUnderTest, string callingMethodName) where T : class
        {
            // Get any attached attribute objects
            var skipTestForAttributes = SkipTestForScenesWithReasonAttribute.GetAttributeCustom<T>(callingMethodName);
            
            // Check if the provided scene(s) match the current test, if yes then skip test
            if (skipTestForAttributes != null)
            {
                foreach (var skipTestForAttribute in skipTestForAttributes)
                {
                    if (skipTestForAttribute.ExperimentNames.Any(x => currentExperimentName.ToUpper().Contains(x.ToUpper())))
                    {
                        Assert.Ignore($"Skipped test! Reason: {skipTestForAttribute.ReasonToSkip})");
                    }
                }
            }

            // Check if the object is part of our ExperimentSetting prefab otherwise skip test
            if (!objectNamesFromExperimentPrefab.Any(x => x.ToUpper().Contains(nameOfObjectUnderTest.ToUpper())))
            {
                Assert.Ignore($"{Constants.ExperimentPrefabName + sceneType} contains no {nameOfObjectUnderTest} - skipping test!");
            }
        }
    }
}
