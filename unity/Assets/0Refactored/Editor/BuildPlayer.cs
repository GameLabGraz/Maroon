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
        // #############################################################################################################
        // Members

        private const string ScenePath      = "Assets/0Refactored/scenes";
        private const string LabPath        = ScenePath + "/laboratory";
        private const string MenuPath       = ScenePath + "/menu";
        private const string ExperimentPath = ScenePath + "/experiments";

        private const string PcExtension = ".pc.unity";
        private const string VrExtension = ".vr.unity";

        private struct PlayerSetOptions
        {
            public string BundleVersion;
            public int DefaultScreenWidth;
            public int DefaultScreenHeight;
            public FullScreenMode FullScreenMode;
        }

        private enum MaroonBuildTarget
        {
            PC,
            MAC,
            WebGL,
            VR
        }

        // #############################################################################################################
        // Editor Build Methods

        [MenuItem("Build/Build All")]
        public static void BuildAll()
        {
            var buildPath = EditorUtility.SaveFolderPanel("Choose Build Location", string.Empty, "Build");

            if (buildPath.Length == 0)
            {
                return;
            }

            foreach(var buildTarget in (MaroonBuildTarget[])Enum.GetValues(typeof(MaroonBuildTarget)))
            {
                Build(buildTarget, $"{buildPath}/{buildTarget}");
            }
        }

        [MenuItem("Build/Build for Platform/Build PC")]
        public static void BuildPC()
        {
            Build(MaroonBuildTarget.PC);
        }

        [MenuItem("Build/Build for Platform/Build MAC")]
        public static void BuildMAC()
        {
            Build(MaroonBuildTarget.MAC);
        }

        [MenuItem("Build/Build for Platform/Build VR")]
        public static void BuildVR()
        {
            Build(MaroonBuildTarget.VR);
        }

        [MenuItem("Build/Build for Platform/Build WebGL")]
        public static void BuildWebGL()
        {
            Build(MaroonBuildTarget.WebGL);
        }

        // #############################################################################################################
        // Build Methods

        private static void Build(MaroonBuildTarget buildTarget, string buildPath = null)
        {
            if(string.IsNullOrEmpty(buildPath))
            {
                if(!UnityEditorInternal.InternalEditorUtility.isHumanControllingUs)
                {
                    return;
                }

                buildPath = EditorUtility.SaveFolderPanel("Choose Build Location", "Build", $"{buildTarget}");

                if(buildPath.Length == 0)
                {
                    return;
                }
            }

            if(UnityEditorInternal.InternalEditorUtility.isHumanControllingUs)
            {
                Debug.ClearDeveloperConsole();
            }

            Log($"Start building for {buildTarget} ...");

            var defaultPlayerSettings = GetPlayerSettings();

            // Set PlayerSettings for Build
            SetPlayerSettings(new PlayerSetOptions()
            {
                BundleVersion = DateTime.UtcNow.Date.ToString("yyyyMMdd"),
                DefaultScreenWidth = 1920,
                DefaultScreenHeight = 1080,
                FullScreenMode = FullScreenMode.FullScreenWindow
            });

            var unityBuildTarget = MaroonBuildTarget2UnityBuildTarget(buildTarget);
            var unityBuildTargetGroup = GetBuildTargetGroup(buildTarget);

            // Switch to build target platform before build
            EditorUserBuildSettings.SwitchActiveBuildTarget(unityBuildTargetGroup, unityBuildTarget);

            var buildPlayerOptions = new BuildPlayerOptions
            {
                target = unityBuildTarget,
                targetGroup = unityBuildTargetGroup,
                options = BuildOptions.None,
                locationPathName = $"{buildPath}/{GetAppName(buildTarget)}",
                scenes = GetScenes(buildTarget)
            };

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            HandleBuildResult(report.summary);

            // Restore PlayerSettings for Editor
            SetPlayerSettings(defaultPlayerSettings);
        }

        public static void JenkinsBuild()
        {
            var args = Environment.GetCommandLineArgs();

            var executeMethodIndex = Array.IndexOf(args, "-executeMethod");
            if (executeMethodIndex + 2 >= args.Length)
            {
                Log("[JenkinsBuild] Incorrect Parameters for -executeMethod Format: -executeMethod <output dir>");
                return;
            }

            //  args[executeMethodIndex + 1] = JenkinsBuild.Build
            var buildPath = args[executeMethodIndex + 2];

            // run build for each build target
            foreach(var buildTarget in (MaroonBuildTarget[])Enum.GetValues(typeof(MaroonBuildTarget)))
            {
                Build(buildTarget, $"{buildPath}/{buildTarget}");
            }
        }

        // #############################################################################################################
        // Helper Methods

        private static string[] GetScenes(MaroonBuildTarget buildTarget)
        {
            var sceneExtension = GetSceneExtension(buildTarget);
            if (sceneExtension == string.Empty)
            {
                Log($"BuildPlayer::GetScenes: Unable to load Scenes for {buildTarget}");
                return null;
            }
            
            var scenes = new List<string>();

            // TODO: If VR Main Menu ready, remove if, just disables main menu for VR 
            if(sceneExtension == PcExtension) 
            {
                scenes.Add($"{MenuPath}/MainMenu{sceneExtension}");
            }
            scenes.Add($"{LabPath}/Laboratory{sceneExtension}");

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
                    return "Maroon.exe";
                case MaroonBuildTarget.VR:
                    return "MaroonVR.exe";
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

        private static BuildTargetGroup GetBuildTargetGroup(MaroonBuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case MaroonBuildTarget.WebGL:
                    return BuildTargetGroup.WebGL;
                case MaroonBuildTarget.MAC:
                case MaroonBuildTarget.PC:
                case MaroonBuildTarget.VR:
                    return BuildTargetGroup.Standalone;
                default:
                    return BuildTargetGroup.Unknown;
            }
        }

        private static void HandleBuildResult(BuildSummary summary)
        {
            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    Log($"Build succeeded: {summary.totalSize} bytes");
                    break;
                case BuildResult.Failed:
                    Log("Build failed!");
                    break;
                case BuildResult.Unknown:
                    Log("Unknown Build result.");
                    break;
                case BuildResult.Cancelled:
                    Debug.Log("Build cancelled.");
                    break;
                default:
                    throw new Exception("BuildPlayer: Unable to handle build result.");
            }
        }

        private static void SetPlayerSettings(PlayerSetOptions options)
        {
            PlayerSettings.bundleVersion = options.BundleVersion;
            PlayerSettings.defaultScreenWidth = options.DefaultScreenWidth;
            PlayerSettings.defaultScreenHeight = options.DefaultScreenHeight;
            PlayerSettings.fullScreenMode = options.FullScreenMode;
        }

        private static PlayerSetOptions GetPlayerSettings()
        {
            return new PlayerSetOptions()
            {
                BundleVersion = PlayerSettings.bundleVersion,
                DefaultScreenWidth = PlayerSettings.defaultScreenWidth,
                DefaultScreenHeight = PlayerSettings.defaultScreenHeight,
                FullScreenMode = PlayerSettings.fullScreenMode
            };
        }

        private static void Log(string message)
        {
            if (UnityEditorInternal.InternalEditorUtility.isHumanControllingUs)
                Debug.Log(message);
            else
                Console.WriteLine(message);
        }
    }
}
