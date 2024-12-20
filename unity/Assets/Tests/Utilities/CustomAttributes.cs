using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using static Tests.Utilities.Constants;

namespace Tests.Utilities
{
    public static class CustomAttributes
    {
        /// <summary>
        /// Use on test methods, where multiple uses per test case are possible.
        /// </summary>
        /// <param name="experimentNames">One or more experiment names in a comma separated string with optional spaces</param>
        /// <param name="reasonToSkip">Explanation why test case is skipped for specified experiment</param>
        /// <example>
        /// <code>
        /// [SkipTestFor("FallingCoil, "CoulombsLaw", "we prefer darkness")]
        /// [Test, Description("...")]
        /// public void sceneHasLights() { ... }
        /// </code>
        /// results in the test case "sceneHasLights" being skipped for FallingCoil and CoulombsLaw with the TestRunner info displaying "we prefer darkness" 
        /// </example>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public sealed class SkipTestForScenesWithReasonAttribute : Attribute
        {
            public string[] ExperimentNames { get; }
            public string ReasonToSkip { get; }

            public SkipTestForScenesWithReasonAttribute(string experimentNames, string reasonToSkip)
            {
                ExperimentNames = experimentNames.Split(',').Select(experimentName => experimentName.Trim()).ToArray();
                ReasonToSkip = reasonToSkip;
            }

            /// <summary>
            /// Get all attached <c>SkipTestForScenesWithReason</c> attributes of an annotated test method
            /// </summary>
            /// <typeparam name="T">The annotated test method's test fixture class</typeparam>
            /// <param name="methodName">The annotated test method's name</param>
            /// <returns>All attached custom attribute objects in an array</returns>
            public static SkipTestForScenesWithReasonAttribute[] GetAttributeCustom<T>(string methodName)
                where T : class
            {
                try
                {
                    return (SkipTestForScenesWithReasonAttribute[])typeof(T).GetMethod(methodName)
                        .GetCustomAttributes(typeof(SkipTestForScenesWithReasonAttribute), false);
                }
                catch (SystemException)
                {
                    return null;
                }
            }
        }
        
        /// <summary>
        /// Checks if either is true and the test case should be skipped:
        /// <list type="number">
        /// <item>
        /// <description>The test case is annotated with [<see cref="SkipTestForScenesWithReasonAttribute"/>] and the experiment name matches</description>
        /// </item>
        /// <item>
        /// <description>The <paramref name="nameOfObjectUnderTest"/> is not part of the experimentSettings prefab</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Don't call this directly, use a test fixture's associated wrapper method <c>SkipCheck</c> instead
        /// </remarks>
        /// <param name="sceneType">Type of scene: either PC or VR</param>
        /// <param name="objectNamesFromExperimentPrefab">Contains object names from experimentSetting prefab</param>
        /// <param name="currentExperimentName">Name of the current experiment under test</param>
        /// <param name="nameOfObjectUnderTest">Name of the object under test. It is matched against <paramref name="objectNamesFromExperimentPrefab"/></param>
        /// <param name="callingMethodName">The test method's name</param>
        public static void SkipCheckBase<T>(string sceneType, IEnumerable<string> objectNamesFromExperimentPrefab,
            string currentExperimentName,
            string nameOfObjectUnderTest, string callingMethodName) where T : class
        {
            // Get any attached attribute objects
            var skipTestForAttributes =
                SkipTestForScenesWithReasonAttribute.GetAttributeCustom<T>(callingMethodName);

            // Check if the provided scene(s) match the current test, if yes then skip test
            if (skipTestForAttributes != null)
            {
                foreach (var skipTestForAttribute in skipTestForAttributes)
                {
                    if (skipTestForAttribute.ExperimentNames.Any(x =>
                        currentExperimentName.ToUpper().Contains(x.ToUpper())))
                    {
                        Assert.Ignore($"Skipped test! Reason: {skipTestForAttribute.ReasonToSkip})");
                    }
                }
            }

            // Check if the object is part of our ExperimentSetting prefab otherwise skip test
            if (objectNamesFromExperimentPrefab != null && !objectNamesFromExperimentPrefab.Any(x => x.ToUpper().Contains(nameOfObjectUnderTest.ToUpper())))
            {
                Assert.Ignore(
                    $"{ExperimentPrefabName + sceneType} contains no {nameOfObjectUnderTest} - skipping test!");
            }
        }
    }
}