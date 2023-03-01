using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandSetUp : MonoBehaviour
{
    [SerializeField] protected int numberOfIslands = 4;
    public int NumberOfIslands
    {
        get => numberOfIslands;
        set
        {
            numberOfIslands = value;
        }
    }

    public GameObject islandPrefab01;
    public GameObject islandPrefab02;
    public GameObject islandPrefab03;
    public GameObject islandPrefab04;
    
    public Transform topLeftBorder;
    public Transform bottomRightBorder;

    static Vector3 radius;

    protected List<GameObject> islandClones = new List<GameObject>();


    //public LayerMask m_LayerMask;

    // Start is called before the first frame update
    void Start()
    {
        // 1.9f for a little extra space
        radius = islandPrefab01.GetComponent<MeshRenderer>().bounds.size / 1.9f;
        createIslands();
    }

    Vector3 setIslandPosition()
    {
        float xRange = Random.Range(topLeftBorder.position.x + radius.x, 
                bottomRightBorder.position.x - radius.x);
        float zRange = Random.Range(bottomRightBorder.position.z + radius.z, 
                topLeftBorder.position.z - radius.z);
        Vector3 pos = new Vector3(xRange, transform.position.y, zRange);

        return pos;
    }

    bool checkIslandCollision(Vector3 islandPosition, Vector3 radius)
    {
        var collide = Physics.OverlapSphere(islandPosition, radius.x);

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
    }

    protected void createIslands()
    {
        GameObject[] islandPrefabArray = { islandPrefab01, islandPrefab02, islandPrefab03, islandPrefab04 };

        int counter = 0;
        for (; counter < 10; counter++)
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

                islandClones.Add(island);
            }
        }
        counter = numberOfIslands;
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

    /// <summary>
    /// Clears All the islands Clones
    /// </summary>
    protected void clearAllIslandClones()
    {
        foreach (var island in islandClones)
        {
            island.SetActive(false);
        }
    }

    /// <summary>
    /// Changed the number of islands
    /// </summary>
    /// 
    public void changeNumberOfIslands()
    {
        int counter = 0;
        for (; counter < numberOfIslands; counter++)
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

    // static function setNumberOfIslands
    public void setNumberOfIslands(int numberOfIslands)
    {
        NumberOfIslands = numberOfIslands;
        //Debug.Log("changed Number of Islands to: " + NumberOfIslands + " bzw. " + numberOfIslands);
        changeNumberOfIslands();
    }

    // dynamic function setNumberOfIslands
    public void setNumberOfIslands(float numberOfIslands)
    {
        NumberOfIslands = (int)numberOfIslands;
        //Debug.Log("(with float) changed Number of Islands to: " + NumberOfIslands + " bzw. " + numberOfIslands);
        changeNumberOfIslands();
    }


    void PrimsAlgorithm()
    {
        int numVertex = 0;
        int numEdge = 0;

        Transform islandTranform = islandClones[numVertex].gameObject.transform;
        Vector3 islandPosition = islandClones[numVertex].gameObject.transform.position;

    }
}
