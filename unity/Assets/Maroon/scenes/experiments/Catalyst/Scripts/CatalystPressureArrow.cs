using System;
using UnityEngine;

public class CatalystPressureArrow : MonoBehaviour
{
    [SerializeField] Transform lookAtTransform;

    private void Start()
    {
        transform.LookAt(lookAtTransform);
    }

    public void PressureChanged(float currentPressure, float maxPressure)
    {
        Vector3 scale = transform.localScale;
        Vector3 newScale = new Vector3(scale.x, scale.y, (currentPressure / maxPressure) + 1);
        transform.localScale = newScale;
    }
}