using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxController : MonoBehaviour, IResetObject
{
    private static SkyboxController _instance;

    [SerializeField] private Material _grass;
    [SerializeField] private Material _space;
    [SerializeField] private Material _experimentRoom;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBackground(string background)
    {
        if (background == "ExperimentRoom")
            SetExperimentRoom();
        else if (background == "Grass")
            SetGrass();
        else if (background == "Space")
            SetSpace();
        else
            SetExperimentRoom();
        
    }

    public void SetSpace()
    {
        MaxCameraPosition.Instance.ChangePosition();
        MinCameraPosition.Instance.ChangePosition();
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        RenderSettings.skybox = _space;
    }

    public void SetGrass()
    {
        MaxCameraPosition.Instance.ChangePosition();
        MinCameraPosition.Instance.ChangePosition();
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        RenderSettings.skybox = _grass;
    }

    public void SetExperimentRoom()
    {
        Camera.main.clearFlags = CameraClearFlags.Depth;
        RenderSettings.skybox = _experimentRoom;
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
        SetExperimentRoom();
    }
}
