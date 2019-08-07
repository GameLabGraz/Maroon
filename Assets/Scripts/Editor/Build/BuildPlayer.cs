using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Maroon.Build
{
    public class BuildPlayer
    {
        private const string ScenePath = "Assets/Scenes";
        private const string LabPath = ScenePath + "/Laboratory";
        private const string MenuPath = ScenePath + "/Menu";
        private const string ExperimentPath = ScenePath + "/Experiments";

        private const string PcExtension = ".pc.unity";
        private const string VrExtension = ".vr.unity";

        private enum MaroonBuildTarget
        {
            PC,
            MAC,
            WebGL,
            VR
        }

        [MenuItem(("Build/Build PC"))]
        public static void BuildPC()
        {
            Build(MaroonBuildTarget.PC);
        }

        [MenuItem(("Build/Build MAC"))]
        public static void BuildMAC()
        {
            Build(MaroonBuildTarget.MAC);
        }

        [MenuItem(("Build/Build VR"))]
        public static void BuildVR()
        {
            Build(MaroonBuildTarget.VR);
        }

        [MenuItem(("Build/Build WebGL"))]
        public static void BuildWebGL()
        {
            Build(MaroonBuildTarget.WebGL);
        }

        private static void Build(MaroonBuildTarget buildTarget)
        {
            var buildPath = EditorUtility.SaveFolderPanel("Choose Build Location", "Build", $"{buildTarget}");
            if (buildPath.Length == 0)
                return;

            Debug.ClearDeveloperConsole();
            Debug.Log($"Start building for {buildTarget} ...");

            PlayerSettings.bundleVersion = DateTime.UtcNow.Date.ToString("yyyyMMdd");

            var buildPlayerOptions = new BuildPlayerOptions
            {
                target = MaroonBuildTarget2UnityBuildTarget(buildTarget),
                options = BuildOptions.None,
                locationPathName = $"{buildPath}/{GetAppName(buildTarget)}",
                scenes = GetScenes(buildTarget)
            };

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            HandleBuildResult(report.summary);
        }

        private static string[] GetScenes(MaroonBuildTarget buildTarget)
        {
            var sceneExtension = GetSceneExtension(buildTarget);
            if (sceneExtension == string.Empty)
            {
                Debug.LogError($"BuildPlayer::GetScenes: Unable to load Scenes for {buildTarget}");
                return null;
            }

            var scenes = new List<string>
            {
                $"{LabPath}/Laboratory{sceneExtension}",
                $"{MenuPath}/Menu.unity"
            };

            var experiments = Directory.GetFiles(ExperimentPath, $"*{sceneExtension}", SearchOption.AllDirectories);
            scenes.AddRange(experiments);

            return scenes.ToArray();
        }

        private static string GetSceneExtension(MaroonBuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case MaroonBuildTarget.WebGL:
                case MaroonBuildTarget.MAC:
                case MaroonBuildTarget.PC:
                    return PcExtension;
                case MaroonBuildTarget.VR:
                    return VrExtension;
                default:
                    return "";
            }
        }

        private static string GetAppName(MaroonBuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case MaroonBuildTarget.WebGL:
                    return "";
                case MaroonBuildTarget.MAC:
                    return "Maroon.app";
                case MaroonBuildTarget.PC:
                case MaroonBuildTarget.VR:
                    return "Maroon.exe";
                default:
                    return "";
            }
        }

        private static BuildTarget MaroonBuildTarget2UnityBuildTarget(MaroonBuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case MaroonBuildTarget.WebGL:
                    return BuildTarget.WebGL;
                case MaroonBuildTarget.MAC:
                    return BuildTarget.StandaloneOSX;
                case MaroonBuildTarget.PC:
                case MaroonBuildTarget.VR:
                    return BuildTarget.StandaloneWindows;
                default:
                    return BuildTarget.NoTarget;
            }
        }

        private static void HandleBuildResult(BuildSummary summary)
        {
            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    Debug.Log($"Build succeeded: {summary.totalSize} bytes");
                    break;
                case BuildResult.Failed:
                    Debug.LogError("Build failed!");
                    break;
                case BuildResult.Unknown:
                    Debug.LogWarning("Unknown Build result.");
                    break;
                case BuildResult.Cancelled:
                    Debug.Log("Build cancelled.");
                    break;
                default:
                    throw new Exception("BuildPlayer: Unable to handle build result.");
            }
        }
    }
}
