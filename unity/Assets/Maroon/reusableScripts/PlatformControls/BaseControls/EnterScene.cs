using Maroon.GlobalEntities;
using UnityEngine;

namespace PlatformControls.BaseControls
{
    public class EnterScene : MonoBehaviour
    {
        [SerializeField]
        private string _sceneName;

        public void Enter()
        {
            var asset = SceneManager.Instance.GetSceneAssetBySceneName(_sceneName);
            SceneManager.Instance.LoadSceneRequest(asset);
        }
    }
}
