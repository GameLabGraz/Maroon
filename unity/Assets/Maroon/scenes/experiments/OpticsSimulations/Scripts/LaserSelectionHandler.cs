using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.Physics;

public class LaserSelectionHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public GameObject currActiveLaser;
    public GameObject laserControlPanel;
    public GameObject laserPointerPrefab;

    public QuantityFloat activeLaserIntensity;
    public QuantityFloat activeLaserWavelength;

    private Vector3 laserArrayStartPoint;
    private Vector3 laserArrayEndPoint;

    private Vector3 lensPos;

    
    void Start()
    {
        currActiveLaser = null;

        laserArrayStartPoint = GameObject.Find("LaserArrayStart").transform.position;
        laserArrayEndPoint = GameObject.Find("LaserArrayEnd").transform.position;
        lensPos = GameObject.Find("LensObject").transform.position;
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
    public void setActiveWavelength(float wavelength)
    {

        if (currActiveLaser != null)
        {
            currActiveLaser.GetComponent<LPProperties>().laserWavelength = wavelength;
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

        //Vector3 startpoint = new Vector3(-1, 1.87f, 2.0f); //hardcoded values?
        //Vector3 endpoint = new Vector3(-1, 1.87f, 3.0f);
        Vector3 offset = (laserArrayEndPoint - laserArrayStartPoint) / numlasers;

        for(int i = 0; i < numlasers; i++)
        {
            Instantiate(laserPointerPrefab, laserArrayStartPoint + i * offset, laserPointerPrefab.transform.rotation);
        }

    }

    private void addLaserArrayWithOptions(int numlasers, Vector3 startpoint, Vector3 endpoint, Vector3 focalpoint, float intensity, float wavelength)
    {
        Vector3 offset = (endpoint - startpoint) / numlasers;

        for(int i = 0; i< numlasers; i++)
        {
            var lpprefab = Instantiate(laserPointerPrefab, startpoint + i * offset, laserPointerPrefab.transform.rotation);
            var lpprops = lpprefab.GetComponent<LPProperties>();
            lpprops.laserIntensity = intensity;
            lpprops.laserWavelength = wavelength;
            lpprops.setLaserColor();

            Vector3 tofocalpoint = focalpoint - lpprefab.transform.position;  
            
            var angl = Vector3.SignedAngle(tofocalpoint, Vector3.right, Vector3.up);
            lpprefab.transform.rotation = Quaternion.Euler(0.0f, -(angl), -90.0f);
        }


    }

    public void addFocusedLaserArray()
    {
        addLaserArrayWithOptions(7, new Vector3(-1.5f, 1.87f, 2.0f), new Vector3(-1.5f, 1.87f, 3.0f), new Vector3(-0.5f, 1.87f, 2.5f), 1.0f, 0.886f);
    }

    public void addRGBLaserArray()
    {
        addLaserArrayWithOptions(7, new Vector3(-1, lensPos.y, 2.0f), new Vector3(-1, lensPos.y, 3.0f), new Vector3(10000.0f, lensPos.y, 2.5f), 1.0f, 0.4f);
        addLaserArrayWithOptions(7, new Vector3(-1.5f, lensPos.y, 2.0f), new Vector3(-1.5f, lensPos.y, 3.0f), new Vector3(10000.0f, lensPos.y, 2.5f), 1.0f, 0.886f);
        addLaserArrayWithOptions(7, new Vector3(-2.0f, lensPos.y, 2.0f), new Vector3(-2.0f, lensPos.y, 3.0f), new Vector3(10000.0f, lensPos.y, 2.5f), 1.0f, 0.2f);
    }

    public void AddLaserPointer()
    {
        Instantiate(laserPointerPrefab, new Vector3(-1.0f, lensPos.y, 2.5f), laserPointerPrefab.transform.rotation);
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
            currActiveLaser.GetComponent<LPProperties>().setLaserColor();
        }

        if(Input.GetMouseButtonDown(0))
        {
            //shoot ray and collide with scene
            Ray testray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rhit;
            Physics.Raycast(testray.origin, testray.direction, out rhit, Mathf.Infinity, Physics.AllLayers); // todo layermask?

            if (rhit.collider.tag == "LaserPointer")
            {
                if(currActiveLaser != null) currActiveLaser.GetComponent<DragLaserObject>().makeInactive();
                currActiveLaser = rhit.collider.gameObject;
                currActiveLaser.GetComponent<DragLaserObject>().makeActive();
            }
            else
            {
                if(! (rhit.collider.tag == "LPHandle") && currActiveLaser != null) //todo make klick on table deselect of laser.
                {
                    if (rhit.collider.gameObject.name == "OpticsTable" || rhit.collider.gameObject.name == "OpticsTable2") //todo change to optical tables
                    {
                        currActiveLaser.GetComponent<DragLaserObject>().makeInactive();
                        currActiveLaser = null;
                    }
                }
            }
        }
        setActiveIntensity(activeLaserIntensity);
        setActiveWavelength(activeLaserWavelength);
    }
}
