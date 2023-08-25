using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public Rigidbody rb;

    [SerializeField] private float speed = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1), ForceMode.Impulse);
 
    }

    // Update is called once per frame
    void Update()
    {

        rb.AddForce(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1), ForceMode.Impulse);

    }

}