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
    float size;
    MeshRenderer bridgeRenderer;
    MeshRenderer islandRenderer;
    static float islandHalf;
    GameObject[] islands;


    // Start is called before the first frame update
    void Start()
    {
        bridgeRenderer = bridgeSegment.GetComponent<MeshRenderer>();
        size = bridgeRenderer.bounds.size.z * 1.011f;

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
        //StartCoroutine(PrimsAlgorithm());
        StartCoroutine(PrimsAlgorithm2());
    }

    IEnumerator PrimsAlgorithm()
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

        int[] edges = new int[vertices];
        foreach (int e in edges)
        {
            edges[e] = -1;
        }

        // Dictionary<Key, value> .....
        //Dictionary<GameObject, float[]> graph = new Dictionary<GameObject, float[]>();

        Dictionary<int, float[]> graph = new Dictionary<int, float[]>();

        int[] parent = new int[vertices];
        float[] key = new float[vertices];
        bool[] MST = new bool[vertices];

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
        for (int count = 0; count < vertices - 1; count++)
        {
            int u;

            //---extra function?-------------
            float least = int.MaxValue;
            int min_index = -1;
            for (int vec = 0; vec < vertices; vec++)
            {
                if (MST[vec] == false && key[vec] < least)
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
                if (graph[u][v] != 0 && MST[v] == false && graph[u][v] < key[v])
                {
                    parent[v] = u;
                    key[v] = graph[u][v];
                    edges[u] = v;
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
            yield return new WaitForSeconds(1);
            InstantiateBridgeSegments(bridge, startPos, endPos);
        }
        for (int i = 0; i < vertices; i++)
        {
            //Debug.Log("edge: " + edges[i] + " i: " + i );
            //Debug.Log("edge: " + edges[i] + " i: " + i + " parent: " + parent[edges[i]]);
            Debug.Log("start: " + parent[edges[i]] + " end: " + edges[i] + " order: " + i);
            Debug.Log("Vergleich: parent[i] start " + parent[edges[i]] + " end: " + edges[i]);
            /*if (edges[i] != -1 && edges[i] != 0)
            {
                // von -parent[i]-  zu  -i-  mit  Distanz -graph[i][parent[i]]-
                Vector3 startPos = islands[parent[edges[i]]].transform.position;
                Vector3 endPos = islands[edges[i]].transform.position;
                GameObject bridge = new GameObject("Bridge " + parent[edges[i]] + "--" + edges[i]);
                bridge.transform.parent = Bridges.transform;
                InstantiateBridgeSegments(bridge, startPos, endPos);
            }*/
        }


        Debug.Log("End of PrimTest!");
        yield break;
    }

    void InstantiateBridgeSegments(GameObject bridge, Vector3 startPos, Vector3 endPos)
    {
        //Debug.Log("startPos:  " + startPos + " endPos: " + endPos + "  bridgeSize: " + size);

        Vector3 newStartPoint = Vector3.MoveTowards(startPos, endPos, islandHalf);
        Vector3 newEndPoint = Vector3.MoveTowards(endPos, startPos, islandHalf);
        Quaternion rot = Quaternion.LookRotation(newEndPoint - newStartPoint);

        //segmentsToCreate = Mathf.RoundToInt(Vector3.Distance(newStartPoint, newEndPoint) / size);
        float d = Vector3.Distance(newStartPoint, newEndPoint) / size;
        segmentsToCreate = (int)d;
        //Debug.Log("Segments: " + d + " : " + segmentsToCreate);
        distance = 1f / d;

        lerpValue = 0 - distance;
        //Debug.Log("segmentsToCreate:  " + segmentsToCreate + " Distance: " + Vector3.Distance(newStartPoint, newEndPoint) + "  betweenSegments: " + distance);
        //Debug.Log("segmentsToCreate:  " + segmentsToCreate);
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
        Debug.Log("min_index is: " + min_index);
        return min_index;
    }

    IEnumerator PrimsAlgorithm2()
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

        /*foreach( KeyValuePair<GameObject, float[]> kvp in edges)
        {
            foreach(float val in kvp.Value)
            { 
                Debug.Log("dict edges " + kvp.Key + "  " + val);
            }

        }*/

        // Set start vertex
        key[0] = 0;
        // Set to -1 because it's the root of the MST
        parent[0] = -1;
        for (int count = 0; count < vertices - 1; count++)
        {
            int u;

            u = FindMinEdge(key, isInMST, vertices);

            //Debug.Log("After Finding Least ");


            //-------------------------------
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
                break;
            }
        }

        Debug.Log("Now building the bridges ");

        /*---------------------------------------------------------------------------------------*/
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
                yield return new WaitForSeconds(1);
                InstantiateBridgeSegments(bridge, startPos, endPos);
            }
            
        }


        Debug.Log("End of PrimTest2!");
        yield break;
    }


}
