using UnityEngine;
using UnityEngine.Networking;

public class SyncSpawner : NetworkBehaviour
{
    public GameObject syncPrefab;

    public override void OnStartServer()
    {
        var sync = (GameObject)Instantiate(
            syncPrefab,
            new Vector3(0, 0, 0),
            new Quaternion(0, 0, 0, 0));
        NetworkServer.Spawn(sync);
    }
}