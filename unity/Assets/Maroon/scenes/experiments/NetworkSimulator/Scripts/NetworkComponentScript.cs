using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NetworkComponentScript : MonoBehaviour, IPointerClickHandler
{
    private  SceneSwitcher sceneSwitcher;

    private void Start()
    {
       AddPhysics2DRaycaster();
       sceneSwitcher = FindObjectOfType<SceneSwitcher>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       // if(eventData.clickCount >= 2)
        Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
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

 /*   void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("gameobject " + hit.transform.name + " has been clicked on!!!");
                //Select stage    
                if (hit.transform.name == "wifi_router")
                {
                   // SceneManager.LoadScene("SceneTwo");
                }
            }
        }
    }*/
}
