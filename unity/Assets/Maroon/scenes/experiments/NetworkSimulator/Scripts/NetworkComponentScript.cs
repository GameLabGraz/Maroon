using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NetworkComponentScript : MonoBehaviour
{
    private SceneSwitcher sceneSwitcher;
    public NetworkComponentType type;

    private void Start()
    {
       AddPhysics2DRaycaster();
       sceneSwitcher = FindObjectOfType<SceneSwitcher>();
    }

    void OnMouseDown()
    {
        // Destroy the gameObject after clicking on it
        Debug.Log("Clicked: ");
        sceneSwitcher.GoToRouterScene();
    }
    private void AddPhysics2DRaycaster()
    {
        PhysicsRaycaster physicsRaycaster = FindObjectOfType<PhysicsRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<PhysicsRaycaster>();
        }
    }

}
