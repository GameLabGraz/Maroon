using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SyncExperiments : NetworkBehaviour
{
    public GameObject syncPrefab;

    [SyncVar(hook = "OnVdg1OnOff")]
    public bool vdg1_on_off = false;
    [SyncVar(hook = "OnVdg2OnOff")]
    public bool vdg2_on_off = false;
    //[SyncVar(hook = "OnChangeDistance")]
    //private float vdg1_distance;

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    void OnVdg1OnOff(bool value)
    {
        Debug.Log("VDG1 turned " + value);
        vdg1_on_off = value;
        SwitchVDG(value, 1);
    }

    void OnVdg2OnOff(bool value)
    {
        Debug.Log("VDG2 turned " + value);
        vdg2_on_off = value;
        SwitchVDG(value, 2);
    }

    void SwitchVDG(bool value, int exp)
    {
        //check for correct scene
        if (SceneManager.GetActiveScene().buildIndex - 1 != exp)
            return;

        GameObject vandeGraaff = GameObject.FindGameObjectWithTag("VandeGraaff");
        if (null != vandeGraaff)
        {
            VandeGraaffController vdgc = vandeGraaff.GetComponent<VandeGraaffController>();
            vdgc.On = value;
            vdgc.sound.enabled = value;
        }
    }
}