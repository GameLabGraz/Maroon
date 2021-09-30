using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;
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
        Maroon.SceneManager.Instance.LoadSceneRequest(this.targetScene);
    }

    // Start is called before the first frame update
    void Awake()
    {
        // Detect Scene type
        if(Maroon.PlatformManager.Instance.CurrentPlatformIsVR)
        {
            this.targetScene = targetLabSceneVR;
        }
        else
        {
            this.targetScene = targetLabScenePC;
        }
        
        // // Prepare scene for VR entering
        // if(Maroon.PlatformManager.Instance.CurrentPlatformIsVR)
        // {
        //     // Highlighter
        //     var highlighter = this.gameObject.AddComponent<VRTK_InteractObjectHighlighter>();
        //     highlighter.touchHighlight = this.highlightColor;
        //     highlighter.objectToHighlight = this.highlightMesh;
        //
        //     // Interactable Object
        //     var interactableObject = this.gameObject.AddComponent<VRTK_InteractableObject>();
        //     interactableObject.isUsable = true;
        //     interactableObject.InteractableObjectUsed += (sender, e) => this.EnterScene();
        // }
    }

    // PC enter scene
    private void OnTriggerStay(Collider other)
    {
        if(!(Maroon.PlatformManager.Instance.CurrentPlatformIsVR))
        {
            if (!PlayerUtil.IsPlayer(other.gameObject))
                return;

            if (Input.GetKey(KeyCode.Return))
                this.EnterScene();
        }
    }
}
