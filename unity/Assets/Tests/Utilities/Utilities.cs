using System;
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
                    Debug.Log(path);
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
            Assert.NotNull(component, $"'{name}' prefab has no '{typeof(T)}' component - faulty test?");

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
        
        /*
         * Find any inactive GameObject by name
         * Adjusted from https://stackoverflow.com/a/44456334/7262963
         */
        public static GameObject FindObjectByName(string name)
        {
            GameObject found = null;
            
            foreach (var transform in Resources.FindObjectsOfTypeAll<Transform>())
            {
                if (transform.hideFlags == HideFlags.None && transform.name == name)
                {
                    found = transform.gameObject;
                }
            }

            Assert.NotNull(found, $"No GameObject named '{name}' found");
            return found;
        }
        
        /*
         * Find any active or inactive GameObject by tag
         * Assert its existence and active state
         * Adjusted from https://stackoverflow.com/a/44456334/7262963
         */
        public static GameObject FindObjectByTag(string tag)
        {
            GameObject found = null;
            
            foreach (var transform in Resources.FindObjectsOfTypeAll<Transform>())
            {
                if (transform.hideFlags == HideFlags.None && transform.CompareTag(tag))
                {
                    found = transform.gameObject;
                }
            }
            
            Assert.NotNull(found, $"No GameObject tagged '{tag}' found");
            return found;
        }

        public static void ValidateGameObject(GameObject gameObject)
        {
            Debug.Log(gameObject.name);
            Debug.Log(gameObject.activeInHierarchy); // TODO FIXME why is it not active in hierarchy? problem while running in editor mode?
            Debug.Log(gameObject.activeSelf);
            Assert.True(gameObject.activeSelf, $"The GameObject '{gameObject.name}' should be active (activeSelf)");
            Assert.True(gameObject.activeInHierarchy, $"The GameObject '{gameObject.name}' should be active (activeInHierarchy)");
        }
        
        /*
         * Get a Behaviour:Component from a GameObject
         * Assert its existence and whether it's enabled
         */
        public static T GetAndValidateBehaviourFromGameObject<T>(GameObject gameObject) where T:Behaviour {
            var component = gameObject.GetComponent<T>();
            
            Assert.NotNull(component, $"No '{nameof(T)}' component in GameObject '{gameObject.name}'");
            Assert.True(component.enabled, $"The '{nameof(T)}' component of '{gameObject.name}' should be enabled");

            return component;
        }
    }
}
