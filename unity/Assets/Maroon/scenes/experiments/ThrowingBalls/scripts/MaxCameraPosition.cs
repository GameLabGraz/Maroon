using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxCameraPosition : MonoBehaviour, IResetObject
{
    private static MaxCameraPosition _instance;
    private Vector3 start_position_;

    // Start is called before the first frame update
    void Start()
    {
        start_position_ = transform.position;
    }

    public void changePosition()
    {
        transform.position = new Vector3(9.7f, 7.1f, 12.7f);
    }

    public static MaxCameraPosition Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<MaxCameraPosition>();
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
