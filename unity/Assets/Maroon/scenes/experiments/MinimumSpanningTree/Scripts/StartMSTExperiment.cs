using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;



public class StartMSTExperiment : MonoBehaviour
{
    public GameObject bridgeSegment;
    public GameObject islandPrefab;
    public GameObject Bridges;
    Vector3 instantiatePosition;
    float lerpValue; //percentage 0 - 100%
    float distance;
    int segmentsToCreate;
    float size;
    MeshRenderer bridgeRenderer;
    MeshRenderer islandRenderer;
    static float islandHalf;
    GameObject[] islands;

    float endDistance;
    float endLengthOfBridges;
    int bridgeSegments;

    [SerializeField] private TextMeshProUGUI lengthOfBridges;
    [SerializeField] private TextMeshProUGUI numberOfBridgeSegments;


    // Start is called before the first frame update
    void Start()
    {
        bridgeRenderer = bridgeSegment.GetComponent<MeshRenderer>();
        size = bridgeRenderer.bounds.size.z * 1.011f;

        islandRenderer = islandPrefab.GetComponent<MeshRenderer>();
        islandHalf = islandRenderer.bounds.size.x / 3f;

        endDistance = 0;
        endLengthOfBridges = 0;
        bridgeSegments = 0;

        lengthOfBridges.text = " ";
        numberOfBridgeSegments.text = " ";
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log("startPoint:  " + startPoint + " " + tt + " " + " endPoint: " + endPoint);


        //startPoint = islands[0].transform.position;
        //endPoint = islands[1].transform.position;
    }

    /**
     * Start Prims Algorithm - from Play Button
     * */
    public void PlayPrim()
    {
        endDistance = 0;
        endLengthOfBridges = 0;
        bridgeSegments = 0;
        StartCoroutine(PrimsAlgorithm());
    }


    /**
     * Calculate distance between GameObjects
     * */
    float getDistance(GameObject from, GameObject to)
    {
        return Vector3.Distance(from.transform.position, to.transform.position);
    }

    /**
     * Start PrimTest - from Button
     * */
    public void PrimTest()
    {
        endDistance = 0;
        endLengthOfBridges = 0;
        bridgeSegments = 0;
        StartCoroutine(PrimsAlgorithm());
    }

    /**
     * Instantiate Bridgesegments to Build a Bridge
     * */
    IEnumerator InstantiateBridgeSegments(GameObject bridge, Vector3 startPos, Vector3 endPos)
    {
        //Debug.Log("startPos:  " + startPos + " endPos: " + endPos + "  bridgeSize: " + size);
        float dist = Vector3.Distance(startPos, endPos);
        endLengthOfBridges += dist;
        Debug.Log("dist " + dist + " endLenghtOfBridges: " + endLengthOfBridges);

        Vector3 newStartPoint = Vector3.MoveTowards(startPos, endPos, islandHalf);
        Vector3 newEndPoint = Vector3.MoveTowards(endPos, startPos, islandHalf);
        Quaternion rot = Quaternion.LookRotation(newEndPoint - newStartPoint);


        //segmentsToCreate = Mathf.RoundToInt(Vector3.Distance(newStartPoint, newEndPoint) / size);
        float d = Vector3.Distance(newStartPoint, newEndPoint) / size;
        segmentsToCreate = (int)d;
        bridgeSegments += segmentsToCreate + 1;
        //Debug.Log("Segments: " + d + " : " + segmentsToCreate);
        distance = 1f / d;

        lerpValue = 0 - distance;
        //Debug.Log("segmentsToCreate:  " + segmentsToCreate + " Distance: " + Vector3.Distance(newStartPoint, newEndPoint) + "  betweenSegments: " + distance);
        //Debug.Log("segmentsToCreate:  " + segmentsToCreate);
        for (int i = 0; i <= segmentsToCreate; i++)
        {
            yield return new WaitForSeconds(0.2f);
            lerpValue += distance;

            //Debug.Log("lerpValue: " + lerpValue + " distance: " + distance);

            instantiatePosition = Vector3.Lerp(newStartPoint, newEndPoint, lerpValue);
            instantiatePosition.y = 0.9f;

            Instantiate(bridgeSegment, instantiatePosition, rot, bridge.transform);
        }

        Debug.Log("segments created!");
        yield break;
        //yield return new WaitForSeconds(0);
    }

    /**
     * find vertex (Island) which is NOT in MST that has the least key (distance/edge)
     * */
    int FindMinEdge(float[] key, bool[] isInMST, int vertices)
    {
        float least = float.MaxValue;
        int min_index = -1;
        for (int vec = 0; vec < vertices; vec++)
        {
            if (isInMST[vec] == false && key[vec] < least)
            {
                least = key[vec];
                min_index = vec;
            }
        }

        endDistance += least;


        Debug.Log("min_index is: " + min_index + " least value: " + least + " endDistance: " + endDistance);
        return min_index;
    }

    /*
     * Using Prims Algorithm for creating the MST and building the Bridges
     * */
    IEnumerator PrimsAlgorithm()
    {
        islands = GameObject.FindGameObjectsWithTag("Island");
        if (islands.Length <= 0)
        {
            //Should not happen
            Debug.Log("No Islands Found:  " + islands.Length);
        }
        int vertices = islands.Length;


        int[] edges = new int[vertices];
        foreach (int e in edges)
        {
            edges[e] = -1;
        }


        Dictionary<int, float[]> graph = new Dictionary<int, float[]>();

        int[] parent = new int[vertices];
        float[] key = new float[vertices];
        bool[] isInMST = new bool[vertices];

        // i = from
        for (int i = 0; i < vertices; i++)
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
                // Add all distances to dictionary
                graph.Add(i, distances);
                //Debug.Log("graph:  " + graph[i].Length);
            }
            catch (ArgumentException)
            {
                //Console.WriteLine("An element with Key " + islands[i] + " already exists.");
                Debug.Log("An element with Key " + islands[i] + " already exists.");
            }
            
            // Initialize with MaxValue and false
            key[i] = float.MaxValue;
            isInMST[i] = false;

        }

        // Set start vertex
        key[0] = 0;
        // Set to -1 because it's the root of the MST
        parent[0] = -1;
        for (int count = 0; count < vertices - 1; count++)
        {
            // find vertex with lowest key that is NOT in MST
            int u = FindMinEdge(key, isInMST, vertices);
            
            // add vertex to MST
            isInMST[u] = true;
            
            edges[count] = u;
            //Find
            for (int v = 0; v < vertices; v++)
            {
                if (graph[u][v] != 0 && isInMST[v] == false && graph[u][v] < key[v])
                {
                    parent[v] = u;
                    key[v] = graph[u][v];
                }
            }

        }

        for(int v = 0; v < vertices; v++)
        {
            if(isInMST[v] == false)
            {
                edges[vertices - 1] = v;
                float dist = getDistance(islands[parent[v]], islands[v]);
                endDistance += dist;
                Debug.Log("last edge: " + v + " least " + dist + " endDistance " + endDistance);
                break;
            }
        }

        Debug.Log("Now building the bridges ");

        /* --------------------------------------------------------------------------------------
         * Building Bridges without order (just per island 1-9)
         * ----------------------------------------------------------------------------------- */
        /*for (int i = 1; i < vertices; i++)
        {
            // von -parent[i]-  zu  -i-  mit  Distanz -graph[i][parent[i]]-
            Vector3 startPos = islands[parent[i]].transform.position;
            Vector3 endPos = islands[i].transform.position;
            GameObject bridge = new GameObject("Bridge " + parent[i] + "--" + i);
            bridge.transform.parent = Bridges.transform;
            yield return new WaitForSeconds(1);
            InstantiateBridgeSegments(bridge, startPos, endPos);
        }*/

        for (int i = 1; i < vertices; i++)
        {
            //Debug.Log("edge: " + edges[i] + " i: " + i );
            //Debug.Log("edge: " + edges[i] + " i: " + i + " parent: " + parent[edges[i]]);
            Debug.Log("start: " + parent[edges[i]] + " end: " + edges[i] + " order: " + i);
            
            if (parent[edges[i]] != -1)// && edges[i] != -1)
            {
                Vector3 startPos = islands[parent[edges[i]]].transform.position;
                Vector3 endPos = islands[edges[i]].transform.position;
                GameObject bridge = new GameObject("Bridge " + parent[edges[i]] + "--" + edges[i]);
                bridge.transform.parent = Bridges.transform;
                //yield return new WaitForSeconds(1);
                //InstantiateBridgeSegments(bridge, startPos, endPos);
                yield return InstantiateBridgeSegments(bridge, startPos, endPos);
                lengthOfBridges.text = "length of all bridges: " + endLengthOfBridges.ToString();
                numberOfBridgeSegments.text = "bridgesegments: " + bridgeSegments.ToString();
            }
            
        }

        lengthOfBridges.text =  "length of all bridges: " + endLengthOfBridges.ToString();
        numberOfBridgeSegments.text = "bridgesegments: " + bridgeSegments.ToString();
        Debug.Log("End of Prims Algorithm!");
        yield break;
    }

}
