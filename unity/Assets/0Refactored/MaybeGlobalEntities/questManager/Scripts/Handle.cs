using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : MonoBehaviour
{
    [SerializeField] private GameObject dataObjectRoot;
    [SerializeField] private float movementMultiplier = 1.0f;

    private Vector3 iniPosition;
    private Vector3 iniRootPosition;

    private void Start()
    {
        iniPosition = transform.position;
        iniRootPosition = dataObjectRoot.transform.position;
    }

    private void Update()
    {
        dataObjectRoot.transform.position = iniRootPosition - (transform.position - iniPosition) * movementMultiplier;
    }
}
