using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridges : MonoBehaviour
{
    public Object bridgeSegmentPrefab;


    protected List<GameObject> bridge = new List<GameObject>();



    GameObject[] islands;


    // Start is called before the first frame update
    void Start()
    {
        islands = GameObject.FindGameObjectsWithTag("Island");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject findClosestIsland()
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in islands)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }

        return closest;
    }

}
