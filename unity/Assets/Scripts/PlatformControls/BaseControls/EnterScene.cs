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
            SceneManager.LoadScene(_sceneName);
        }
    }
}
