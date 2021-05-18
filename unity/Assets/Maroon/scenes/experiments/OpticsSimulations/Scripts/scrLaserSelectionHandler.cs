using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrLaserSelectionHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject currActiveLaser;
    void Start()
    {
        currActiveLaser = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //shoot ray and collide with scene
            Ray testray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rhit;
            Physics.Raycast(testray.origin, testray.direction, out rhit, Mathf.Infinity, Physics.AllLayers); // todo layermask?

            if (rhit.collider.tag == "LaserPointer")
            {
                if(currActiveLaser != null) currActiveLaser.GetComponent<scrDragObject>().makeInactive();
                currActiveLaser = rhit.collider.gameObject;
                currActiveLaser.GetComponent<scrDragObject>().makeActive();
            }
            else
            {
                if(! (rhit.collider.tag == "LPHandle") && currActiveLaser != null) //todo make klick on table deselect of laser.
                {
                    currActiveLaser.GetComponent<scrDragObject>().makeInactive();
                    currActiveLaser = null;
                }
                
            }
        }
    }
}
