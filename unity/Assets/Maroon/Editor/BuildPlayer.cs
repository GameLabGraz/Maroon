using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.XR.Management;

namespace Maroon.Build
{
    public class BuildPlayer
    {
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
            VR,
            MAC,
            WebGL
        }
        
        // #############################################################################################################
        // Members

        private const string ScenePath      = "Assets/Maroon/scenes";
        private const string LabPath        = ScenePath + "/laboratory";
        private const string SpecialPath    = ScenePath + "/special";
        private const string ExperimentPath = ScenePath + "/experiments";

        private const string PcExtension = ".pc.unity";
        private const string VrExtension = ".vr.unity";

        private static readonly PlayerSetOptions BuildPlayerSetOptions = new PlayerSetOptions()
        {
            BundleVersion = DateTime.UtcNow.Date.ToString("yyyyMMdd"),
            DefaultScreenWidth = 1920,
            DefaultScreenHeight = 1080,
            FullScreenMode = FullScreenMode.FullScreenWindow
        };

        // #############################################################################################################
        // Editor Build Methods

        [MenuItem("Build/Conventional Maroon/PC")]
        public static void BuildLaboratoryPC()
        {
            BuildConventionalMaroon(MaroonBuildTarget.PC);
        }

        [MenuItem("Build/Conventional Maroon/PC VR")]
        public static void BuildLaboratoryVR()
        {
            BuildConventionalMaroon(MaroonBuildTarget.VR);
        }

        [MenuItem("Build/Conventional Maroon/Mac")]
        public static void BuildLaboratoryMAC()
        {
            BuildConventionalMaroon(MaroonBuildTarget.MAC);
        }

        [MenuItem("Build/Conventional Maroon/WebGL")]
        public static void BuildLaboratoryWebGL()
        {
            BuildConventionalMaroon(MaroonBuildTarget.WebGL);
        }

        [MenuItem("Build/Standalone Experiments/PC")]
        public static void BuildExperimentsPC()
        {
            BuildStandaloneExperiments(MaroonBuildTarget.PC);
        }

        [MenuItem("Build/Standalone Experiments/PC VR")]
        public static void BuildExperimentsVR()
        {
            BuildStandaloneExperiments(MaroonBuildTarget.VR);
        }

        [MenuItem("Build/Standalone Experiments/Mac")]
        public static void BuildExperimentsMAC()
        {
            BuildStandaloneExperiments(MaroonBuildTarget.MAC);
        }

        [MenuItem("Build/Standalone Experiments/WebGL")]
        public static void BuildExperimentsWebGL()
        {
            BuildStandaloneExperiments(MaroonBuildTarget.WebGL);
        }

        [MenuItem("Build/All Platforms, Conventional and Standalone")]
        public static void BuildAll()
        {
            var buildPath = EditorUtility.SaveFolderPanel("Choose Build Location", string.Empty, "Build");

            if (buildPath.Length == 0)
            {
                return;
            }

            foreach (var buildTarget in (MaroonBuildTarget[])Enum.GetValues(typeof(MaroonBuildTarget)))
            {
                BuildConventionalMaroon(buildTarget, $"{buildPath}/Laboratory");
                BuildStandaloneExperiments(buildTarget, $"{buildPath}/Experiments");
            }
        }

        // #############################################################################################################
        // Build Methods

        private static void BuildConventionalMaroon(MaroonBuildTarget buildTarget, string buildPath = null)
        {
            if(string.IsNullOrEmpty(buildPath))
            {
                if(!UnityEditorInternal.InternalEditorUtility.isHumanControllingUs)
                {
                    return;
                }

                buildPath = EditorUtility.SaveFolderPanel("Choose Build Location", "Build", "Laboratory");

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
            SetPlayerSettings(BuildPlayerSetOptions);

            // Enable "Initialize XR on Startup" only for VR
            XRGeneralSettings.Instance.InitManagerOnStart = buildTarget == MaroonBuildTarget.VR;

            var unityBuildTarget = MaroonBuildTarget2UnityBuildTarget(buildTarget);
            var unityBuildTargetGroup = GetBuildTargetGroup(buildTarget);

            // Switch to build target platform before build
            EditorUserBuildSettings.SwitchActiveBuildTarget(unityBuildTargetGroup, unityBuildTarget);

            var buildPlayerOptions = new BuildPlayerOptions
            {
                target = unityBuildTarget,
                targetGroup = unityBuildTargetGroup,
                options = BuildOptions.None,
                locationPathName = $"{buildPath}/{buildTarget}/{GetAppName("Maroon", buildTarget)}",
                scenes = GetScenes(buildTarget)
            };

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            HandleBuildResult(report.summary);

            // Restore PlayerSettings for Editor
            SetPlayerSettings(defaultPlayerSettings);
        }

        private static void BuildStandaloneExperiments(MaroonBuildTarget buildTarget, string buildPath = null)
        {
            if (string.IsNullOrEmpty(buildPath))
            {
                if (!UnityEditorInternal.InternalEditorUtility.isHumanControllingUs)
                {
                    return;
                }

                buildPath = EditorUtility.SaveFolderPanel("Choose Build Location", "Build", "Experiments");

                if (buildPath.Length == 0)
                {
                    return;
                }
            }

            if (UnityEditorInternal.InternalEditorUtility.isHumanControllingUs)
            {
                Debug.ClearDeveloperConsole();
            }

            var sceneExtension = GetSceneExtension(buildTarget);
            var experiments = Directory.GetFiles(ExperimentPath, $"*{sceneExtension}", SearchOption.AllDirectories);
            foreach (var experiment in experiments)
            {
                BuildExperiment(experiment, buildTarget, buildPath);
            }
        }

        private static void BuildExperiment(string experiment, MaroonBuildTarget buildTarget, string buildPath)
        {
            if (string.IsNullOrEmpty(experiment) || string.IsNullOrEmpty(buildPath))
            {
                return;
            }

            var match = new Regex(@".*\\(?<experiment>\w+)\.(\w+)\.unity").Match(experiment);
            var experimentName = match.Groups["experiment"].Value;

            Log($"Start building {experimentName} experiment for {buildTarget} ...");

            var defaultPlayerSettings = GetPlayerSettings();

            // Set PlayerSettings for Build
            SetPlayerSettings(BuildPlayerSetOptions);

            var unityBuildTarget = MaroonBuildTarget2UnityBuildTarget(buildTarget);
            var unityBuildTargetGroup = GetBuildTargetGroup(buildTarget);

            // Switch to build target platform before build
            EditorUserBuildSettings.SwitchActiveBuildTarget(unityBuildTargetGroup, unityBuildTarget);

            var buildPlayerOptions = new BuildPlayerOptions
            {
                target = unityBuildTarget,
                targetGroup = unityBuildTargetGroup,
                options = BuildOptions.None,
                locationPathName = $"{buildPath}/{experimentName}/{buildTarget}/{GetAppName(experimentName, buildTarget)}",
                scenes = new []{experiment}
            };

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            HandleBuildResult(report.summary);

            // Restore PlayerSettings for Editor
            SetPlayerSettings(defaultPlayerSettings);
        }
        

        // called by github actions workflow
        public static void ActionsBuild()
        {
            var args = Environment.GetCommandLineArgs();
            
            // usage: -maroonBuildPath /path/to/build/dir -maroonBuildTarget (WebGL/PC/VR)
            // path is relative to project dir (./unity)

            // extract path and build target from commandline arguments
            var maroonBuildPath = args[Array.IndexOf(args, "-maroonBuildPath") + 1];
            var maroonBuildTarget = (MaroonBuildTarget)Enum.Parse(typeof(MaroonBuildTarget), args[Array.IndexOf(args, "-maroonBuildTarget") + 1]);

            BuildConventionalMaroon(maroonBuildTarget, maroonBuildPath);
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

            var scenes = new List<string>
            {
                // Add Bootstrapping
                $"{SpecialPath}/Bootstrapping{sceneExtension}",

                // Add Loading
                // TODO

                // Add main menu
                $"{SpecialPath}/MainMenu{sceneExtension}",

                // Add laboratory
                $"{LabPath}/Laboratory{sceneExtension}"
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

        private static string GetAppName(string name, MaroonBuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case MaroonBuildTarget.WebGL:
                    return "";
                case MaroonBuildTarget.MAC:
                    return $"{name}.app";
                case MaroonBuildTarget.PC:
                    return $"{name}.exe";
                case MaroonBuildTarget.VR:
                    return $"{name}.exe";
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
                    Log($"Saved build to: {summary.outputPath}");
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
