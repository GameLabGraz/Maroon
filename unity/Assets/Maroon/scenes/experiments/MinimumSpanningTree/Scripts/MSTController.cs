using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Maroon.UI; // for DialogueManager
using GEAR.Localization; // for LanguageManager

public class MSTConstants
{
    public const int MAX_ISLANDS = 10;
}

// options for selecting start- and endislands manually
enum ManualIslandPickerOptions
{
    NoneSelected,
    OneSelected,
    BothSelected
}

public class MSTController : MonoBehaviour
{
    [SerializeField] private GameObject IslandsParent;
    [SerializeField] private GameObject BridgesParent;
    [SerializeField] private GameObject ManualBridgesParent;

    private readonly float bridgeHeightY = 0.82f;
    [SerializeField] private GameObject bridgeSegmentPrefab;
    [SerializeField] private GameObject bridgeSegmentGreyPrefab;
    [SerializeField] private GameObject islandPrefab;

    private int _numberOfIslands = 4;
    public float NumberOfIslands
    {
        set => _numberOfIslands = ((int)value);
        get => _numberOfIslands;
    }

    private float size;
    private MeshRenderer bridgeRenderer;
    private MeshRenderer islandRenderer;
    private static float islandHalf;
    private GameObject[] islands;

    private int[] startIndices;
    private int[] endIndices;

    private Coroutine routineInstantiatePrim;
    private Coroutine routineInstantiateManual;

    // To Build Bridges Manually
    [SerializeField] private GameObject FromButton;
    [SerializeField] private GameObject ToButton;
    private GameObject manualFromIsland;
    private int manualFromIslandIndex;
    private GameObject manualToIsland;
    private int isInManualSetCounter;
    private bool[] isInManualSet;
    private int manualStart;
    private ManualIslandPickerOptions manualCases;

    private int allManualBridgeSegments;

    // to show in DialogueManager
    private int allBridgeSegments;
    [SerializeField] private TextMeshProUGUI PseudoCode;

    private DialogueManager _dialogueManager;

    public static MSTController Instance { get; private set; } // static singleton

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //Debug.Log("Awake This is: " + this.name);
        }
        else
        {
            Destroy(gameObject);
        }
        routineInstantiatePrim = null;
        routineInstantiateManual = null;
    }

    /**
     * Start is called before the first frame update
     * */
    private void Start()
    {
        bridgeRenderer = bridgeSegmentPrefab.GetComponent<MeshRenderer>();
        size = bridgeRenderer.bounds.size.z * 1.011f;

        islandRenderer = islandPrefab.GetComponent<MeshRenderer>();
        islandHalf = islandRenderer.bounds.size.x / 3f;

        allBridgeSegments = 0;

        allManualBridgeSegments = 0;

        SetPseudoCode();

        islands = GameObject.FindGameObjectsWithTag("Island");
        if (islands.Length <= 0)
        {
            // Should not happen
            Debug.Log("No Islands Found:  " + islands.Length);
        }
        //Debug.Log("islandsAmount: " + islands.Length);

        isInManualSet = new bool[MSTConstants.MAX_ISLANDS];
        for(int i = 0; i < isInManualSet.Length; i++)
        {
            isInManualSet[i] = false;
        }
        manualStart = -1;
        manualCases = ManualIslandPickerOptions.NoneSelected;
        isInManualSetCounter = 0;

        if (_dialogueManager == null)
        {
            _dialogueManager = FindObjectOfType<DialogueManager>();
        }

        DisplayMessageByKey("welcome message");

        PrimsAlgorithm();
        CalculateMSTDistance();
    }

    /**
     * Update the islands[] when Slider for number of islands changes
     * and recalculate the MST with Prims Algorithm
     * */
    public void UpdateIslands()
    {
        allBridgeSegments = 0;
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
    private float GetDistance(GameObject from, GameObject to)
    {
        return Vector3.Distance(from.transform.position, to.transform.position);
    }

    #region Calculate Prims Algorithm

    /**
     * find vertex (Island) which is NOT in MST that has the least key (distance/edge)
     * */
    private int FindMinEdge(float[] key, bool[] isInMST, int vertices)
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
        //Debug.Log("min_index is: " + min_index + " least value: " + least);
        return min_index;
    }

    /**
     * Using Prims Algorithm for creating the MST and building the Bridges
     * */
    private void PrimsAlgorithm()
    {
        int vertices = _numberOfIslands;

        // to get right order for building bridges
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
                distances[j] = GetDistance(islands[i], islands[j]);
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

            // update values for adjacent vertices
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
                break;
            }
        }
        // set globally to build bridges later
        startIndices = parent;
        endIndices = edges;
    }

    /**
     * Sum up all Segments needed for a bridge between two islands
     * Called from function CalculateMSTDistance()
     * to calculate global variable to show complete bridgelenght of MST
     * */
    private void CalculateSegmentsToCreate(Vector3 startPos, Vector3 endPos)
    {
        Vector3 newStartPoint = Vector3.MoveTowards(startPos, endPos, islandHalf);
        Vector3 newEndPoint = Vector3.MoveTowards(endPos, startPos, islandHalf);

        float d = Vector3.Distance(newStartPoint, newEndPoint) / size;
        allBridgeSegments += (int)d + 1;
    }

    /**
     * Calculate all distances in MST with Prims Algorithm
     * */
    private void CalculateMSTDistance()
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

    #region Prim Bridge Building (Start and Stop Algorithm)

    /**
     * Start Prims Algorithm and Build Bridges - from Play Button
     * */
    public void PlayPrim()
    {
        DeletePrimsBridges();
        PrimsAlgorithm();
        StartCoroutine(BuildBrigdesPrim());
    }

    public void StopPrim()
    {
        if (routineInstantiatePrim != null)
        {
            StopCoroutine(routineInstantiatePrim);
        }
        StopCoroutine(BuildBrigdesPrim());
        DeletePrimsBridges();
    }

    /**
     * Build Bridges according to Prims Algorithm
     * */
    private IEnumerator BuildBrigdesPrim()
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
                GameObject bridge = new GameObject("Bridge " + parent[edges[i]] + "--" + edges[i]);
                bridge.transform.parent = BridgesParent.transform;
                routineInstantiatePrim = StartCoroutine(InstantiateBridgeSegments(bridge, startPos, endPos, bridgeSegmentPrefab));
                yield return routineInstantiatePrim;
            }
        }
    }

    #endregion

    #region ManualBridgeBuilding

    /**
     * Check for collisions between the two selected islands and build the bridge
     * */
    private IEnumerator ManualBridgeBuilder(GameObject manualFromIsland, GameObject manualToIsland, int fromIndex, int toIndex)
    {
        Vector3 startPos = manualFromIsland.transform.position;
        Vector3 endPos = manualToIsland.transform.position;

        Ray ray = new Ray(startPos, (endPos - startPos));

        float dist = GetDistance(manualFromIsland, manualToIsland);

        if(Physics.Raycast(ray, out RaycastHit hit, dist, LayerMask.GetMask("Island")) && hit.collider.gameObject != manualToIsland)
        {
            //Debug.Log(hit.collider.gameObject.name + " was hit!");
            yield break;
        }

        // Calculate here to show in UI
        Vector3 newStartPoint = Vector3.MoveTowards(startPos, endPos, islandHalf);
        Vector3 newEndPoint = Vector3.MoveTowards(endPos, startPos, islandHalf);
        float d = Vector3.Distance(newStartPoint, newEndPoint) / size;
        allManualBridgeSegments += (int)d + 1;

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
        routineInstantiateManual = StartCoroutine(InstantiateBridgeSegments(bridge, startPos, endPos, bridgeSegmentGreyPrefab));
        yield return routineInstantiateManual;

        if (isInManualSetCounter == _numberOfIslands)
        {
            yield return new WaitForSeconds(0.5f);
            if(allBridgeSegments == allManualBridgeSegments)
            {
                DisplayMessageByKey("islands manually connected minimum case");
            }
            else
            {
                var message = LanguageManager.Instance.GetString("islands manually connected");
                DisplayMessage(string.Format(message, allManualBridgeSegments, allBridgeSegments));
            }
        }
        yield break;
    }

    /**
     * Select start- and endisland manually from MouseDownEvent
     * highlight selected islands
     * and build a bridge between them
     * */
    public IEnumerator SelectIsland(string text)
    {
        var message = LanguageManager.Instance.GetString("SelectIsland");
        string _text = text;
        int index = -1;
        // Child in Prefab Islands -> Spotlight 
        Transform light;

        if (isInManualSetCounter >= _numberOfIslands)
        {
            yield break;
        }

        for (int c = 0; c < _numberOfIslands; c++)
        {
            if (String.Compare(islands[c].name, _text) == 0)
            {
                index = c;
            }
        }

        if (index < 0)
        {
            Debug.Log("Error - Selected Island not found!");
        }

        if (manualStart < 0)
        {
            //Debug.Log("Manual Start Island is: " + index);
            manualStart = index;
        }

        switch(manualCases)
        {
            case ManualIslandPickerOptions.NoneSelected:
                manualFromIsland = islands[index];
                light = manualFromIsland.transform.GetChild(0);
                light.gameObject.SetActive(true);
                SetFromButton(manualFromIsland.name);
                manualFromIslandIndex = index;
                manualCases = ManualIslandPickerOptions.OneSelected;
                break;
            case ManualIslandPickerOptions.OneSelected:
                if (manualFromIsland != islands[index] && 
                    ((isInManualSet[index] ^ isInManualSet[manualFromIslandIndex]) || 
                    ((manualStart == index || manualStart == manualFromIslandIndex) && (!isInManualSet[index] && !isInManualSet[manualFromIslandIndex]))) )
                {
                    manualToIsland = islands[index];
                    light = manualToIsland.transform.GetChild(0);
                    light.gameObject.SetActive(true);
                    SetToButton(manualToIsland.name);
                    manualCases = ManualIslandPickerOptions.BothSelected;
                    // Check for Collisions and build Bridge
                    yield return ManualBridgeBuilder(manualFromIsland, manualToIsland, manualFromIslandIndex, index);
                }
                if (manualCases == ManualIslandPickerOptions.BothSelected)
                {
                    light = manualFromIsland.transform.GetChild(0);
                    light.gameObject.SetActive(false);
                    SetFromButton(message);
                    light = manualToIsland.transform.GetChild(0);
                    light.gameObject.SetActive(false);
                    SetToButton(message);
                    manualCases = ManualIslandPickerOptions.NoneSelected;
                }
                break;
            case ManualIslandPickerOptions.BothSelected:
                light = manualFromIsland.transform.GetChild(0);
                light.gameObject.SetActive(false);
                SetFromButton(message);
                light = manualToIsland.transform.GetChild(0);
                light.gameObject.SetActive(false);
                SetToButton(message);
                manualCases = ManualIslandPickerOptions.NoneSelected;
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

    /**
     * delete the already built manual bridges
     * */
    public void TryAgain()
    {
        if (routineInstantiateManual != null)
        {
            StopCoroutine(routineInstantiateManual);
        }
        ResetManualBridges();
    }

    #endregion

    #region Instantiate Bridge

    /**
     * Instantiate Bridgesegments to Build a Bridge
     * */
    private IEnumerator InstantiateBridgeSegments(GameObject bridge, Vector3 startPos, Vector3 endPos, GameObject bridgeSegment)
    {
        //Debug.Log("startPos:  " + startPos + " endPos: " + endPos);
        Vector3 newStartPoint = Vector3.MoveTowards(startPos, endPos, islandHalf);
        Vector3 newEndPoint = Vector3.MoveTowards(endPos, startPos, islandHalf);

        Quaternion rot = Quaternion.LookRotation(newEndPoint - newStartPoint);

        float d = Vector3.Distance(newStartPoint, newEndPoint) / size;

        int segmentsToCreate = (int)d;
        float distance = 1f / d;

        // percentage 0 - 100%
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
        //Debug.Log("segments created!");
        yield break;
    }

    #endregion

    #region Reset everything

    /**
     * destroy all manual built bridges
     * */
    private void DeleteManualBridges()
    {
        foreach (Transform child in ManualBridgesParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /**
     * destroy all with Prims Algorithm built bridges
     * */
    private void DeletePrimsBridges()
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

        allManualBridgeSegments = 0;
        for (int i = 0; i < isInManualSet.Length; i++)
        {
            isInManualSet[i] = false;
        }
        isInManualSetCounter = 0;
        manualStart = -1;
        manualCases = ManualIslandPickerOptions.NoneSelected;
        var message = LanguageManager.Instance.GetString("SelectIsland");
        SetFromButton(message);
        SetToButton(message);
        // spotlights on islands should be deactivated
        foreach (GameObject island in islands)
        {
            island.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    /**
     * resets the object
     * called from ResetObject in IslandSetUp
     * */
    public void ResetMSTController()
    {
        StopAllCoroutines();
        DeletePrimsBridges();
    }

    #endregion

    #region UI messages

    /**
     *  Show message by key from LanguageManager in Helpi's dialogue
     * */
    private void DisplayMessageByKey(string key)
    {
        if (_dialogueManager == null)
            return;

        var message = LanguageManager.Instance.GetString(key);

        _dialogueManager.ShowMessage(message);
    }

    /**
     *  Show message string in Helpi's dialogue
     * */
    private void DisplayMessage(string message)
    {
        if (_dialogueManager == null)
            return;

        _dialogueManager.ShowMessage(message);
    }

    /**
     * Set PseudoCode in UI Text field
     * */
    private void SetPseudoCode()
    {
        var myPseudoCode =
            "  <style=\"Normal\">mst = empty set</style>\n" +
            "  <style=\"Normal\">startVertex = first vertex in graph</style>\n" +
            "  <style=\"Normal\">mst.<style=\"sortingFunction\">add</style>(startVertex)</style>\n\n" +

            "  <style=\"Normal\">edgesToCheck = edges connected to startVertex</style>\n\n" +

            "  <style=\"sortingKeyword\">while</style><style=\"Normal\"> mst <style=\"sortingNumber\">has fewer vertices than</style> graph:</style>\n" +
            "      <style=\"Normal\">minEdge, minWeight = <style=\"sortingFunction\">findMinEdge</style><style=\"Normal\">(edges)</style>\n\n" +

            "  <style=\"Normal\">mst.</style><style=\"sortingFunction\">add</style><style=\"Normal\">(minEdge)</style>\n\n" +

            "  <style=\"sortingKeyword\">for</style><style=\"Normal\"> edge </style><style=\"sortingKeyword\">in</style><style=\"Normal\"> edges connected to minEdge:</style>\n" +
            "      <style=\"sortingKeyword\">if</style><style=\"Normal\"> edge <style=\"sortingNumber\">is not</style> in mst:</style>\n" +
            "          <style=\"Normal\">edges.</style><style=\"sortingFunction\">add</style><style=\"Normal\">(edge)</style>\n\n" +

            "  <style=\"Normal\">edges.</style><style=\"sortingFunction\">remove</style><style=\"Normal\">(minEdge)</style>\n\n" +

            "  <style=\"sortingFunction\">return</style><style=\"Normal\"> mst as an array</style>\n\n";

        PseudoCode.text = myPseudoCode;
    }

    #endregion

}