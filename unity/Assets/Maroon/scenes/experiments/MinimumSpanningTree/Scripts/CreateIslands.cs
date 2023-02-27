using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateIslands : MonoBehaviour
{
    //public int numOfIslands = 4;

    public GameObject[] islandPrefabs;

    public GameObject[] islands;

    public Transform topBorder;
    public Transform bottomBorder;
    public Transform leftBorder;
    public Transform rightBorder;

    private void Awake()
    {
        islandPrefabs = GameObject.FindGameObjectsWithTag("Island");
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 newIslandPosition = new Vector3(Random.Range(leftBorder.position.x + 0.2f, rightBorder.position.x - 0.2f),
            islandPrefabs[0].transform.position.y, Random.Range(bottomBorder.position.z + 0.2f, topBorder.position.z - 0.2f));

        float angle = Random.Range(0f, 360f);

        Quaternion newIslandRotation = /*islandPrefabs[0].transform.rotation * */Quaternion.Euler(0, angle, 0);

        int islandPicker = Random.Range(0, 3);

        Instantiate(islandPrefabs[islandPicker], newIslandPosition, newIslandRotation);

        foreach (GameObject i in islandPrefabs)
        {
            MoveIsland(i);
        }


        //islands = GameObject.FindGameObjectsWithTag("Island");

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void MoveIsland(GameObject i)
    {
        /*Vector3 newIslandPosition = new Vector3(Random.Range(leftBorder.position.x + 0.2f, rightBorder.position.x - 0.2f),
            i.transform.position.y, Random.Range(bottomBorder.position.z + 0.2f, topBorder.position.z - 0.2f));*/

        float xRange = Random.Range(10000 * (leftBorder.position.x + 0.2f), 10000 * (rightBorder.position.x - 0.2f)) / 10000;
        float zRange = Random.Range(10000 * (bottomBorder.position.z + 0.2f), 10000 * (topBorder.position.z - 0.2f)) / 10000;

        Vector3 newIslandPosition = new Vector3(xRange, i.transform.position.y, zRange);

        //Debug.Log("MoveIsland " + i.name);
        i.transform.position = newIslandPosition;
    }
}
