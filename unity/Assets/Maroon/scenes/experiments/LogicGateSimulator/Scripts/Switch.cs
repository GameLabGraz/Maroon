using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject switchOffVisual;
    public GameObject switchPressedVisual;
    public GameObject output;

    bool switched = false;

    void Start()
    {
        switchOffVisual.SetActive(true);
        switchPressedVisual.SetActive(false);

        output.GetComponent<Output>().OutputValue = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Camera inputCamera = Camera.main;

            if (inputCamera == null)
            {
                return;
            }

            Ray mouseRay = inputCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            bool hitSomething = Physics.Raycast(mouseRay, out hit, Mathf.Infinity, -1, QueryTriggerInteraction.Collide);

            if (!hitSomething)
            {
                return;
            }

            if (hit.collider.gameObject == gameObject)
            {
                ToggleSwitch();
            }
        }
    }

    void ToggleSwitch()
    {
        if (switched)
        {
            switched = false;

            switchOffVisual.SetActive(true);
            switchPressedVisual.SetActive(false);

            output.GetComponent<Output>().OutputValue = 0;
        }
        else
        {
            switched = true;

            switchOffVisual.SetActive(false);
            switchPressedVisual.SetActive(true);

            output.GetComponent<Output>().OutputValue = 1;
        }
    }
}


