using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMiddleRings : MonoBehaviour
{
    // Objects to check status
    public DragObject source;
    public DragObject destination;
    public DragObject gateway;

    // Objects to move
    public GameObject ring_to_move_up;
    public GameObject pos_to_move_up;


    public GameObject ring_to_move_down;
    public GameObject pos_to_move_down;

    public float speed = 0.01f;


    Vector3 r1_position;
    Vector3 r1_end_position;

    Vector3 r2_position;
    Vector3 r2_end_position;

    // Start is called before the first frame update
    void Start()
    {
        r1_position = ring_to_move_up.transform.position;
        r1_end_position = pos_to_move_up.transform.position;

        r2_position = ring_to_move_down.transform.position;
        r2_end_position = pos_to_move_down.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if ( (source.source_snapped == true) &&
             (destination.destination_snapped == true) &&
             (gateway.gateway_snapped == true))
        {
            openMiddle();
        }
    }


    private void openMiddle()
    {//transform.position = Vector3.Lerp(transform.position, startPos, lerpSpeed * Time.deltaTime);

        ring_to_move_up.transform.position = Vector3.Lerp(r1_position, r1_end_position, speed * Time.deltaTime);
        ring_to_move_down.transform.position = Vector3.Lerp(r2_position, r2_end_position, speed * Time.deltaTime);
    }


}
