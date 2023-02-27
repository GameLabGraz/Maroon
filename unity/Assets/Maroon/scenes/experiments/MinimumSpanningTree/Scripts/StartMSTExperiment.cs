using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMSTExperiment : MonoBehaviour
{
    public GameObject bridgeSegment;
    public GameObject islandPrefab;
    //GameObject start, end;
    Vector3 startPoint;
    Vector3 endPoint;
    Vector3 instantiatePosition;
    float lerpValue; //percentage
    float distance;
    int segmentsToCreate;
    static Vector3 size;
    MeshRenderer bridgeRenderer;
    MeshRenderer islandRenderer;
    static float islandHalf;
    GameObject[] islands;



    void InstantiateSegments()
    {
        Debug.Log("startPoint:  " + startPoint + " endPoint: " + endPoint + "  bridgeSize: " + size);



        /*Vector3 vector = endPoint - startPoint;
        Quaternion rotation = Quaternion.LookRotation(Vector3.Cross(vector, Vector3.up).normalized);
        Quaternion spinCCW90 = Quaternion.Euler(0, -90, 0);
        Quaternion rot = rotation * spinCCW90;*/

        //Quaternion rot = Quaternion.LookRotation(endPoint - startPoint);

        //Vector3 newStartPoint = startPoint + (start.transform.forward * 0.5f);
        Vector3 newStartPoint = Vector3.MoveTowards(startPoint, endPoint, islandHalf);
        Vector3 newEndPoint = Vector3.MoveTowards(endPoint, startPoint, islandHalf);
        Quaternion rot = Quaternion.LookRotation(newEndPoint - newStartPoint);

        //Here we calculate how many segments will fit between the two points
        segmentsToCreate = Mathf.RoundToInt(Vector3.Distance(newStartPoint, newEndPoint) / size.z);
        //As we'll be using vector3.lerp we want a value between 0 and 1, and the distance value is the value we have to add
        distance = 1f / segmentsToCreate;
        Debug.Log("segmentsToCreate:  " + segmentsToCreate + " Distance: " + Vector3.Distance(newStartPoint, newEndPoint) + " : " + distance);
        for (int i = 0; i < segmentsToCreate; i++)
        {
            //We increase our lerpValue
            lerpValue += distance;

            //Debug.Log("lerpValue: " + lerpValue + " distance: " + distance);

            //Get the position
            instantiatePosition = Vector3.Lerp(newStartPoint, newEndPoint, lerpValue);
            instantiatePosition.y = 0.9f;




            //Instantiate the object
            Instantiate(bridgeSegment, instantiatePosition, rot);

            // set transform after so clones are child of Islands parent
            //var island = Instantiate(islandPrefabArray[islandPrefabPicker], transform) as GameObject;
        }

        Debug.Log("segments created ");
    }


    // Start is called before the first frame update
    void Start()
    {
        bridgeRenderer = bridgeSegment.GetComponent<MeshRenderer>();
        size = bridgeRenderer.bounds.size;

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

    public void startTestBridge()
    {
        islands = GameObject.FindGameObjectsWithTag("Island");
        //Vector2 tt = new Vector2();
        //Debug.Log("Find Island Clones:  " + islands.Length);
        if (islands.Length >= 1)
        {
            startPoint = islands[0].transform.position;
            endPoint = islands[1].transform.position;

            //start = islands[0];
            //end = islands[1];
            //tt = islands[0].transform.position;
        }
        InstantiateSegments();
    }
}
