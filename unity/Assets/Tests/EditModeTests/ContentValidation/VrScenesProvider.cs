/*using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Tests.EditModeTests.ContentValidation
{
    // Provides all VR experiment names with matching scene paths to tests and test fixtures
    public class VrScenesProvider : IEnumerable
    {
        private readonly Regex _experimentNameRegex = new Regex(@"\w+\.vr");

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var scene in EditorBuildSettings.scenes)
            {
                // Only return VR scenes from the experiments folder that are enabled in build settings
                if (scene.enabled && scene.path != null && scene.path.Contains("experiments") &&
                    scene.path.EndsWith(".vr.unity"))
                {
                    // Return the experiment name extracted from its path - this defines test fixture name
                    var experimentName = _experimentNameRegex.Match(scene.path).ToString();
                    yield return experimentName;
                }
            }
        }
    }
}*/