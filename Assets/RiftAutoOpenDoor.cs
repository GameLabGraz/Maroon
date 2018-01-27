using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class RiftAutoOpenDoor : MonoBehaviour
{
    private Animator anim;
    public GameObject character;
    public float distanceToOpen = 5f;
    private int characterNearbyHash = Animator.StringToHash("character_nearby");
//    private VRTK_BodyPhysics _bodyPhysics;
//    private GameObject _bodyColliderContainer;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

//    private void Awake()
//    {
        // get Food/BodyColliderContainer
//        _bodyPhysics = GameObject.FindGameObjectWithTag("VRTK_PlayArea").GetComponent<VRTK_BodyPhysics>();
//        _bodyColliderContainer = _bodyPhysics.customBodyColliderContainer;
//    }

    void Update()
    {
        var playTransform = VRTK_DeviceFinder.PlayAreaTransform();
        if(playTransform == null)
            return;
        var characterPosition = playTransform.transform.position;
        var distance = Vector3.Distance(transform.position, characterPosition);

        if (distanceToOpen >= distance)
        {
            anim.SetBool(characterNearbyHash, true);
        }
        else
        {
            anim.SetBool(characterNearbyHash, false);
        }
    }

//    private void OnTriggerEnter(Collider other)
//    {
////        Debug.Log("Collided with Door: " + other.gameObject.name);
//        Debug.Log(other.gameObject.name);
//        Debug.Log(_bodyColliderContainer.name);
//        if (other.gameObject.name == _bodyColliderContainer.name)
//        {
//            Debug.Log("Yes");
//        }
//    }
}