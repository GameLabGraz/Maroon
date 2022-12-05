namespace Tests.Utilities
{
    /// <summary>
    /// Collection of test related constant values
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// File ending for PC scenes and prefabs
        /// </summary>
        public const string TypePC = "pc";
        
        /// <summary>
        /// File ending for VR scenes and prefabs
        /// </summary>
        public const string TypeVR = "vr";
        
        /// <summary>
        /// template experiment room prefab name without type
        /// </summary>
        public const string ExperimentPrefabName = "ExperimentSetting.";
        
        /// <summary>
        /// Reason for skipping an outdated scene
        /// </summary>
        public const string ReasonItsOutdated = "scene is not using the up-to-date 'ExperimentSetting' prefab";

        /// <summary>
        /// Reason for skipping a scene with a missing or disabled GameObject
        /// </summary>
        public const string ReasonIntentionallyMissing = "scene intentionally disabled or removed the GameObject";
    }
}