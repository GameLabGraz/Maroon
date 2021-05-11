using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.UI;
using TMPro;

public class ValueGraph : MonoBehaviour
{

    private static ValueGraph _instance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadChoosenValue(int choice)
    {
        switch (choice)
        {
            case 0:
                
                break;
            case 1:
                
                break;
            case 2:
                
                break;
            case 3:
                
                break;
            default:
                break;
        }
    }

    public static ValueGraph Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ValueGraph>();
            return _instance;
        }
    }
}
