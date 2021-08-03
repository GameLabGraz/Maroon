using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinCameraPosition : MonoBehaviour, IResetObject
{
    private static MinCameraPosition _instance;
    private Vector3 start_position_;

    // Start is called before the first frame update
    void Start()
    {
        start_position_ = transform.position;
    }

    public void changePosition()
    {
        transform.position = new Vector3(-8.6f, -6.4f, -5.9f);
    }

    public static MinCameraPosition Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<MinCameraPosition>();
            return _instance;
        }
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public void ResetObject()
    {
        transform.position = start_position_;
    }
}
