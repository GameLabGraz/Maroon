using NUnit.Framework;
using UnityEngine;
using System.IO;

namespace Tests.EditModeTests.ContentValidation
{
    public class MissingAssemblyDefinitionTests
    {
        [Test, Description("Tests if each experiment has its own Assembly Definition.")]
        public void TestMissingAssemblyDefinitionForExperiments()
        {
            string allExperimentsPath = Path.Combine(
                Application.dataPath, 
                "Maroon/scenes/experiments"
            );

            string[] experimentDirectories = Directory.GetDirectories(allExperimentsPath);
            foreach (string experimentDirectory in experimentDirectories)
            {
                Debug.Log("ex="+ experimentDirectory);
                string[] asmdefFiles = Directory.GetFiles(experimentDirectory, "*.asmdef", SearchOption.AllDirectories);
                Assert.AreNotEqual(asmdefFiles.Length, 0, "Experiment needs at least one .asmdef Assembly Definition file in " + experimentDirectory + ".");
            }
        }
    }
}
