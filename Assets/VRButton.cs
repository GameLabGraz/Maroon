using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using VRStandardAssets.Utils;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
[RequireComponent(typeof(VRInteractiveItem))]
public class VRButton : VRMenuItem
{
    protected override void onActivate()
    {
        base.onActivate();
        GetComponent<Button>().onClick.Invoke();
    }
}