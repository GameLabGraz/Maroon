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


    public bool onUIpanel { get; set; } = false;


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

        Vector3 offset = (laserArrayEndPoint - laserArrayStartPoint) / numlasers;

        for(int i = 1; i < numlasers; i++)
        {
            Instantiate(laserPointerPrefab, laserArrayStartPoint + i * offset, laserPointerPrefab.transform.rotation);
        }

    }

    private void addLaserArrayWithOptions(int numlasers, Vector3 startpoint, Vector3 endpoint, Vector3 focalpoint, float intensity, float wavelength)
    {
        Vector3 offset = (endpoint - startpoint) / numlasers;

        for(int i = 1; i< numlasers; i++)
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
        addLaserArrayWithOptions(7, laserArrayStartPoint, laserArrayEndPoint, (laserArrayStartPoint + laserArrayEndPoint)/2 + Vector3.right*0.5f, 1.0f, 680f);
    }

    public void addFocusedLaserArrayWithOffset()
    {
        Vector3 offsetVec = - Vector3.right * 0.3f;
        addLaserArrayWithOptions(7, laserArrayStartPoint +offsetVec, laserArrayEndPoint +offsetVec , (laserArrayStartPoint + laserArrayEndPoint) / 2 + Vector3.right * 0.5f +offsetVec, 1.0f, 680f);
    }

    public void addRGBLaserArray()
    {
        addLaserArrayWithOptions(7, laserArrayStartPoint, laserArrayEndPoint, laserArrayStartPoint + Vector3.right * 10000.0f, 1.0f, 450f);
        addLaserArrayWithOptions(7, laserArrayStartPoint - Vector3.right*0.2f, laserArrayEndPoint - Vector3.right * 0.2f, laserArrayStartPoint + Vector3.right * 10000.0f, 1.0f, 500f);
        addLaserArrayWithOptions(7, laserArrayStartPoint - Vector3.right * 0.4f, laserArrayEndPoint - Vector3.right * 0.4f, laserArrayStartPoint + Vector3.right * 10000.0f, 1.0f, 700f);
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


    public void setLaserArrangements(int arr)
    {
        if(!(arr == 0))
        {
            removeAllLaserPointers();
        }
        switch (arr)
        {
            case 0: break;
            case 1: addLaserArray(); break;
            case 2: addRGBLaserArray(); break;
            case 3: addFocusedLaserArray(); break;
            case 4: addFocusedLaserArrayWithOffset(); break;
            default: break;
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
                        if (!onUIpanel)
                        {
                            currActiveLaser.GetComponent<DragLaserObject>().makeInactive();
                            currActiveLaser = null;
                        }
                       
                    }
                }
            }
        }
        setActiveIntensity(activeLaserIntensity);
        setActiveWavelength(activeLaserWavelength);
    }
}
