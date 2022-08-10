using UnityEngine;
using UnityEngine.SceneManagement;

public class VRPlayerPositionHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnNewSceneLoaded;
    }

    void OnNewSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RepositionPlayer();
    }

    public void RepositionPlayer()
    {
        //1. find all player objects in the scene
        var players = GameObject.FindGameObjectsWithTag("Player");

        //2. deactivate all other player except me
        foreach (var player in players)
        {
            if (player != gameObject)
            {
                player.SetActive(false);
                var position = player.transform.position;
                position.y = transform.position.y;

                var rotation = player.transform.rotation;

                transform.position = position;
                transform.rotation = rotation;
            }
        }
    }
}
