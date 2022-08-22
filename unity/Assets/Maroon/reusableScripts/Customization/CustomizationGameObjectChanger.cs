using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationGameObjectChanger : MonoBehaviour
{
    public List<GameObject> changingObjects = new List<GameObject>();

    public void OnChangeMaterial(int selectedMaterial)
    {
        if (selectedMaterial < 0 || selectedMaterial >= changingObjects.Count || changingObjects.Count == 0)
            return;

        for (var i = 0; i < changingObjects.Count; ++i)
        {
            changingObjects[i].SetActive(i == selectedMaterial);
        }
    }
}
