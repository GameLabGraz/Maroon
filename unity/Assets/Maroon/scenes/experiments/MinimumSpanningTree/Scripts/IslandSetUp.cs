using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandSetUp : MonoBehaviour, IResetObject
{
    public GameObject islandPrefab01;
    public GameObject islandPrefab02;
    public GameObject islandPrefab03;
    public GameObject islandPrefab04;
    
    public Transform topLeftBorder;
    public Transform bottomRightBorder;

    static Vector3 radius;
    int _numberOfIslands = 4;

    protected List<GameObject> islandClones = new List<GameObject>();

    //public LayerMask m_LayerMask;

    private void Awake()
    {
        //Debug.Log("IslandSetUp Awake");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("IslandSetUp Start");
        // 1.9f for a little extra space
        radius = islandPrefab01.GetComponent<MeshRenderer>().bounds.size / 1.9f;
        createIslands();
    }

    /**
     * Set a Random Position within Range
     * */
    Vector3 setIslandPosition()
    {
        float xRange = Random.Range(topLeftBorder.position.x + radius.x, 
                bottomRightBorder.position.x - radius.x);
        float zRange = Random.Range(bottomRightBorder.position.z + radius.z, 
                topLeftBorder.position.z - radius.z);
        Vector3 pos = new Vector3(xRange, transform.position.y, zRange);

        return pos;
    }

    /**
     * Check for collisions
     * */
    bool checkIslandCollision(Vector3 islandPosition, Vector3 radius)
    {
        #region check Collision using Layer (NOT used!)
        // Using Layer
        //var collider = Physics.CheckSphere(islandPosition, radius.x, LayerMask.GetMask("Island"));
        //Debug.Log("Collider is " + collider);

        //Check with Tag
        /*var collide = Physics.OverlapSphere(islandPosition, radius.x);
        for (int i = 0; i < collide.Length; i++)
        {
            //Debug.Log("Collision!! Clone0" + islandClones.Count + "  " + collide[i].name);
            if (collide[i].CompareTag("Island"))
            {
                //Debug.Log("Here already is an Island!!  " + collide[i].name);
                return true;
            }
        }
        return false;
        */
        #endregion

        return Physics.CheckSphere(islandPosition, radius.x, LayerMask.GetMask("Island"));
    }

    /**
     * Create all Islands on random positions in Pool
     * */
    protected void createIslands()
    {
        GameObject[] islandPrefabArray = { islandPrefab01, islandPrefab02, islandPrefab03, islandPrefab04 };

        int counter = 0;
        for (; counter < MSTConstants.MAX_Islands; counter++)
        {
            if (counter < islandClones.Count)
            {
                islandClones[counter].SetActive(true);
            }
            else
            {
                int islandPrefabPicker = counter % 4;

                float angle = Random.Range(0f, 360f);
                Quaternion islandRotation = Quaternion.Euler(0, angle, 0);
                                
                Vector3 islandPosition = setIslandPosition();
                //Debug.Log("radius: "  + radius + "  Size: " + island.GetComponent<MeshRenderer>().bounds.size);
                int tmp = 0;
                while (checkIslandCollision(islandPosition, radius))
                {
                    islandPosition = setIslandPosition();
                    tmp++;
                    //just force break after too many tries -> maybe there is no free space anymore
                    if (tmp > 1000000)
                    {
                        Debug.Log("createIslands(): No free space for new Island!");
                        break;
                    }

                }
                var island = Instantiate(islandPrefabArray[islandPrefabPicker], transform) as GameObject;
                island.transform.SetPositionAndRotation(islandPosition, islandRotation);
                island.name = ("Island " + counter);

                islandClones.Add(island);
            }
        }
        counter = (int)MSTController.Instance.NumberOfIslands;
        Debug.Log("createIslands(): counter= " + counter);
        for (; counter < islandClones.Count; counter++)
        {
            // break if already deactivated
            if (!islandClones[counter].activeSelf) break;
            islandClones[counter].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {        
    }

    /**
    * deactivates all the islands Clones
    * */
    protected void clearAllIslandClones()
    {
        foreach (var island in islandClones)
        {
            island.SetActive(false);
        }
    }

    /**
    * Change the number of active islands
    * */
    public void changeNumberOfIslands()
    {
        if (SimulationController.Instance.SimulationRunning)
        {
            Debug.Log("changeNumberOfIslands(): SimulationRunning");
            ResetObject();
        }

        int counter = 0;
        for (; counter < _numberOfIslands; counter++)
        {
            if (counter < islandClones.Count)
            {
                islandClones[counter].SetActive(true);
            }
        }

        for (; counter < islandClones.Count; counter++)
        {
            // break if already deactivated
            if (!islandClones[counter].activeSelf) break;
            islandClones[counter].SetActive(false);
        }
    }

    /**
    * dynamic function to setNumberOfIslands from Slider
    * */
    public void setNumberOfIslands(float numberOfIslands)
    {
        _numberOfIslands = (int)numberOfIslands;
        //Debug.Log("(with float) changed Number of Islands to: " + NumberOfIslands + " bzw. " + numberOfIslands);
        changeNumberOfIslands();
    }

    /**
     * resets all islandpositions
     * sets the number of islands active that are set in slider
     * */
    public void resetIslandPositions()
    {
        if(SimulationController.Instance.SimulationRunning)
        {
            //should already be stopped if this function is called!
            Debug.Log("resetIslandPosition(): SimulationRunning");
        }
        clearAllIslandClones();
        for (int i = 0; i < MSTConstants.MAX_Islands; i++)
        {
            float angle = Random.Range(0f, 360f);
            Quaternion islandRotation = Quaternion.Euler(0, angle, 0);

            Vector3 islandPosition = setIslandPosition();
            
            int tmp = 0;
            while (checkIslandCollision(islandPosition, radius))
            {
                islandPosition = setIslandPosition();
                tmp++;
                //just force break after too many tries -> maybe there is no free space anymore
                if (tmp > 1000000)
                {
                    Debug.Log("resetIslandPositions(): No free space for Island!");
                    break;
                }

            }
            var island = islandClones[i];
            island.transform.SetPositionAndRotation(islandPosition, islandRotation);
            island.SetActive(true);
        }
        changeNumberOfIslands();

    }

    /**
     * Resets the object
     * */
    public void ResetObject()
    {
        MSTController.Instance.ResetMSTController();
        if (SimulationController.Instance.SimulationRunning)
        {
            SimulationController.Instance.StopSimulation();
            StopAllCoroutines();
            Debug.Log("IslandSetUp ResetObject(): Simulation is Running!");
        }
        Debug.Log("IslandSetUp ResetObject(): Reset Islandpositions");
        resetIslandPositions();
        MSTController.Instance.UpdateIslands();
    }
}
