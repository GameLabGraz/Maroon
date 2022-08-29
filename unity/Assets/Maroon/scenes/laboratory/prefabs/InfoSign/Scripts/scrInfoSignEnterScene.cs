using UnityEngine;
using Maroon.GlobalEntities;
using Util;

public class scrInfoSignEnterScene : MonoBehaviour
{
    // #################################################################################################################
    // Members
    [SerializeField] private Maroon.CustomSceneAsset targetLabScenePC;
    [SerializeField] private Maroon.CustomSceneAsset targetLabSceneVR;
    public Maroon.CustomSceneAsset targetScene;

    // Settings
    [Header("Setting")]
    [SerializeField] private Color highlightColor;
    [SerializeField] private GameObject highlightMesh;

    // #################################################################################################################
    // Methods

    // Load new scene
    public void EnterScene()
    {
        Debug.Log("Enter Scene : " + this.targetScene);
        Maroon.GlobalEntities.SceneManager.Instance.LoadSceneRequest(this.targetScene);
    }

    // Start is called before the first frame update
    void Awake()
    {
        // Detect Scene type
        if(Maroon.GlobalEntities.PlatformManager.Instance.CurrentPlatformIsVR)
        {
            this.targetScene = targetLabSceneVR;
        }
        else
        {
            this.targetScene = targetLabScenePC;
        }
    }
    
    // PC enter scene
    private void OnTriggerStay(Collider other)
    {
        if(!(PlatformManager.Instance.CurrentPlatformIsVR))
        {
            if (!PlayerUtil.IsPlayer(other.gameObject))
                return;

            if (Input.GetKey(KeyCode.Return))
                this.EnterScene();
        }
    }
}
