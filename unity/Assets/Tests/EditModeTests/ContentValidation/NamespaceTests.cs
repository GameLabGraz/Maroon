using NUnit.Framework;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Tests.EditModeTests.ContentValidation
{
    [TestFixtureSource(typeof(ExperimentFolderProvider))]
    public class NamespaceTests
    {
        private string experimentFolderPath;

        public NamespaceTests(string experimentFolderName, string absolutePathExperimentFolder)
        {
            experimentFolderPath = absolutePathExperimentFolder;
        }

        [Test, Description("Tests if every script of an experiment contains a namespace.")]
        public void TestNamespaceInScriptsForExperiments()
        {
            const string STRING_TO_CHECK = "namespace ";

            // Get all .cs files
            IEnumerable<string> csFiles = Directory.EnumerateFiles(experimentFolderPath, "*.*", SearchOption.AllDirectories)
                .Where(s => Path.GetExtension(s).TrimStart('.').ToLowerInvariant().Equals("cs"));

            foreach (string csFilePath in csFiles)
            {
                string fileContents = File.ReadAllText(csFilePath);
                Assert.IsTrue(fileContents.Contains(STRING_TO_CHECK), "Class in file " + csFilePath + " needs to be in a namespace.");
            }
            Debug.Log("All " + csFiles.Count().ToString() + " .cs files contain a namespace.");
        }
    }
}
