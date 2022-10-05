using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Tests.EditModeTests.ContentValidation
{
    // Used in Scene Validation test fixtures:
    // It provides experiment scene names and full paths retrieved from editor build settings
    public abstract class ScenesProvider : IEnumerable
    {
        protected abstract Regex experimentNameRegex { get; }
        protected abstract string fileEnding { get; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var scene in EditorBuildSettings.scenes)
            {
                // Only return PC scenes from the experiments folder that are enabled in build settings
                if (scene.enabled && scene.path != null && scene.path.Contains("experiments") && scene.path.EndsWith(fileEnding))
                {
                    // Return the experiment name extracted from its path - this defines test fixture name
                    var experimentName = experimentNameRegex.Match(scene.path).ToString();
                    yield return new object[] { experimentName, scene.path };
                }
            }
        }
    }
}