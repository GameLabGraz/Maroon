using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    [SerializeField] private Material soccerField;
    [SerializeField] private Material space;



    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.skybox = space;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
