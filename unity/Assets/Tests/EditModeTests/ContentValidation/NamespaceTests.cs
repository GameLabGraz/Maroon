using NUnit.Framework;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static Tests.Utilities.Constants;
using static Tests.Utilities.CustomAttributes;
using static Tests.Utilities.UtilityFunctions;

namespace Tests.EditModeTests.ContentValidation
{
    [TestFixtureSource(typeof(ExperimentFolderProvider))]
    public class NamespaceTests
    {
        private string experimentFolderName;
        private string experimentFolderPath;

        public NamespaceTests(string newExperimentFolderName, string absolutePathExperimentFolder)
        {
            experimentFolderName = newExperimentFolderName;
            experimentFolderPath = absolutePathExperimentFolder;
        }

        // Do NOT add new experiments to this list of skipped experiments! Instead simply use namespaces in all .cs files
        [SkipTestForScenesWithReason("3DMotionSimulation, Capacitor, Catalyst, CoulombsLaw, HuygensPrinciple, MinimumSpanningTree, " +
            "PathFinding, PointWave, SortingAlgorithms, TitrationExperiment, VanDeGraaffGenerator, Whiteboard", 
            "Legacy code; should be ported to include namespaces some time soon.")]
        [Test, Description("Tests if every script of an experiment contains a namespace.")]
        public void TestNamespaceInScriptsForExperiments()
        {
            SkipCheck();

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

        /// <summary>
        /// Skips test if check is triggered
        /// </summary>
        /// <param name="callingMethodName">test method name (automatically provided through <see cref="CallerMemberNameAttribute"/>)</param>
        /// <remarks>
        /// Wrapper for utility function <see cref="SkipCheckBase"/> with shorter param list and fixture specific arguments
        /// </remarks>
        protected void SkipCheck([CallerMemberName] string callingMethodName = null)
        {
            SkipCheckBase<NamespaceTests>(
                "", 
                null,
                experimentFolderName, 
                "", 
                callingMethodName
            );
        }
    }
}
