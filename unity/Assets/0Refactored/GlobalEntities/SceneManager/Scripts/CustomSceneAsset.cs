using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Based on
// https://answers.unity.com/questions/242794/inspector-field-for-scene-asset.html#answer-1204071

namespace Maroon
{
    // #################################################################################################################
    // Custom Scene Asset
    [System.Serializable] public class CustomSceneAsset
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Fields

        [SerializeField] private Object unitySceneAsset;
        [SerializeField] private string unityScenePath = "";
        [SerializeField] private string unitySceneName = "";
        [SerializeField] private bool isVirtualRealityScene = false;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Getters and Properties

        public string ScenePath
        {
            get { return this.unityScenePath; }
        }

        public string SceneName
        {
            get { return this.unitySceneName; }
        }

        public string SceneNameWithoutPlatformExtension
        {
            get { return this.SceneName.Substring(0, this.SceneName.LastIndexOf('.')); }
        }

        public bool IsVirtualRealityScene
        {
            get { return this.isVirtualRealityScene; }
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Operators

        // Make it work with the existing Unity methods (LoadLevel/LoadScene)
        public static implicit operator string(Maroon.CustomSceneAsset customSceneAsset)
        {
            return customSceneAsset.ScenePath;
        }
    }

    // #################################################################################################################
    // Custom Scene Asset Property Drawer
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Maroon.CustomSceneAsset))] class CustomSceneAssetPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            var propertyUnitySceneAsset = property.FindPropertyRelative("unitySceneAsset");
            var propertyUnityScenePath = property.FindPropertyRelative("unityScenePath");
            var propertyUnitySceneName = property.FindPropertyRelative("unitySceneName");
            var propertyIsVirtualRealityScene = property.FindPropertyRelative("isVirtualRealityScene");

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if(propertyUnitySceneAsset != null)
            {
                EditorGUI.BeginChangeCheck();
                var value = EditorGUI.ObjectField(position, propertyUnitySceneAsset.objectReferenceValue, typeof(SceneAsset), false);
                if(EditorGUI.EndChangeCheck())
                {
                    propertyUnitySceneAsset.objectReferenceValue = value;
                    if(propertyUnitySceneAsset.objectReferenceValue != null)
                    {
                        var scenePath = AssetDatabase.GetAssetPath(propertyUnitySceneAsset.objectReferenceValue);
                        var assetsIndex = scenePath.IndexOf("Assets", System.StringComparison.Ordinal) + 7;
                        var extensionIndex = scenePath.LastIndexOf(".unity", System.StringComparison.Ordinal);
                        scenePath = scenePath.Substring(assetsIndex, extensionIndex - assetsIndex);
                        propertyUnityScenePath.stringValue = scenePath;
                        var sceneName = (propertyUnitySceneAsset.objectReferenceValue as SceneAsset).name;
                        propertyUnitySceneName.stringValue = sceneName;
                        if((sceneName.Length > 3) && (sceneName.Substring(sceneName.Length - 3) == ".vr"))
                        {
                            propertyIsVirtualRealityScene.boolValue = true;
                        }
                        else
                        {
                            propertyIsVirtualRealityScene.boolValue = false;
                        }
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }
    #endif
}