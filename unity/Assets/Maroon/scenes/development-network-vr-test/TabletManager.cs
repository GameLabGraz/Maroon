using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TabletManager : MonoBehaviour
{
    enum TabletNetType
    {
        Host,
        Client
    }

    public TabletNetType net_type = null;

    /// <summary>
    ///     The SceneCategory that is currently active.
    //      For example if the player is in the physics lab/category, the physics category should be set to active.
    /// </summary>
    public TabletManager.TabletNetType Network
    {
        get
        {
            return this._activeSceneCategory;
        }

        set
        {
            // Only set if it exists in categories array
            if(System.Array.Exists(this._sceneCategories, element => element == value))
            {
                this._activeSceneCategory = value;
            }
        }
    }
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
