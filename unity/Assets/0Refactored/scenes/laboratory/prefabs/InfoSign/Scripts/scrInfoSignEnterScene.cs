using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;
using Util;

public class scrInfoSignEnterScene : MonoBehaviour
{
    // #################################################################################################################
    // Members
    [SerializeField] private Utilities.SceneField targetLabScenePC;
    [SerializeField] private Utilities.SceneField targetLabSceneVR;
    private Utilities.SceneField targetScene;

    // Settings
    [SerializeField] private Color highlightColor;
    [SerializeField] private GameObject highlightMesh;

    // Detected scene type
    private string detectedSceneType = "other";

    // #################################################################################################################
    // Methods

    // Load new scene
    public void EnterScene()
    {
        //SceneManager.LoadScene(this.targetScene);
        MaroonNetworkManager.Instance.EnterScene(this.targetScene);
    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Use TargetPlatformDetector
        // Detect Scene type
        string sceneName = SceneManager.GetActiveScene().name;
        if(sceneName.Contains(".pc"))
        {
            this.detectedSceneType = "pc";
            this.targetScene = targetLabScenePC;
        }
        else if(sceneName.Contains(".vr"))
        {
            this.detectedSceneType = "vr";
            this.targetScene = targetLabSceneVR;
        }

        // Prepare scene for VR entering
        if(this.detectedSceneType == "vr")
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
        if(this.detectedSceneType == "pc")
        {
            if (!PlayerUtil.IsPlayer(other.gameObject))
                return;

            if (Input.GetKey(KeyCode.Return))
                this.EnterScene();
        }
    }
}
