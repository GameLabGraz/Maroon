using UnityEngine;
using VRTK;

public class VRTK_StartPosition : VRTK_BasicTeleport
{
    [SerializeField] private VRTK_SDKManager vrtkManager; 
    // Start is called before the first frame update
    void Start()
    {
        foreach(var setup in vrtkManager.setups)
        {
            var args = BuildTeleportArgs(setup.actualBoundaries.transform, transform.position, 
                setup.actualBoundaries.transform.rotation, true);
            DoTeleport(this, args);
        }
    }
}
