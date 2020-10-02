using UnityEngine;

public class ExitToMainMenuPC : MonoBehaviour
{
    [SerializeField] private Utilities.SceneField targetMainMenuScenePC;

    private void OnTriggerStay()
    {
        // TODO: Use inputmap instead
        if (Input.GetKey(KeyCode.Return))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(this.targetMainMenuScenePC);
        }
    }
}
