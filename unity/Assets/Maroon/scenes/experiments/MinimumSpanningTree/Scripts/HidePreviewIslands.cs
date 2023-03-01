using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidePreviewIslands : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //previewIslandsParent
        foreach ( Transform child in transform )
        {
            //Debug.Log("IslandPreview: " + child.name);
            child.gameObject.SetActive(false);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
