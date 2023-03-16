using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class StartMSTExperiment : MonoBehaviour
{
    public GameObject bridgeSegment;
    public GameObject islandPrefab;
    public GameObject Bridges;
    Vector3 instantiatePosition;
    float lerpValue; //percentage
    float distance;
    int segmentsToCreate;
    static Vector3 size;
    MeshRenderer bridgeRenderer;
    MeshRenderer islandRenderer;
    static float islandHalf;
    GameObject[] islands;


    // Start is called before the first frame update
    void Start()
    {
        bridgeRenderer = bridgeSegment.GetComponent<MeshRenderer>();
        size = bridgeRenderer.bounds.size * 1.001f;

        islandRenderer = islandPrefab.GetComponent<MeshRenderer>();
        islandHalf = islandRenderer.bounds.size.x / 3f;

    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log("startPoint:  " + startPoint + " " + tt + " " + " endPoint: " + endPoint);


        //startPoint = islands[0].transform.position;
        //endPoint = islands[1].transform.position;
    }



    float getDistance(GameObject from, GameObject to)
    {
        return Vector3.Distance(from.transform.position, to.transform.position);
    }

    public void PrimTest()
    {
        //For MST - graph T (V', E')
        //Dictionary<GameObject, GameObject> MST = new Dictionary<GameObject, GameObject>();
        //Dictionary<GameObject, EndPoint> MST = new Dictionary<GameObject, EndPoint>();

        islands = GameObject.FindGameObjectsWithTag("Island");
        if (islands.Length <= 0)
        {
            //Should not happen
            Debug.Log("No Islands Found:  " + islands.Length);
        }
        int vertices = islands.Length;


        Dictionary<int, float[]> graph = new Dictionary<int, float[]>();
        
        int[] parent = new int[vertices];
        float[] key = new float[vertices];
        bool[] MST = new bool[vertices];

        // i = from
        for(int i = 0; i < vertices; i++)
        {
            float[] distances = new float[vertices];
            // j = to
            for (int j = 0; j < vertices; j++)
            {
                distances[j] = getDistance(islands[i], islands[j]);
            }   
            // Exception if key is already used
            try
            {
                // Add all distances from island to disctionary
                //graph.Add(islands[i], distances);
                graph.Add(i, distances);
                //Debug.Log("graph:  " + graph[i].Length);
            }
            catch (ArgumentException)
            {
                //Console.WriteLine("An element with Key " + islands[i] + " already exists.");
                Debug.Log("An element with Key " + islands[i] + " already exists.");
            }
            key[i] = float.MaxValue;
            MST[i] = false;

        }

        /*foreach( KeyValuePair<GameObject, float[]> kvp in edges)
        {
            foreach(float val in kvp.Value)
            { 
                Debug.Log("dict edges " + kvp.Key + "  " + val);
            }

        }*/

        key[0] = 0;
        parent[0] = -1;
        for(int count = 0; count < vertices - 1; count++)
        {
            int u;

            //---extra function?-------------
            float least = int.MaxValue;
            int min_index = -1;
            for(int vec = 0; vec < vertices; vec++)
            {
                if(MST[vec] == false && key[vec] < least)
                {
                    least = key[vec];
                    min_index = vec;
                }
            }
            u = min_index;

            Debug.Log("After Finding Least ");


            //-------------------------------
            MST[u] = true;
            for (int v = 0; v < vertices; v++)
            {
                if(graph[u][v] != 0  && MST[v] == false && graph[u][v] < key[v])
                {
                    parent[v] = u;
                    key[v] = graph[u][v];
                }
            }
        }
        Debug.Log("Now building the bridges ");

        /*---------------------------------------------------------------------------------------*/
        for (int i = 1; i < vertices; i++)
        {
            // von -parent[i]-  zu  -i-  mit  Distanz -graph[i][parent[i]]-
            Vector3 startPos = islands[parent[i]].transform.position;
            Vector3 endPos = islands[i].transform.position;
            GameObject bridge = new GameObject("Bridge " + parent[i] + "--" + i);
            bridge.transform.parent = Bridges.transform;
            InstantiateBridgeSegments(bridge, startPos, endPos);
        }


        Debug.Log("End of PrimTest!");
    }

    void InstantiateBridgeSegments(GameObject bridge, Vector3 startPos, Vector3 endPos)
    {
        Debug.Log("startPos:  " + startPos + " endPos: " + endPos + "  bridgeSize: " + size);

        Vector3 newStartPoint = Vector3.MoveTowards(startPos, endPos, islandHalf);
        Vector3 newEndPoint = Vector3.MoveTowards(endPos, startPos, islandHalf);
        Quaternion rot = Quaternion.LookRotation(newEndPoint - newStartPoint);

        segmentsToCreate = Mathf.RoundToInt(Vector3.Distance(newStartPoint, newEndPoint) / size.z);
        float d = Vector3.Distance(newStartPoint, newEndPoint) / size.z;
        segmentsToCreate = (int)d + 1;
        Debug.Log("Segments: " + d + " : " + segmentsToCreate);
        distance = 1f / segmentsToCreate;

        lerpValue = 0 - distance;
        Debug.Log("segmentsToCreate:  " + segmentsToCreate + " Distance: " + Vector3.Distance(newStartPoint, newEndPoint) + "  betweenSegments: " + distance);
        for (int i = 0; i <= segmentsToCreate; i++)
        {
            lerpValue += distance;

            //Debug.Log("lerpValue: " + lerpValue + " distance: " + distance);

            instantiatePosition = Vector3.Lerp(newStartPoint, newEndPoint, lerpValue);
            instantiatePosition.y = 0.9f;

            Instantiate(bridgeSegment, instantiatePosition, rot, bridge.transform);
        }

        Debug.Log("segments created!");
    }














    /**************************************************************************
     * 
     * 
     * 
     * ***********************************************************************/



   
}
