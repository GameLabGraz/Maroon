using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandCollisions : MonoBehaviour
{
    //Rigidbody thisGameObject;

    public Transform topBorder;
    public Transform bottomBorder;
    public Transform leftBorder;
    public Transform rightBorder;

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveIsland(GameObject i)
    {
        float xRange = Random.Range(10000 * (leftBorder.position.x + 0.2f), 10000 * (rightBorder.position.x - 0.2f)) / 10000;
        float zRange = Random.Range(10000 * (bottomBorder.position.z + 0.2f), 10000 * (topBorder.position.z - 0.2f)) / 10000;
        /*Vector3 newIslandPosition = new Vector3(Random.Range(leftBorder.position.x + 0.2f, rightBorder.position.x - 0.2f),
            i.transform.position.y, Random.Range(bottomBorder.position.z + 0.2f, topBorder.position.z - 0.2f));*/

        Vector3 newIslandPosition = new Vector3(xRange, i.transform.position.y, zRange);

        Debug.Log("MoveIsland " + i.name);
        i.transform.position = newIslandPosition;
    }


    // "OnTriggerEnter" wird aufgerufen, wenn ein "Collider other" in den Trigger eingetreten ist
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Island"))
        {
            Debug.Log("Collision Trigger");
            //other.gameObject.SetActive(false);
            MoveIsland(gameObject);
        }
        //Debug.Log("Collision Trigger1");
    }*/


    // "OnTriggerStay" wird einmal pro Bild für jedes "Collider" aufgerufen, das den Trigger berührt
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Island"))
        {
            Debug.Log("Trigger Stay");
            //other.gameObject.SetActive(false);
            MoveIsland(gameObject);
        }
    }

}
