using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightAdapter : MonoBehaviour
{
    [SerializeField] protected Transform reference;

    private float _yOffsetInWorldPos = 0f;

    void Awake()
    {
        _yOffsetInWorldPos = transform.position.y - reference.position.y;
    }

    public void AdaptYOffset()
    {
        var pos = transform.position;
        pos.y = reference.position.y + _yOffsetInWorldPos;
        transform.position = pos;
    }
    
}
