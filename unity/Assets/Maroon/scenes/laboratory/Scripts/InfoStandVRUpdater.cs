using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoStandVRUpdater : MonoBehaviour
{
    public List<GameObject> PC_Only;
    public List<GameObject> VR_Only;
    
    // Start is called before the first frame update
    void Start()
    {
        var isVR = Maroon.PlatformManager.Instance.CurrentPlatformIsVR;
        
        foreach(var pcObj in PC_Only)
            pcObj.SetActive(!isVR);
        
        foreach(var vrObj in VR_Only)
            vrObj.SetActive(isVR);
    }

}
