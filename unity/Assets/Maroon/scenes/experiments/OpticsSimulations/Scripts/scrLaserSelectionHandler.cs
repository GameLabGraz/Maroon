using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.Physics;

public class scrLaserSelectionHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public GameObject currActiveLaser;
    public GameObject laserControlPanel;
    public GameObject laserPointerPrefab;

    public QuantityFloat activeLaserIntensity;
    public QuantityFloat activeLaserWavelength;

    
    void Start()
    {
        currActiveLaser = null;
    }


    public void setActiveIntensityAndWavelength(float intensity, float wavelength) 
    {
        activeLaserIntensity.Value = intensity;
        activeLaserWavelength.Value = wavelength;
    }


    public void setActiveIntensity(float intensity)
    {
        
        if(currActiveLaser != null)
        {
            currActiveLaser.GetComponent<LPProperties>().laserIntensity = intensity;
        }
    }
    public void setWavelength(float wavelength)
    {
        if (currActiveLaser != null)
        {
            currActiveLaser.GetComponent<LPProperties>().setLaserColor(wavelength);
        }
    }

    public GameObject[] getAllLaserPointers()
    {
        return GameObject.FindGameObjectsWithTag("LaserPointer"); ;
    }

    public void removeAllLaserPointers()
    {
        var lps = getAllLaserPointers();

        foreach(var lp in lps)
        {
            Destroy(lp);
        }
    }

    public void addLaserArray()
    {
        int numlasers = 7;

        Vector3 startpoint = new Vector3(-1, 1.87f, 2.0f);
        Vector3 endpoint = new Vector3(-1, 1.87f, 3.0f);

        Vector3 offset = (endpoint - startpoint) / numlasers;

        for(int i = 0; i < numlasers; i++)
        {
            Instantiate(laserPointerPrefab, startpoint + i * offset, laserPointerPrefab.transform.rotation);
        }

    }



    public void AddLaserPointer()
    {
        Instantiate(laserPointerPrefab, new Vector3(-1.0f, 1.88f, 2.5f), laserPointerPrefab.transform.rotation);
    }
    public void RemoveLaserPointer()
    {
        if(currActiveLaser != null)
        {
            Destroy(currActiveLaser);
        }

    }

    // Update is called once per frame
    void Update()
    {

        //if current active laser is null disable lasercontrolpanel
        if (currActiveLaser == null)
        {
            laserControlPanel.SetActive(false);
        }
        else
        {
            laserControlPanel.SetActive(true);
        }
        
        


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
                    if(rhit.collider.tag == "LensObject") //todo change to optical tables
                    {
                        currActiveLaser.GetComponent<scrDragObject>().makeInactive();
                        currActiveLaser = null;
                    }
                }
            }
        }
        setActiveIntensity(activeLaserIntensity);
        setWavelength(activeLaserWavelength);
    }
}
