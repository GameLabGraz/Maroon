using NUnit.Framework;
using UnityEngine;
using System.IO;

namespace Tests.EditModeTests.ContentValidation
{
    [TestFixtureSource(typeof(ExperimentFolderProvider))]
    public class MissingAssemblyDefinitionTests
    {
        private string experimentFolderPath;

        public MissingAssemblyDefinitionTests(string experimentFolderName)
        {
            experimentFolderPath = Path.Combine(ExperimentFolderProvider.GetBaseExperimentsPath(), experimentFolderName);
        }

        [Test, Description("Tests if each experiment has its own Assembly Definition.")]
        public void TestMissingAssemblyDefinitionForExperiments()
        {
            string[] asmdefFiles = Directory.GetFiles(experimentFolderPath, "*.asmdef", SearchOption.AllDirectories);
            Assert.AreNotEqual(asmdefFiles.Length, 0, "Experiment needs at least one .asmdef Assembly Definition file in " + experimentFolderPath + ".");
        }
    }
}
