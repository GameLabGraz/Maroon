/*
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Tests.EditModeTests.ContentValidation
{
    // Provides all PC experiment names with matching scene paths to tests and test fixtures
    public class PcScenesProvider : IEnumerable
    {
        private readonly Regex _experimentNameRegex = new Regex(@"\w+\.pc");
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var scene in EditorBuildSettings.scenes)
            {
                // Only return PC scenes from the experiments folder that are enabled in build settings
                if (scene.enabled && scene.path != null && scene.path.Contains("experiments") && scene.path.EndsWith(".pc.unity"))
                {
                    // Return the experiment name extracted from its path - this defines test fixture name
                    var experimentName = _experimentNameRegex.Match(scene.path).ToString();
                    yield return new object[] { experimentName, scene.path };
                }
            }
        }
    }
}
*/
