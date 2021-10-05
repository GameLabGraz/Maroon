using Maroon.GlobalEntities;
using UnityEngine;
using VRTK;
using Util;

public class scrInfoSignEnterScene : MonoBehaviour
{
    // #################################################################################################################
    // Members
    [SerializeField] private Maroon.CustomSceneAsset targetLabScenePC;
    [SerializeField] private Maroon.CustomSceneAsset targetLabSceneVR;
    private Maroon.CustomSceneAsset targetScene;

    // Settings
    [SerializeField] private Color highlightColor;
    [SerializeField] private GameObject highlightMesh;

    // #################################################################################################################
    // Methods

    // Load new scene
    public void EnterScene()
    {
        SceneManager.Instance.LoadSceneRequest(this.targetScene);
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Detect Scene type
        this.targetScene = PlatformManager.Instance.CurrentPlatformIsVR ? targetLabSceneVR : targetLabScenePC;

        // Prepare scene for VR entering
        if(PlatformManager.Instance.CurrentPlatformIsVR)
        {
            // Highlighter
            var highlighter = this.gameObject.AddComponent<VRTK_InteractObjectHighlighter>();
            highlighter.touchHighlight = this.highlightColor;
            highlighter.objectToHighlight = this.highlightMesh;

            // Interactable Object
            var interactableObject = this.gameObject.AddComponent<VRTK_InteractableObject>();
            interactableObject.isUsable = true;
            interactableObject.InteractableObjectUsed += (sender, e) => this.EnterScene();
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
