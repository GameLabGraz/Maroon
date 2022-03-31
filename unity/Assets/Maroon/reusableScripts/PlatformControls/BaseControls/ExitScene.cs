using Maroon.GlobalEntities;
using UnityEngine;

namespace PlatformControls.BaseControls
{
    public class ExitScene : MonoBehaviour
    {
        public void Exit()
        {
            SceneManager.Instance.LoadPreviousScene();
        }
    }
}
