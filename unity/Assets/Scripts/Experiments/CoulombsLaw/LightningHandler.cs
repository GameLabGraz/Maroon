using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningHandler : MonoBehaviour
{
    public List<GameObject> lightnings;
    [Range(0f, 5f)]
    public float lightningDuration;

    private float _currentTime = -1f;

    private void Start()
    {
        foreach(var obj in lightnings)
            obj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _currentTime -= Time.deltaTime;

        if (_currentTime < 0f)
        {
            foreach(var obj in lightnings)
                obj.SetActive(false);
        }
    }


    public void ActivateLightning()
    {
        _currentTime = lightningDuration;

        foreach (var obj in lightnings)
        {
            obj.SetActive(true);
        }
    }
}