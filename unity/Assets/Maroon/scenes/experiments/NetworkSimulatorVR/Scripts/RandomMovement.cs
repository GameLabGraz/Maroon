using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public Rigidbody rb;

    //[SerializeField] private float speed = 0.1f;
    private float speed = 0.001f;
    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(speed, speed, speed, ForceMode.Impulse);


    }

    // Update is called once per frame
    void Update()
    {
        timer = timer + Time.deltaTime;
        Debug.Log("timer: " + timer);
        if( (timer > 3) && (timer <= 6) )
        {
            //rb.transform.position = new Vector3(transform.position.x - speed, transform.position.y - speed, transform.position.z - speed);
            //timer = 0.0f;
           
            rb.AddForce(-speed, -speed, -speed, ForceMode.Impulse);
        }
        else
        {
            //rb.transform.position = new Vector3(transform.position.x + speed, transform.position.y + speed, transform.position.z + speed);
            rb.AddForce(speed, speed, speed, ForceMode.Impulse);
            
        }

        if( timer > 6)
        {
            timer = 0.0f;
        }

    }

}