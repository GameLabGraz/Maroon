using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class Door : VRTK_InteractableObject
{
    public string SceneName;

    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);

        Debug.Log("Door pressed, leave experiment.");
        SceneManager.LoadScene(this.SceneName);
    }

    public override void StopUsing(GameObject usingObject)
    {
        base.StopUsing(usingObject);
    }
}
