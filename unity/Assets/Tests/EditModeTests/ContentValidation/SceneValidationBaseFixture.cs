using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using static Tests.Utilities.Constants;
using static Tests.Utilities.CustomAttributes;
using static Tests.Utilities.UtilityFunctions;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Events;

namespace Tests.EditModeTests.ContentValidation
{
    /// <summary>
    /// Base class for scene validation test fixtures.
    /// </summary>
    /// <typeparam name="Type">type of the derived class</typeparam>
    /// <example>
    /// <code>
    /// public sealed class PcSceneValidationTests : SceneValidationBaseFixture<PcSceneValidationTests> { ... }
    /// </code>
    /// </example>
    public class SceneValidationBaseFixture<Type> where Type : class
    {
        /// <summary>
        /// Name of the experiment scene to be tested
        /// </summary>
        private readonly string _experimentName;
        
        /// <summary>
        /// Relative path to scene starting from "Assets" folder
        /// </summary>
        private readonly string _scenePath;
        
        /// <summary>
        /// Name of the ExperimentSetting prefab (room template)
        /// </summary>
        private readonly string _prefabName;

        /// <summary>
        /// Contains all GameObjects of the ExperimentSetting prefab up to depth <see cref="MaxDepth"/> in the object hierarchy
        /// </summary>
        protected GameObject[] GameObjectsFromExperimentPrefab;

        /// <summary>
        /// Specifies max depth to fill <see cref="GameObjectsFromExperimentPrefab"/> with <see cref="GetChildrenFromGameObject"/>
        /// </summary>
        private const int MaxDepth = 5;
        
        /// <summary>
        /// Holds all objects' names contained in <see cref="GameObjectsFromExperimentPrefab"/>
        /// </summary>
        private string[] _objectNamesFromExperimentPrefab;

        /// <summary>
        /// Base constructor used by TestFixtureSource annotation to initialize attributes
        /// </summary>
        /// <param name="experimentName">Name of the experiment scene to be tested</param>
        /// <param name="scenePath">Relative path to scene starting from "Assets" folder</param>
        /// <param name="sceneType">Name of the ExperimentSetting prefab (room template)</param>
        protected SceneValidationBaseFixture(string experimentName, string scenePath, string sceneType) =>
            (_experimentName, _scenePath, _prefabName) = (experimentName, scenePath, ExperimentPrefabName + sceneType);

        /// <summary>
        /// Called once for each test fixture on test execution.
        /// Queries all GameObjects from ExperimentSetting prefab and if necessary loads the scene matching the test fixture.
        /// </summary>
        protected void BaseOneTimeSetup()
        {
            // Get objects and names from ExperimentSetting prefab
            var experimentSettingPrefab = GetPrefabByName(_prefabName);
            GameObjectsFromExperimentPrefab = GetChildrenFromGameObject(experimentSettingPrefab, MaxDepth);
            _objectNamesFromExperimentPrefab = GameObjectsFromExperimentPrefab.Select(x => x.name).ToArray();
            
            // Load scene if necessary
            var scene = SceneManager.GetSceneAt(0);
            if (SceneManager.sceneCount > 1 || scene.path != _scenePath)
            {
                EditorSceneManager.OpenScene(_scenePath, OpenSceneMode.Single);
            }
        }
        
        /// <summary>
        ///  Skips test if check is triggered
        /// </summary>
        /// <param name="objectNameUnderTest">name of object under test</param>
        /// <param name="callingMethodName">test method name (automatically provided through <see cref="CallerMemberNameAttribute"/>)</param>
        /// <remarks>
        /// Wrapper for utility function <see cref="SkipCheckBase"/> with shorter param list and fixture specific arguments
        /// </remarks>
        protected void SkipCheck(string objectNameUnderTest, [CallerMemberName] string callingMethodName = null)
        {
            SkipCheckBase<Type>(TypePC, _objectNamesFromExperimentPrefab,
                _experimentName, objectNameUnderTest, callingMethodName);
        }

        [Test, Description("MonoBehaviours must not have any missing target objects or methods for UnityEvents.")]
        public void SceneHasNoMissingUnityEventMethods()
        {
            // Based on https://gist.github.com/AaronV/3fd7cc22039cf34f536ab98db47d044a
            // and https://stackoverflow.com/questions/42784338/unity-missing-warning-when-button-has-missing-onclick/42788400

            List<string> errors = new List<string>();

            // Iterate over all MonoBehaviours
            MonoBehaviour[] monoBehavioursInScene = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            foreach (MonoBehaviour monoBehaviour in monoBehavioursInScene)
            {
                // Check all fields whether they are a UnityEvent
                System.Type monoBehaviourType = monoBehaviour.GetType();
                FieldInfo[] fields = monoBehaviourType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (FieldInfo field in fields)
                {
                    if (!typeof(UnityEvent).IsAssignableFrom(field.FieldType)) 
                        continue;

                    UnityEvent unityEvent = field.GetValue(monoBehaviour) as UnityEvent;
                    // Check all persistent (assigned via the Inspector) UnityEvent listeners
                    for (int persistentEventCountIndex = 0; persistentEventCountIndex < unityEvent.GetPersistentEventCount(); persistentEventCountIndex++)
                    {
                        // Assert event target object is not null
                        UnityEngine.Object eventTargetObject = unityEvent.GetPersistentTarget(persistentEventCountIndex);
                        if (eventTargetObject == null)
                        {
                            errors.Add($"The UnityEvent of {monoBehaviourType.Name} \"{monoBehaviour.name}\" called \"{field.Name}\" " +
                                $"has an event target object that is null (index {persistentEventCountIndex}). " +
                                $"The path of the GameObject is {monoBehaviour.gameObject.GetScenePath()}");
                            continue;
                        }

                        // Assert event target object Type is not null
                        string eventTargetObjectFullName = eventTargetObject.GetType()?.AssemblyQualifiedName;
                        if (string.IsNullOrEmpty(eventTargetObjectFullName)) // AssemblyQualifiedName can be null if the current instance represents a generic type parameter
                            continue;
                        System.Type eventTargetObjectType = System.Type.GetType(eventTargetObjectFullName);
                        if (eventTargetObjectType == null)
                        {
                            errors.Add($"The UnityEvent of {monoBehaviourType.Name} \"{monoBehaviour.name}\" called \"{field.Name}\" " +
                                $"has an event target object whose type is null (index {persistentEventCountIndex})." +
                                $"The path of the GameObject is {monoBehaviour.gameObject.GetScenePath()}");
                            continue;
                        }

                        string eventTargetMethodName = unityEvent.GetPersistentMethodName(persistentEventCountIndex);
                        try
                        {
                            MethodInfo methodInfo = eventTargetObjectType.GetMethod
                                (eventTargetMethodName,
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                            // Assert the event target method exists
                            if (methodInfo != null)
                                continue;
                            errors.Add($"The UnityEvent of  {monoBehaviourType.Name}  \" {monoBehaviour.name} \" called \"{field.Name}\" " +
                                $"has an event target method \"{eventTargetMethodName}\" that could not be found (index {persistentEventCountIndex}). " +
                                $"The path of the GameObject is {monoBehaviour.gameObject.GetScenePath()}");
                        }
                        catch (AmbiguousMatchException)
                        {
                            // Multiple overloads for the method found, this is okay
                        }
                    }
                }
            }

            Assert.Zero(errors.Count, "Found " + errors.Count + " error(s):\r\n" + string.Join("\r\n", errors));
        }
    }
}
