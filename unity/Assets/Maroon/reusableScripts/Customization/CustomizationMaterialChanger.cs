using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationMaterialChanger : MonoBehaviour
{
    [Serializable]
    public struct MaterialChangeRenderer
    {
        public MeshRenderer renderer;
        public int materialIndex;
    }

    public List<Material> materials = new List<Material>();
    public List<MaterialChangeRenderer> changingObjects = new List<MaterialChangeRenderer>();

    public void OnChangeMaterial(int selectedMaterial)
    {
        Debug.Log("CustomizationMaterialChanger: new Material index = " + selectedMaterial);
        
        if (selectedMaterial < 0 || selectedMaterial >= materials.Count || changingObjects.Count == 0)
            return;
        
        foreach (var changingObject in changingObjects)
        {
            var mats = changingObject.renderer.materials;
            mats[changingObject.materialIndex] = materials[selectedMaterial];
            changingObject.renderer.materials = mats;
        }
    }
}
