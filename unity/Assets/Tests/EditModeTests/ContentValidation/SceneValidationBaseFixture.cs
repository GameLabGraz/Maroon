using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Tests.Utilities.UtilityFunctions;
using static Tests.Utilities.Constants;

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
        /// Contains all GameObjects of the ExperimentSetting prefab up to a specified depth in the object hierarchy
        /// </summary>
        protected GameObject[] GameObjectsFromExperimentPrefab;

        /// <summary>
        /// Specifies max depth for <see cref="GameObjectsFromExperimentPrefab"/>
        /// </summary>
        private const int GetObjectsFromPrefabMaxDepth = 5;
        
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
            GameObjectsFromExperimentPrefab = GetObjectsFromPrefabOfDepth(_prefabName, GetObjectsFromPrefabMaxDepth);
            _objectNamesFromExperimentPrefab = GameObjectsFromExperimentPrefab.Select(x => x.name).ToArray();

            // Load scene
            LoadSceneIfNotYetLoaded(_scenePath);
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
    }
}
