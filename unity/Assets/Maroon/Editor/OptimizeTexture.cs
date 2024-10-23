#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Maroon.Editor
{
    public class OptimizeTexture : ScriptableObject
    {
        private static readonly TextureImporterPlatformSettings TextureSettingsWebGL = new TextureImporterPlatformSettings()
        {
            textureCompression = TextureImporterCompression.Compressed,
            crunchedCompression = true,
            compressionQuality = 75,
            name = "WebGL",
            overridden = true,
            maxTextureSize = 512
        };

        [MenuItem("Assets/Optimize Texture for Platform/WebGL")]
        static void OptimizeTextureForWebGL()
        {
            var count = 0;

            Debug.Log("Start optimizing textures for WebGL");

            foreach (var guid in AssetDatabase.FindAssets("t:texture", null))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (textureImporter == null) continue;

                Debug.Log($"optimizing: {path}...");

                textureImporter.SetPlatformTextureSettings(TextureSettingsWebGL);
                AssetDatabase.ImportAsset(path);
                count++;
            }
            Debug.Log(count + " textures optimized");
        }
    }
}
#endif