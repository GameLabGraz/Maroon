using System;
using System.IO;
using UnityEngine;


namespace Maroon.GlobalEntities.SettingsManager
{
    public static class SaveHelper
    {
        private static string SETTINGS_SAVE_PATH = Path.Combine(Application.persistentDataPath, "settings.json");

        public static void SaveSettingsResource(SettingsResource settingsResource)
        {
            string json = JsonUtility.ToJson(settingsResource);
            SaveToFile(SETTINGS_SAVE_PATH, json);
        }

        public static SettingsResource LoadSettingsResource()
        {
            const string DEFAULT_VALUE = "";
            string json = TryLoadFile(SETTINGS_SAVE_PATH, DEFAULT_VALUE);

            if (!json.Equals(DEFAULT_VALUE))
            {
                try
                {
                    SettingsResource settingsResource = JsonUtility.FromJson<SettingsResource>(json);
                    return settingsResource;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error applying saved JSON to SettingsResource: {e.Message}.");
                }
            }

            return new SettingsResource();
        }

        public static string TryLoadFile(string filePath, string defaultValue)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // Saving not supported in WebGL
            return defaultValue;
#endif

            if (File.Exists(filePath))
            {
                try
                {
                    string text = File.ReadAllText(filePath);
                    return text;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error reading file \"{filePath}\": {e.Message}.");
                }
            }

            return defaultValue;
        }

        public static void SaveToFile(string filePath, string content)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // Saving not supported in WebGL
            return;
#endif
            try
            {
                File.WriteAllText(filePath, content);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error writing to file \"{filePath}\": {e.Message}.");
            }
        }
    }
}
