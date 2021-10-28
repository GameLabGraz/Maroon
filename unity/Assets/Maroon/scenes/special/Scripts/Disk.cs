using TMPro;
using UnityEngine;

public class Disk : MonoBehaviour
{
    [Header("Design Settings")]
    [SerializeField] protected GameObject innerModel;
    [SerializeField] protected TextMeshPro nameField;

    [SerializeField] protected GameObject activateSnapObject = null;
    
    // Start is called before the first frame update
    public void Setup(Material outsideMaterial, Material highlightImageMaterial, string displayName = "")
    {
        if (!innerModel)
            return;

        var rend = innerModel.GetComponent<Renderer>();
        if(!rend)
            return;
        
        Debug.Assert(rend.materials.Length == 2);
        var materials = rend.materials;
        materials[0] = outsideMaterial;
        materials[1] = highlightImageMaterial;
        rend.materials = materials;

        if(displayName != "")
            nameField.text = displayName;
    }

    public GameObject GetActivateSnapObject()
    {
        return activateSnapObject;
    }
}
