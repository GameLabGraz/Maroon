using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpCharacterController : MonoBehaviour {


    private Transform player; //Player
    private Rigidbody rb;
    private Vector3 direction;
    public float speed;
    public float turnSpeed;



	// Use this for initialization
	void Start ()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "Laboratory")
            player = GameObject.FindGameObjectWithTag("Player").transform;
        else
            player = GameObject.FindGameObjectWithTag("MainCamera").transform;

        rb = GetComponent<Rigidbody>();
        speed = 100;
        turnSpeed = 5.0f;
        //Cursor.lockState = CursorLockMode.Locked;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Set direction of helpi
        direction = player.position - rb.position;
        direction.Normalize(); //for Lookrotation vector needs to be orthogonal
        //Slerp = Rotation from X to Y. X is current rotation, Y is where player direction vector is
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
       

    }




}
