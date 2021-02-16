using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlatformControls.BaseControls
{
    public class EnterScene : MonoBehaviour
    {
        [SerializeField]
        private string _sceneName;

        public void Enter()
        {
            Maroon.CustomSceneAsset asset = Maroon.SceneManager.Instance.GetSceneAssetBySceneName(_sceneName);
            Maroon.SceneManager.Instance.LoadSceneRequest(asset);
        }
    }
}
