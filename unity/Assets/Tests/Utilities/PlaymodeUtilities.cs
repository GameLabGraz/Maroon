using System;
using System.Collections;
using GEAR.Localization;
using UnityEngine;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using static Tests.Utilities.UtilityFunctions;
using Object = UnityEngine.Object;

namespace Tests.Utilities
{
    public static class PlaymodeUtilities
    {
        /// <summary>
        /// Loads the scene from given path and checks it's actually loaded after skipping a frame
        /// </summary>
        /// <param name="pathOfSceneToLoad">relative file path ('Assets/...') of scene to load</param>
        /// <returns>yields an IEnumerator to skip a frame</returns>
        /// <remarks>Use only in playmode tests</remarks>
        public static IEnumerator LoadSceneAndCheckItsLoadedCorrectly(string pathOfSceneToLoad)
        {
            // Start from Main Menu for every following test
            EditorSceneManager.LoadScene(pathOfSceneToLoad,
                new LoadSceneParameters(LoadSceneMode.Single));
            yield return null;
            var currentSceneName = SceneManager.GetActiveScene().path;
            Assert.AreEqual(pathOfSceneToLoad, currentSceneName, $"'{pathOfSceneToLoad}' scene was not loaded");
        }

        /// <summary>
        /// Destroy DontDestroyOnLoad'ed objects to prevent tests possibly affecting each other
        /// </summary>
        /// <remarks>Use only in playmode tests</remarks>
        public static void DestroyPermanentObjects()
        {
            Object.Destroy(FindObjectByName("LanguageManager"));
            Object.Destroy(FindObjectByName("GlobalEntities"));
        }

        /// <summary>
        /// Find and click Menu's Language button
        /// </summary>
        /// <remarks>Use only in playmode tests</remarks>
        public static IEnumerator ClickButtonByLanguageManagerKey(string buttonLmKey, SystemLanguage language)
        {
            string languageButtonLabel = LanguageManager.Instance.GetString(buttonLmKey, language);
            GetButtonViaTextLabel(languageButtonLabel).onClick.Invoke();
            yield return null;
        }

        /// <summary>
        /// Use this to wait until a condition is met or timeout happens
        /// </summary>
        /// <remarks>Use only in playmode tests<br/>
        /// Source: https://forum.unity.com/threads/coroutine-question-can-you-combine-waitforseconds-with-a-condition.107041/#post-6241319</remarks>
        public sealed class WaitUntilConditionOrTimeout : CustomYieldInstruction
        {
            private Func<bool> m_Predicate;
            private float m_timeout;

            private bool WaitForDoneProcess()
            {
                m_timeout -= Time.deltaTime;
                if (m_timeout <= 0f)
                    Debug.LogError("Timeout reached!");
                return m_timeout <= 0f || m_Predicate();
            }

            public override bool keepWaiting => !WaitForDoneProcess();

            public WaitUntilConditionOrTimeout(float timeout, Func<bool> predicate)
            {
                m_Predicate = predicate;
                m_timeout = timeout;
            }
        }
    }
}
