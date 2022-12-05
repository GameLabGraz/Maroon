namespace Tests.Utilities
{
    public static class Constants
    {
        public const string TypePC = "pc";
        public const string TypeVR = "vr";
        
        /// <summary>
        /// template experiment prefab name for PC scenes
        /// </summary>
        public const string ExperimentPrefabName = "ExperimentSetting";
        
        /// <summary>
        /// Reason for skipping an outdated scene
        /// </summary>
        public const string ReasonItsOutdated = "scene is not using the up-to-date 'ExperimentSetting' prefab";
    }
}