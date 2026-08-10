using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;
using static Tests.Utilities.Constants;

namespace Tests.EditModeTests.ContentValidation
{
    /// <summary>
    /// Provides all scenes in Build Settings of type PC
    /// </summary>
    public class PcScenesProvider : ScenesProvider { protected override string sceneType => TypePC; }

    /// <summary>
    /// Provides all scenes in Build Settings of type PC
    /// </summary>
    public class VrScenesProvider : ScenesProvider { protected override string sceneType => TypeVR; }


    /// <summary>
    /// Provides scene names and paths from build settings to test fixtures using the <c>TestFixtureSource</c> attribute
    /// </summary>
    /// <example>
    /// <code>
    /// public class PcScenesProvider : ScenesProvider { ... }
    /// [TestFixtureSource(typeof(PcScenesProvider))]
    /// public class PcSceneValidationTests { ... }
    /// </code>
    /// </example>
    /// <remarks>
    /// The content validation PC and VR test fixtures use a derived scenes provider to provide a string
    /// </remarks>
    public abstract class ScenesProvider : IEnumerable
    {
        /// <summary>
        /// Specify type of scenes to look for (PC or VR)
        /// </summary>
        protected abstract string sceneType { get; }
        
        /// <summary>
        /// Regex to match experiment scenes of a specific type (PC or VR)
        /// </summary>
        private Regex experimentNameRegex => new Regex(@"\w+\." + sceneType.ToLower());

        /// <summary>
        /// Queries all scenes from Build Settings menu and filters experiments of specified type.
        /// </summary>
        /// <returns>It yields a pair of an experiment scene's name and its matching scene path on each iteration</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var scene in EditorBuildSettings.scenes)
            {
                // Only return scenes that are enabled in build settings, reside the experiments folder and match the file ending 
                if (scene.enabled && scene.path != null && scene.path.Contains("experiments") && scene.path.ToLower().EndsWith("." + sceneType.ToLower() + ".unity"))
                {
                    // Return the experiment name extracted from its path - this defines test fixture name
                    var experimentName = experimentNameRegex.Match(scene.path).ToString();
                    yield return new object[] { experimentName, scene.path};
                }
            }
        }
    }
}