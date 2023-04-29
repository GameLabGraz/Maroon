using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Maroon.UI; // for DialogueManager
using GEAR.Localization; // for LanguageManager

public class MSTConstants
{
    public const int MAX_Islands = 10;
}

// options for selecting start- and endislands manually
enum ManualIslandPickerOptions
{
    noneSelected,
    oneSelected,
    bothSelected
}

public class MSTController : MonoBehaviour
{
    public GameObject IslandsParent;
    public GameObject BridgesParent;
    public GameObject ManualBridgesParent;

    public float bridgeHeightY = 0.9f;
    public GameObject bridgeSegmentPrefab;
    public GameObject bridgeSegmentGreyPrefab;
    public GameObject islandPrefab;

    private int _numberOfIslands = 4;
    public float NumberOfIslands
    {
        set => _numberOfIslands = ((int)value);
        get => _numberOfIslands;
    }

    float size;
    MeshRenderer bridgeRenderer;
    MeshRenderer islandRenderer;
    static float islandHalf;
    GameObject[] islands;

    public GameObject FromButton;
    public GameObject ToButton;
    GameObject manualFromIsland;
    int manualFromIslandIndex;
    GameObject manualToIsland;
    int isInManualSetCounter;
    bool[] isInManualSet;
    int manualStart;
    ManualIslandPickerOptions manualCases;
    float endLengthOfManualBridges;
    int allManualBridgeSegments;


    int[] startIndices;
    int[] endIndices;

    // to show in UI
    //float endDistance;
    float endLengthOfBridges;
    int allBridgeSegments;
    //[SerializeField] private TextMeshProUGUI lengthOfBridges;
    //[SerializeField] private TextMeshProUGUI numberOfBridgeSegments;

    private DialogueManager _dialogueManager;


    public static MSTController Instance { get; private set; } // static singleton
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("Awake This is: " + this.name);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        bridgeRenderer = bridgeSegmentPrefab.GetComponent<MeshRenderer>();
        size = bridgeRenderer.bounds.size.z * 1.011f;

        islandRenderer = islandPrefab.GetComponent<MeshRenderer>();
        islandHalf = islandRenderer.bounds.size.x / 3f;

        //endDistance = 0;
        endLengthOfBridges = 0;
        allBridgeSegments = 0;

        endLengthOfManualBridges = 0;
        allManualBridgeSegments = 0;


        //lengthOfBridges.text = " ";
        //numberOfBridgeSegments.text = " ";


        islands = GameObject.FindGameObjectsWithTag("Island");
        if (islands.Length <= 0)
        {
            //Should not happen
            Debug.Log("No Islands Found:  " + islands.Length);
        }
        Debug.Log("islandsAmount: " + islands.Length);

        isInManualSet = new bool[MSTConstants.MAX_Islands];
        for(int i = 0; i < isInManualSet.Length; i++)
        {
            isInManualSet[i] = false;
        }
        manualStart = -1;
        manualCases = ManualIslandPickerOptions.noneSelected;
        isInManualSetCounter = 0;

        DisplayMessageByKey("welcome message");
    }

    // Update is called once per frame
    void Update()
    {
    }

    /**
     * Update the islands[] when Slider for number of islands changes
     * and recalculate the MST with Prims Algorithm
     * */
    public void UpdateIslands()
    {
        endLengthOfBridges = 0;
        allBridgeSegments = 0;

        endLengthOfManualBridges = 0;
        allManualBridgeSegments = 0;

        islands = GameObject.FindGameObjectsWithTag("Island");
        if (islands.Length <= 0)
        {
            //Should not happen
            Debug.Log("No Islands Found:  " + islands.Length);
        }
        PrimsAlgorithm();
        CalculateMSTDistance();

        ResetManualBridges();
    }

    /**
     * Calculate distance between GameObjects
     * */
    float getDistance(GameObject from, GameObject to)
    {
        return Vector3.Distance(from.transform.position, to.transform.position);
    }

    #region Calculate Prims Algorithm

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
        //endDistance += least;
        //Debug.Log("min_index is: " + min_index + " least value: " + least + " endDistance: " + endDistance);
        return min_index;
    }

    /**
     * Using Prims Algorithm for creating the MST and building the Bridges
     * */
    void PrimsAlgorithm()
    {
        //endDistance = 0;
        int vertices = _numberOfIslands;

        //to get right order for building bridges
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

            // find
            for (int v = 0; v < vertices; v++)
            {
                if (graph[u][v] != 0 && isInMST[v] == false && graph[u][v] < key[v])
                {
                    parent[v] = u;
                    key[v] = graph[u][v];
                }
            }
        }

        // add last found islandconnection to edges[]
        for (int v = 0; v < vertices; v++)
        {
            if (isInMST[v] == false)
            {
                edges[vertices - 1] = v;
                float dist = getDistance(islands[parent[v]], islands[v]);
                //endDistance += dist;
                //Debug.Log("last edge: " + v + " least " + dist + " endDistance " + endDistance);
                break;
            }
        }

        //set globally to build bridges later
        startIndices = parent;
        endIndices = edges;

        Debug.Log("End of Prims Algorithm!");
    }

    /**
     * Sum up all Segments needed for a bridge between two islands
     * Called from function CalculateMSTDistance()
     * to calculate global variable to show complete bridgelenght of MST
     * */
    void CalculateSegmentsToCreate(Vector3 startPos, Vector3 endPos)
    {
        float dist = Vector3.Distance(startPos, endPos);
        endLengthOfBridges += dist;
        //Debug.Log("dist: " + dist + " endLenghtOfBridges: " + endLengthOfBridges);

        Vector3 newStartPoint = Vector3.MoveTowards(startPos, endPos, islandHalf);
        Vector3 newEndPoint = Vector3.MoveTowards(endPos, startPos, islandHalf);

        float d = Vector3.Distance(newStartPoint, newEndPoint) / size;
        allBridgeSegments += (int)d + 1;
        //Debug.Log("Segments: " + allBridgeSegments);
    }

    /**
     * Calculate all distances in MST with Prims Algorithm
     * */
    void CalculateMSTDistance()
    {
        int vertices = _numberOfIslands;
        int[] parent = startIndices;
        int[] edges = endIndices;

        for (int i = 1; i < vertices; i++)
        {
            //Debug.Log("start: " + parent[edges[i]] + " end: " + edges[i] + " order: " + i);

            if (parent[edges[i]] != -1)
            {
                Vector3 startPos = islands[parent[edges[i]]].transform.position;
                Vector3 endPos = islands[edges[i]].transform.position;
                CalculateSegmentsToCreate(startPos, endPos);
            }

        }
    }

    #endregion

    #region Prim Bridge Building

    /**
     * Start Prims Algorithm and Build Bridges - from Play Button
     * */
    public void PlayPrim()
    {
        endLengthOfBridges = 0;
        allBridgeSegments = 0;

        StopAllCoroutines();
        DeletePrimsBridges();
        UpdateIslands();
        //StartCoroutine(PrimsAlgorithm());
        PrimsAlgorithm();
        StartCoroutine(BuildBrigdesPrim());
        //Debug.Log("PlayPrim Finished!");
    }

    public void StopPrim()
    {
        StopAllCoroutines();
        DeletePrimsBridges();
    }

    /**
     * Build Bridges according to Prims Algorithm
     * */
    IEnumerator BuildBrigdesPrim()
    {
        int vertices = _numberOfIslands;
        int[] parent = startIndices;
        int[] edges = endIndices;

        for (int i = 1; i < vertices; i++)
        {
            //Debug.Log("start: " + parent[edges[i]] + " end: " + edges[i] + " order: " + i);

            if (parent[edges[i]] != -1)// && edges[i] != -1)
            {
                Vector3 startPos = islands[parent[edges[i]]].transform.position;
                Vector3 endPos = islands[edges[i]].transform.position;
                GameObject bridge = new GameObject("Bridge " + parent[edges[i]] + "--" + edges[i]);
                bridge.transform.parent = BridgesParent.transform;
                yield return InstantiateBridgeSegments(bridge, startPos, endPos, bridgeSegmentPrefab);
                //lengthOfBridges.text = "length of all bridges: " + endLengthOfBridges.ToString();
                //numberOfBridgeSegments.text = "allBridgeSegments: " + allBridgeSegments.ToString();
            }

        }
        Debug.Log("BuildBrigdesPrim Finished!");

    }

    #endregion

    #region ManualBridgeBuilding

    /**
    * Check for collisions between the two selected islands and build the bridge
    * */
    IEnumerator ManualBridgeBuilder(GameObject manualFromIsland, GameObject manualToIsland, int fromIndex, int toIndex)
    {
        Vector3 startPos = manualFromIsland.transform.position;
        Vector3 endPos = manualToIsland.transform.position;

        Ray ray = new Ray(startPos, (endPos - startPos));

        float dist = getDistance(manualFromIsland, manualToIsland);

        if(Physics.Raycast(ray, out RaycastHit hit, dist, LayerMask.GetMask("Island")) && hit.collider.gameObject != manualToIsland)
        {
            Debug.Log(hit.collider.gameObject.name + " was hit!");
        }
        else
        {
            if (!isInManualSet[manualFromIslandIndex])
            {
                isInManualSet[manualFromIslandIndex] = true;
                isInManualSetCounter++;
            }
            if (!isInManualSet[toIndex])
            {
                isInManualSet[toIndex] = true;
                isInManualSetCounter++;

            }
            GameObject bridge = new GameObject("ManualBridge " + fromIndex + "--" + toIndex);
            bridge.transform.parent = ManualBridgesParent.transform;
            yield return StartCoroutine(InstantiateBridgeSegments(bridge, startPos, endPos, bridgeSegmentGreyPrefab));
        }

        if (isInManualSetCounter == _numberOfIslands)
        {
            Debug.Log("All Islands connected (manually)!: " + isInManualSetCounter);
            yield return new WaitForSeconds(0.2f);
            DisplayMessageByKey("islands manually connected");
            ///TODO
            //Debug.Log("lengthOfBridges " + endLengthOfBridges + " numberOfBridgeSegments " + allBridgeSegments + " endDistance " + endDistance);
            Debug.Log("lengthOfBridges " + endLengthOfBridges + " numberOfBridgeSegments " + allBridgeSegments);
            //if(minimum weight)
            yield return new WaitForSeconds(1);
            DisplayMessageByKey("islands manually connected minimum case");
        }
    }

    /**
     * Select start- and endisland manually from MouseDownEvent
     * and build a bridge between them
     * */
    public IEnumerator SelectIsland(string text)
    {
        var message = LanguageManager.Instance.GetString("SelectIsland");
        string _text = text;
        int index = -1;

        for (int c = 0; c < _numberOfIslands; c++)
        {
            if (String.Compare(islands[c].name, _text) == 0)
            {
                index = c;
                Debug.Log("islandAmount.name: " + islands[c].name + "  _text: " + _text);
            }
        }

        if (index < 0)
        {
            Debug.Log("Error - Selected Island not found!");
        }

        if (manualStart < 0)
        {
            Debug.Log("Manual Start Island is: " + index);
            manualStart = index;
        }

        switch(manualCases)
        {
            case ManualIslandPickerOptions.noneSelected:
                manualFromIsland = islands[index];
                SetFromButton(manualFromIsland.name);
                manualFromIslandIndex = index;
                manualCases = ManualIslandPickerOptions.oneSelected;
                break;
            case ManualIslandPickerOptions.oneSelected:
                if (manualFromIsland != islands[index] && 
                    ((isInManualSet[index] ^ isInManualSet[manualFromIslandIndex]) || 
                    ((manualStart == index || manualStart == manualFromIslandIndex) && (!isInManualSet[index] && !isInManualSet[manualFromIslandIndex]))) )
                {
                    manualToIsland = islands[index];
                    SetToButton(manualToIsland.name);
                    manualCases = ManualIslandPickerOptions.bothSelected;
                    // Check for Collisions and build Bridge
                    yield return ManualBridgeBuilder(manualFromIsland, manualToIsland, manualFromIslandIndex, index);
                }
                if (manualCases == ManualIslandPickerOptions.bothSelected)
                {
                    SetFromButton(message);
                    SetToButton(message);
                    manualCases = ManualIslandPickerOptions.noneSelected;
                }
                break;
            case ManualIslandPickerOptions.bothSelected:
                SetFromButton(message);
                SetToButton(message);
                manualCases = ManualIslandPickerOptions.noneSelected;
                break;
            default:
                break;
        }

        yield break;
    }

    /**
     * write selected start Island + number to Button
     * */
    public void SetFromButton(string text)
    {
        string _text = text;
        //Debug.Log("SetFromButton: " + _text);
        FromButton.GetComponent<TextMeshProUGUI>().text = _text;
    }

    /**
     * write selected end Island + number to Button
     * */
    public void SetToButton(string text)
    {
        string _text = text;
        //Debug.Log("SetToButton: " + _text);
        ToButton.GetComponent<TextMeshProUGUI>().text = _text;
    }

    /**
     * hide or show all manually build bridges when toggling RadioButton
     * */
    public void HideMyTry(bool hideTry)
    {
        if (hideTry)
        {
            foreach (Transform child in ManualBridgesParent.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (Transform child in ManualBridgesParent.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    #endregion

    #region Instantiate Bridge

    /**
    * Instantiate Bridgesegments to Build a Bridge
    * */
    IEnumerator InstantiateBridgeSegments(GameObject bridge, Vector3 startPos, Vector3 endPos, GameObject bridgeSegment)
    {
        //Debug.Log("startPos:  " + startPos + " endPos: " + endPos);
        Vector3 newStartPoint = Vector3.MoveTowards(startPos, endPos, islandHalf);
        Vector3 newEndPoint = Vector3.MoveTowards(endPos, startPos, islandHalf);

        Quaternion rot = Quaternion.LookRotation(newEndPoint - newStartPoint);

        float d = Vector3.Distance(newStartPoint, newEndPoint) / size;

        //segmentsToCreate = Mathf.RoundToInt(Vector3.Distance(newStartPoint, newEndPoint) / size);
        int segmentsToCreate = (int)d;
        float distance = 1f / d;

        ///percentage 0 - 100%
        float lerpValue = 0 - distance;
        //Debug.Log("segmentsToCreate:  " + segmentsToCreate);
        for (int i = 0; i <= segmentsToCreate; i++)
        {
            yield return new WaitForSeconds(0.15f);
            lerpValue += distance;

            //Debug.Log("lerpValue: " + lerpValue + " distance: " + distance);

            Vector3 instantiatePosition = Vector3.Lerp(newStartPoint, newEndPoint, lerpValue);
            instantiatePosition.y = bridgeHeightY;

            Instantiate(bridgeSegment, instantiatePosition, rot, bridge.transform);
        }

        Debug.Log("segments created!");
        yield break;
    }

    #endregion

    #region Reset everything

    /**
     * destroy all manual build bridges
     * */
    void DeleteManualBridges()
    {
        foreach (Transform child in ManualBridgesParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /**
     * destroy all with Prims Algorithm build Bridges
     * */
    void DeletePrimsBridges()
    {
        foreach (Transform child in BridgesParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /**
    * resets the manually built bridges
    * */
    private void ResetManualBridges()
    {
        DeleteManualBridges();

        for (int i = 0; i < isInManualSet.Length; i++)
        {
            isInManualSet[i] = false;
        }
        isInManualSetCounter = 0;
        manualStart = -1;
        manualCases = ManualIslandPickerOptions.noneSelected;
        var message = LanguageManager.Instance.GetString("SelectIsland");
        SetFromButton(message);
        SetToButton(message);
    }

    /**
    * resets the object
    * called from ResetObject in IslandSetUp
    * */
    public void ResetMSTController()
    {
        if (SimulationController.Instance.SimulationRunning)
        {
            StopAllCoroutines();
            Debug.Log("ResetMSTController(): Simulation is Running!");
        }
        Debug.Log("MSTController: ResetObject()");
        ResetManualBridges();

        DeletePrimsBridges();
    }



    #endregion


    private void DisplayMessageByKey(string key)
    {
        if (_dialogueManager == null)
            _dialogueManager = FindObjectOfType<DialogueManager>();

        if (_dialogueManager == null)
            return;

        var message = LanguageManager.Instance.GetString(key);

        _dialogueManager.ShowMessage(message);
    }
    
}
