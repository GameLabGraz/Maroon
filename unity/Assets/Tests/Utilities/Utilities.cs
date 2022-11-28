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

        public static T GetComponentFromPrefab<T>(GameObject prefab)
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
            List<GameObject> taggedObjectsInScene = new List<GameObject>();

            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (!EditorUtility.IsPersistent(go.transform.root.gameObject) &&
                    !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave) &&
                    go.name.Equals(name))
                    taggedObjectsInScene.Add(go);
            }
            Assert.AreEqual(1, taggedObjectsInScene.Count, $"Found multiple ({taggedObjectsInScene.Count}) gameObjects named '{name}'");

            GameObject found = taggedObjectsInScene.First();
            Assert.NotNull(found, $"No GameObject named '{name}' found");
            
            return found;
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
        public static T GetAndValidateComponentFromGameObject<T>(GameObject gameObject) where T:Behaviour {
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
    }
}
