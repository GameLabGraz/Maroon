using System.Collections.Generic;
using UnityEngine;

public class IslandSetUp : MonoBehaviour, IResetObject
{
    [SerializeField] private GameObject islandPrefab01;
    [SerializeField] private GameObject islandPrefab02;
    [SerializeField] private GameObject islandPrefab03;
    [SerializeField] private GameObject islandPrefab04;

    [SerializeField] private Transform topLeftBorder;
    [SerializeField] private Transform bottomRightBorder;

    private static Vector3 radius;
    private int _numberOfIslands = 4;

    protected List<GameObject> islandClones = new List<GameObject>();

    /**
     * Start is called before the first frame update
     * */
    private void Start()
    {
        // 1.9f for a little extra space
        radius = islandPrefab01.GetComponent<MeshRenderer>().bounds.size / 1.9f;
        CreateIslands();
    }

    /**
     * Set a Random Position within Range
     * */
    private Vector3 SetIslandPosition()
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
    private bool CheckIslandCollision(Vector3 islandPosition, Vector3 radius)
    {
        return Physics.CheckSphere(islandPosition, radius.x, LayerMask.GetMask("Island"));
    }

    /**
     * Create all Islands on random positions in Pool
     * */
    protected void CreateIslands()
    {
        GameObject[] islandPrefabArray = { islandPrefab01, islandPrefab02, islandPrefab03, islandPrefab04 };

        int counter = 0;
        for (; counter < MSTConstants.MAX_ISLANDS; counter++)
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

                Vector3 islandPosition = SetIslandPosition();
                int tmp = 0;
                while (CheckIslandCollision(islandPosition, radius))
                {
                    islandPosition = SetIslandPosition();
                    tmp++;
                    // just force break after too many tries -> maybe there is no free space anymore
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
        //Debug.Log("createIslands(): counter= " + counter);
        for (; counter < islandClones.Count; counter++)
        {
            // break if already deactivated
            if (!islandClones[counter].activeSelf) break;
            islandClones[counter].SetActive(false);
        }
    }

    /**
     * deactivates all the islands Clones
     * */
    protected void ClearAllIslandClones()
    {
        foreach (var island in islandClones)
        {
            island.SetActive(false);
        }
    }

    /**
     * Change the number of active islands
     * */
    public void ChangeNumberOfIslands()
    {
        if (SimulationController.Instance.SimulationRunning)
        {
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
    public void SetNumberOfIslands(float numberOfIslands)
    {
        _numberOfIslands = (int)numberOfIslands;
        ChangeNumberOfIslands();
    }

    /**
     * resets all islandpositions
     * sets the number of islands active that are set in slider
     * */
    public void ResetIslandPositions()
    {
        ClearAllIslandClones();
        for (int i = 0; i < MSTConstants.MAX_ISLANDS; i++)
        {
            float angle = Random.Range(0f, 360f);
            Quaternion islandRotation = Quaternion.Euler(0, angle, 0);

            Vector3 islandPosition = SetIslandPosition();
            
            int tmp = 0;
            while (CheckIslandCollision(islandPosition, radius))
            {
                islandPosition = SetIslandPosition();
                tmp++;
                // just force break after too many tries -> maybe there is no free space anymore
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
        ChangeNumberOfIslands();
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
            //Debug.Log("IslandSetUp ResetObject(): Simulation is Running!");
        }
        ResetIslandPositions();
        MSTController.Instance.UpdateIslands();
    }
}