using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FollowPlayer : MonoBehaviour
{
    public Transform target;
    public UnityEvent follow;

    // Update is called once per frame
    void Update()
    {

        follow.Invoke();
        //transform.LookAt(target);
        //transform.InverseTransformDirection(Vector3.left);
    }
}
