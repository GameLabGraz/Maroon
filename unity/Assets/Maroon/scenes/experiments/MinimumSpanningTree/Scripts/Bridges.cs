using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridges : MonoBehaviour
{
    public Object bridgeSegmentPrefab;

    public GameObject bridgeSegment;
    public GameObject islandPrefab;
    //GameObject start, end;
    //Vector3 startPoint;
    //Vector3 endPoint;
    Vector3 instantiatePosition;
    float lerpValue; //percentage
    float distance;
    int segmentsToCreate;
    static Vector3 size;
    MeshRenderer bridgeRenderer;
    MeshRenderer islandRenderer;
    static float islandHalf;
    GameObject[] islands;


    protected List<GameObject> bridge = new List<GameObject>();



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

    /*void InstantiateSegments(GameObject bridge)
    {
        Debug.Log("startPoint:  " + startPoint + " endPoint: " + endPoint + "  bridgeSize: " + size);

        Vector3 newStartPoint = Vector3.MoveTowards(startPoint, endPoint, islandHalf);
        Vector3 newEndPoint = Vector3.MoveTowards(endPoint, startPoint, islandHalf);
        Quaternion rot = Quaternion.LookRotation(newEndPoint - newStartPoint);

        segmentsToCreate = Mathf.RoundToInt(Vector3.Distance(newStartPoint, newEndPoint) / size.z);
        distance = 1f / segmentsToCreate;
        Debug.Log("segmentsToCreate:  " + segmentsToCreate + " Distance: " + Vector3.Distance(newStartPoint, newEndPoint) + "  betweenSegments: " + distance);
        for (int i = 0; i < segmentsToCreate; i++)
        {
            lerpValue += distance;

            //Debug.Log("lerpValue: " + lerpValue + " distance: " + distance);

            instantiatePosition = Vector3.Lerp(newStartPoint, newEndPoint, lerpValue);
            instantiatePosition.y = 0.9f;

            Instantiate(bridgeSegment, instantiatePosition, rot, bridge.transform);
        }

        Debug.Log("segments created!");
    }*/

    /*public void startTestBridge()
{
    islands = GameObject.FindGameObjectsWithTag("Island");
    //Debug.Log("Find Island Clones:  " + islands.Length);
    if (islands.Length >= 1)
    {
        startPoint = islands[0].transform.position;
        endPoint = islands[1].transform.position;
    }
    GameObject bridge = new GameObject("Bridge " + 1 + "--"+ 2);
    bridge.transform.parent = Bridges.transform;
    InstantiateSegments(bridge);
}*/

}
