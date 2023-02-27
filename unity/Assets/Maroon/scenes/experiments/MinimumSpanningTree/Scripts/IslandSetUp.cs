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

    public Object islandPrefab01;
    public Object islandPrefab02;
    public Object islandPrefab03;
    public Object islandPrefab04;

    public Transform topBorder;
    public Transform bottomBorder;
    public Transform leftBorder;
    public Transform rightBorder;

    protected List<GameObject> islandClones = new List<GameObject>();


    //public LayerMask m_LayerMask;

    // Start is called before the first frame update
    void Start()
    {
        createIslands();
    }

    Vector3 setIslandPosition()
    {
        float xRange = Random.Range(10000 * (leftBorder.position.x + 0.2f), 10000 *
            (rightBorder.position.x - 0.2f)) / 10000;
        float zRange = Random.Range(10000 * (bottomBorder.position.z + 0.2f), 10000 *
            (topBorder.position.z - 0.2f)) / 10000;
        Vector3 pos = new Vector3(xRange, transform.position.y, zRange);

        return pos;
    }

    bool checkIslandCollision(Vector3 islandPosition, Vector3 radius)
    {
        // halfExtends means its the radius so normally /2 but i want to extend the Collider region
        var collide = Physics.OverlapBox(islandPosition, radius);
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
        Object[] islandPrefabArray = { islandPrefab01, islandPrefab02, islandPrefab03, islandPrefab04 };

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

                Vector3 islandPosition = setIslandPosition();

                float angle = Random.Range(0f, 360f);
                Quaternion islandRotation = Quaternion.Euler(0, angle, 0);

                // set transform after so clones are child of Islands parent
                var island = Instantiate(islandPrefabArray[islandPrefabPicker], transform) as GameObject;

                Vector3 radius = island.transform.localScale / 1.4f;
                //Debug.Log("Scale = " + island.transform.localScale + "  island.transform.localScale / 1.4f = " + radius);
                int tmp = 0;
                while (checkIslandCollision(islandPosition, radius))
                {
                    islandPosition = setIslandPosition();
                    tmp++;
                    //just force break after too many tries -> maybe there is no free space anymore
                    if (tmp > 1000000)
                    {
                        if (radius == island.transform.localScale / 1.4f)
                        {
                            radius = island.transform.localScale / 1.75f;
                            tmp = 0;
                            Debug.Log("createIslands(): No free space -> change radius");
                        }
                        else
                        {
                            Debug.Log("createIslands(): No free space for new Island!");
                            break;
                        }
                    }

                }

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
