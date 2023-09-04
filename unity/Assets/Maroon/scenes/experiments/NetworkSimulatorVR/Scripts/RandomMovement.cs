using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    //public Rigidbody rb;

    //Move on Y axis
    private float frequency = 0.07f;

    //[SerializeField] private float speed = 0.1f;
    // Position Storage Variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        posOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * frequency) / 2;// * amplitude;

        transform.position = tempPos;


        Debug.Log("Status:\n" + tempPos.y);
    }

    void FixedUpdate()
    {
        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * frequency) / 2;// * amplitude;

        transform.position = tempPos;


        Debug.Log("Status2:\n" + tempPos.y);
    }





}