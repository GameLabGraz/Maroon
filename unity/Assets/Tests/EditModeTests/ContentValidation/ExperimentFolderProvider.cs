using System.Collections;
using UnityEngine;
using System.IO;

namespace Tests.EditModeTests.ContentValidation
{
    /// <summary>
    /// Provides names of the experiment folders to test fixtures using the <c>TestFixtureSource</c> attribute
    /// 
    /// Note: Enumerator just returns the folder name, cannot directly return the absolute path, 
    /// as that long name would be truncated for test case identification resulting in test cases named the same.
    /// </summary>
    public class ExperimentFolderProvider : IEnumerable
    {
        /// <summary>
        /// Enumerates the folders of all experiments
        /// </summary>
        /// <returns>Name of the experiment folder</returns>
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