using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxController : MonoBehaviour, IResetObject
{
    private static SkyboxController _instance;

    [SerializeField] private Material grass;
    [SerializeField] private Material space;
    [SerializeField] private Material experiment_room;



    // Start is called before the first frame update
    void Start()
    {
        /*
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        RenderSettings.skybox = space;*/
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void setBackground(string background)
    {
        Debug.Log("setting background to: " + background);

        if (background == "ExperimentRoom")
            setExperimentRoom();
        else if (background == "Grass")
            setGrass();
        else if (background == "Space")
            setSpace();
        else
            setExperimentRoom();
        
    }

    public void setSpace()
    {
        Debug.Log("setSpace()");
        MaxCameraPosition.Instance.changePosition();
        MinCameraPosition.Instance.changePosition();
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        RenderSettings.skybox = space;
    }

    public void setGrass()
    {
        Debug.Log("setGrass()");
        MaxCameraPosition.Instance.changePosition();
        MinCameraPosition.Instance.changePosition();
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        RenderSettings.skybox = grass;
    }

    public void setExperimentRoom()
    {
        Debug.Log("setExperimentRoom()");
        Camera.main.clearFlags = CameraClearFlags.Depth;
        RenderSettings.skybox = experiment_room;
    }

    public static SkyboxController Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SkyboxController>();
            return _instance;
        }
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public void ResetObject()
    {
        setExperimentRoom();
    }
}
