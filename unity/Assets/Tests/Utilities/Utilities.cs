using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;

namespace Tests.Utilities
{
    public static class Utilities
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
        
        public static void AddDescendantsUntilDepth(Transform parent, List<GameObject> list, int maxDepth, int depth=1)
        {
            if (depth > maxDepth)
                return;
            
            foreach (Transform child in parent)
            {
                list.Add(child.gameObject);
                AddDescendantsUntilDepth(child, list, maxDepth, depth+1);
            }
        }
        
        /**
         * Custom attribute (decorator) for tests.
         * Use it on test methods.
         * Provide argument(s): any number of experiment names to skip the respective test for
         * e.g.: [SkipTestFor("FallingCoil"]
         */
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public sealed class SkipTestForScenesWithReason : Attribute
        {
            public string[] ExperimentNames { get; }
            public string ReasonToSkip { get; }

            public SkipTestForScenesWithReason(string experimentNames, string reasonToSkip) {
                ExperimentNames = experimentNames.Split(',').Select(experimentName => experimentName.Trim()).ToArray();
                ReasonToSkip = reasonToSkip;
            }

            /**
             * Returns attribute data as array of attributes
             */
            public static SkipTestForScenesWithReason[] GetAttributeCustom<T>(string methodName) where T : class
            {
                try
                {
                    return (SkipTestForScenesWithReason[])typeof(T).GetMethod(methodName).GetCustomAttributes(typeof(SkipTestForScenesWithReason), false);
                }
                catch(SystemException)
                {
                    return null;
                }
            }
        }
        
        /**
-         * Checks whether a test case should be skipped if either is true:
-         * 1.) The test case is decorated with the custom attribute [SkipTestFor("SomeExperimentName")] and the experiment matches
-         * 2.) The objectNameUnderTest is not part of the experiment prefab
          */
        public static void SkipCheckLong<T>(string experimentPrefabName, IEnumerable<string> objectNamesFromExperimentPrefab, string currentExperimentName,
            string nameOfObjectUnderTest, string callingMethodName) where T : class
        {
            // Get attribute
            var skipTestForAttributes = SkipTestForScenesWithReason.GetAttributeCustom<T>(callingMethodName);
            
            if (skipTestForAttributes != null)
            {
                // TODO iterate over all custom attributes, in each one check all contained experiment names and assert with provided reason!
                // TODO afterwards: adjust all skipped tests :)
                var concatenatedExperimentNames = new List<string>();
                foreach (var skipTestForAttribute in skipTestForAttributes)
                {
                    concatenatedExperimentNames = concatenatedExperimentNames.Concat(skipTestForAttribute.ExperimentNames).ToList();
                    // Check if the provided scene(s) match the current test, if yes then skip test
                    if (skipTestForAttribute.ExperimentNames.Any(x => currentExperimentName.ToUpper().Contains(x.ToUpper())))
                    {
                        Assert.Ignore($"Skipped test for '{nameOfObjectUnderTest}'! Reason: {skipTestForAttribute.ReasonToSkip})");
                    }
                }

                
            }

            // Check if the object is part of our ExperimentSetting prefab otherwise skip test
            if (!objectNamesFromExperimentPrefab.Any(x => x.ToUpper().Contains(nameOfObjectUnderTest.ToUpper())))
            {
                Assert.Ignore($"{experimentPrefabName} contains no {nameOfObjectUnderTest} - skipping test!");
            }
             
        }
    }
}
