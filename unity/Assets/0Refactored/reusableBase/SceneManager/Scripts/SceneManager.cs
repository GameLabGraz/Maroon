using UnityEngine;

namespace Maroon
{
    class SceneManager : MonoBehaviour
    {
        public CustomSceneAsset test;

        [SerializeField] private Maroon.SceneCategory[] SceneCategories;

        //
        public void Start()
        {
            Debug.Log(test.ScenePath);
            Debug.Log(test.SceneName);
            Debug.Log(test.IsVirtualRealityScene);
        }

        public bool LoadScene(bool showLoadingScreen = false)
        {
            return true;
        }

    }
}
