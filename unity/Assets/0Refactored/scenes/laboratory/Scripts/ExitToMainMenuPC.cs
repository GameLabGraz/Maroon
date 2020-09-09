using UnityEngine;
using Util;

public class ExitToMainMenuPC : MonoBehaviour
{
    [SerializeField] private Utilities.SceneField targetMainMenuScenePC;

    private void OnTriggerStay(Collider other)
    {
        // TODO: Use inputmap instead
        if (!PlayerUtil.IsPlayer(other.gameObject))
            return;

        if (Input.GetKeyDown(KeyCode.Return))
            MaroonNetworkManager.Instance.EnterScene(this.targetMainMenuScenePC);
    }
}
