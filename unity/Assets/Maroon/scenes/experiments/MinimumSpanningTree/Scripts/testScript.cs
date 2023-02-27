using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    //public int numOfIslands = 4;

    //public Rigidbody islandPrefab;

    public GameObject[] islands;

    // Start is called before the first frame update
    void Start()
    {
        /*Vector3 newIslandPosition = islandPrefab.position;
        newIslandPosition.x -= 0.8f;

        float angle = Random.Range(0f, 360f);

        Quaternion newIslandRotation = islandPrefab.rotation * Quaternion.Euler(0, angle, 0);

        Instantiate(islandPrefab, newIslandPosition, newIslandRotation);*/

        
        islands = GameObject.FindGameObjectsWithTag("IslandPreview");

        foreach(GameObject i in islands)
        {
            //Debug.Log("Island " + i.name);
            i.SetActive(false);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
