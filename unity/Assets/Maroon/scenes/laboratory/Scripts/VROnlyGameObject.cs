using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VROnlyGameObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var isVR = Maroon.GlobalEntities.PlatformManager.Instance.CurrentPlatformIsVR;
        gameObject.SetActive(isVR);
    }

}
