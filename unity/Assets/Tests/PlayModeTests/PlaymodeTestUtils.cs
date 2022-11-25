using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Tests.PlayModeTests
{
    public static class PlaymodeTestUtils
    {
        private static string MainMenuScenePath = "Assets/Maroon/scenes/special/MainMenu.pc.unity";
        private static string FallingCoilScenePath = "Assets/Maroon/scenes/experiments/FallingCoil/FallingCoil.pc.unity";
        
        /*
         * Loads the main menu and checks if it's the active scene
         */
        public static IEnumerator LoadMainMenuScene()
        {
            // Start from Main Menu for every following test
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(MainMenuScenePath, new LoadSceneParameters(LoadSceneMode.Single));

            var currentSceneName = SceneManager.GetActiveScene().name;
            Assert.AreEqual("MainMenu.pc", currentSceneName, "'MainMenu.pc' scene did not load");
        }
        
        /*
         * Loads experiment scene Falling Coil and checks if it's the active scene
         */
        public static IEnumerator LoadFallingCoilScene()
        {
            // Start from Main Menu for every following test
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(FallingCoilScenePath, new LoadSceneParameters(LoadSceneMode.Single));

            var currentSceneName = SceneManager.GetActiveScene().name;
            Assert.AreEqual("FallingCoil.pc", currentSceneName, "'FallingCoil.pc' scene did not load");
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
        public static GameObject FindInActiveGameObjectByName(string name)
        {
            Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
            foreach (var obj in objs)
            {
                if (obj.hideFlags == HideFlags.None && obj.name == name)
                {
                    return obj.gameObject;
                }
            }
            return null;
        }
    }
}
