using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using System.IO;
using static Tests.Utilities.Constants;

namespace Tests.EditModeTests.ContentValidation
{
    /// <summary>
    /// Provides names of the experiment folders to test fixtures using the <c>TestFixtureSource</c> attribute
    /// </summary>
    public class ExperimentFolderProvider : IEnumerable
    {
        /// <summary>
        /// Enumerates the folders of all experiments
        /// </summary>
        /// <returns>Absolute path to experiment folders</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            string[] experimentDirectories = Directory.GetDirectories(GetBaseExperimentsPath());
            foreach (string experimentDirectory in experimentDirectories)
            {
                string dirName = Path.GetFileName(experimentDirectory.TrimEnd(Path.DirectorySeparatorChar));
                yield return dirName;
            }
        }

        /// <summary>
        /// Returns the path to the folder where all experiments subfolders are located.
        /// </summary>
        /// <returns>Absolute path to the Maroon/scenes/experiments folder.</returns>
        public static string GetBaseExperimentsPath()
        {
            return Path.Combine(
                Application.dataPath,
                "Maroon/scenes/experiments"
            );
        }
    }
}