using NUnit.Framework;
using UnityEngine;
using System.IO;
using System.Linq;

namespace Tests.EditModeTests.ContentValidation
{
    [TestFixtureSource(typeof(ExperimentFolderProvider))]
    public class MissingAssemblyDefinitionTests
    {
        private string experimentFolderPath;

        public MissingAssemblyDefinitionTests(string experimentFolderName, string absolutePathExperimentFolder)
        {
            experimentFolderPath = absolutePathExperimentFolder;
        }

        [Test, Description("Tests if each experiment has its own Assembly Definition.")]
        public void TestMissingAssemblyDefinitionForExperiments()
        {
            int numberOfCsFiles = Directory.EnumerateFiles(experimentFolderPath, "*.*", SearchOption.AllDirectories)
                .Count(s => Path.GetExtension(s).TrimStart('.').ToLowerInvariant().Equals("cs"));
            if (numberOfCsFiles > 0)
            {
                string[] asmdefFiles = Directory.GetFiles(experimentFolderPath, "*.asmdef", SearchOption.AllDirectories);
                Assert.AreNotEqual(asmdefFiles.Length, 0, "Experiment needs at least one .asmdef Assembly Definition file in " + experimentFolderPath + ".");
            }
            else
            {
                Assert.Zero(numberOfCsFiles, "No .cs files in an experiment, not need for its own assembly definition.");
                Debug.Log("No .cs files, no assembly definition required.");
            }
        }
    }
}
