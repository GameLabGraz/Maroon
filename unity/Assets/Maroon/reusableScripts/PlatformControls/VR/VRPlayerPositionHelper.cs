
using GameLabGraz.VRInteraction;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VRPlayerPositionHelper : MonoBehaviour
{

    private VRPlayer _player = null;
    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<VRPlayer>();
        Debug.Assert(_player != null);

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

        var playerFeetOffset = _player.trackingOriginTransform.position - _player.feetPositionGuess;

        //2. deactivate all other player except me
        foreach (var player in players)
        {
            if (player != gameObject)
            {
                player.SetActive(false);
                var position = player.transform.position;
                //var rotation = player.transform.rotation;
                
                //_player.trackingOriginTransform.rotation = rotation;
                _player.trackingOriginTransform.position = position + playerFeetOffset;

                if (_player.leftHand.currentAttachedObjectInfo.HasValue)
                    _player.leftHand.ResetAttachedTransform(_player.leftHand.currentAttachedObjectInfo.Value);
                if (_player.rightHand.currentAttachedObjectInfo.HasValue)
                    _player.rightHand.ResetAttachedTransform(_player.rightHand.currentAttachedObjectInfo.Value);
            }
        }
    }
}
