using UnityEngine;
using Util;

public class ExitToMainMenuPC : MonoBehaviour
{
    [SerializeField] private Maroon.CustomSceneAsset targetMainMenuScenePC;

    private void OnTriggerStay(Collider other)
    {
        // TODO: Use inputmap instead
        if (!PlayerUtil.IsPlayer(other.gameObject))
            return;

        if (Input.GetKey(KeyCode.Return))
            MaroonNetworkManager.Instance.EnterScene(this.targetMainMenuScenePC);
    }
}
