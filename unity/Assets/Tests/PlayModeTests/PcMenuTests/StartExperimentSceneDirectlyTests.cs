using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Tests.Utilities.PlaymodeUtilities;

namespace Tests.PlayModeTests.PcMenuTests
{
    /// <summary>
    /// Tests if each experiment scene can be started directly
    /// </summary>
    public class StartExperimentSceneDirectlyTests
    {
        // Experiment scenes
        // TODO: Find better solution than hardcoding the scenes (SceneManager.GetSceneAt does not work though)
        private static readonly string[] ExperimentScenePaths =
        {
            "Assets/Maroon/scenes/experiments/FallingCoil/FallingCoil.pc.unity",
            "Assets/Maroon/scenes/experiments/Whiteboard/Whiteboard.pc.unity",
            "Assets/Maroon/scenes/experiments/HuygensPrinciple/HuygensPrinciple.pc.unity",
            "Assets/Maroon/scenes/experiments/Pendulum/Pendulum.pc.unity",
            "Assets/Maroon/scenes/experiments/FaradaysLaw/FaradaysLaw.pc.unity",
            "Assets/Maroon/scenes/experiments/VanDeGraaffGenerator/VandeGraaffBalloon.pc.unity",
            "Assets/Maroon/scenes/experiments/VanDeGraaffGenerator/VandeGraaffGenerator.pc.unity",
            "Assets/Maroon/scenes/experiments/SortingAlgorithms/Sorting.pc.unity",
            "Assets/Maroon/scenes/experiments/TitrationExperiment/TitrationExperiment.pc.unity",
            "Assets/Maroon/scenes/experiments/OpticsSimulations/Optics.pc.unity",
            "Assets/Maroon/scenes/experiments/PointWave/PointWaveExperiment.pc.unity",
            "Assets/Maroon/scenes/experiments/StateMachine/StateMachine.pc.unity",
            "Assets/Maroon/scenes/experiments/PathFinding/PathFinding.pc.unity",
            "Assets/Maroon/scenes/experiments/CathodeRayTube/CathodeRayTube.pc.unity",
            "Assets/Maroon/scenes/experiments/CoulombsLaw/CoulombsLaw.pc.unity",
            "Assets/Maroon/scenes/experiments/3DMotionSimulation/3DMotionSimulation.pc.unity",
            "Assets/Maroon/scenes/experiments/MinimumSpanningTree/MinimumSpanningTree.pc.unity",
            "Assets/Maroon/scenes/experiments/Catalyst/Catalyst.pc.unity",
        };

        [UnityTest, Description("Attempts to start an experiment scene directly")]
        [Timeout(1800000)]
        public IEnumerator StartExperimentSceneDirectly([ValueSource(nameof(ExperimentScenePaths))] string scenePath)
        {
            Debug.Log("Trying to load experiment at " + scenePath + "; at " + Time.realtimeSinceStartup);
            yield return LoadSceneAndCheckItsLoadedCorrectly(scenePath);
            Debug.Log("Finished loading experiment at " + scenePath + "; at " + Time.realtimeSinceStartup);
        }
    }
}
