using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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
            IEnumerable<string> csFilePaths = Directory.EnumerateFiles(experimentFolderPath, "*.*", SearchOption.AllDirectories)
                .Where(s => Path.GetExtension(s).TrimStart('.').ToLowerInvariant().Equals("cs"));

            if (csFilePaths.Any())
            {
                // Get all Assembly Definition files WITHIN the experiment folder
                IEnumerable<string> asmdefFilePaths = Directory.EnumerateFiles(experimentFolderPath, "*.*", SearchOption.AllDirectories)
                    .Where(s => Path.GetExtension(s).TrimStart('.').ToLowerInvariant().Equals("asmdef"));
                HashSet<string> directoriesWithAsmdefFiles = asmdefFilePaths.Select(path => Path.GetDirectoryName(path)).ToHashSet();

                foreach (string csFilePath in csFilePaths)
                {
                    string csFileDirectory = Path.GetDirectoryName(csFilePath);

                    // Check if the directory of the .cs file also contains any of the .asmdef files
                    if (directoriesWithAsmdefFiles.Contains(csFileDirectory))
                        continue;

                    // Check if any parent directory contains any of the .asmdef files
                    string parentDirectory = Path.GetDirectoryName(csFileDirectory);
                    bool foundAsmdef = false;
                    while (!string.IsNullOrEmpty(parentDirectory))
                    {
                        if (directoriesWithAsmdefFiles.Contains(parentDirectory))
                        {
                            foundAsmdef = true;
                            break;
                        }

                        parentDirectory = Path.GetDirectoryName(parentDirectory);
                    }

                    Assert.True(foundAsmdef, "The experiment .cs file \"" + csFilePath + "\" needs to be part of an AssemblyDefinition in the experiment folder \"" + experimentFolderPath + "\".");
                }
            }
            else
            {
                Assert.Pass("Experiment has no .cs files, thus no AssemblyDefinition required.");
            }
        }
    }
}
