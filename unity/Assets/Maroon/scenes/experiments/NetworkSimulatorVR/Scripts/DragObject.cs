using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DragObject : MonoBehaviour
{
    public TextMeshPro Text;
    public bool goingToStartPosition = false;
    public DragSlot slot;

    public float lerpSpeed = 2f;

    Vector3 startPos;
    bool waitOneFrame = false;

    private void Start()
    {
        
        startPos = transform.position;
    }

    private void Update()
    {
        if (goingToStartPosition)
        {
            if (waitOneFrame)
            {
                waitOneFrame = false;
            } else
            {
                transform.position = Vector3.Lerp(transform.position, startPos, lerpSpeed * Time.deltaTime);
            }
        }
    }


    public void SendToStartPosition()
    {
        waitOneFrame = true;
        goingToStartPosition = true;
    }
}
