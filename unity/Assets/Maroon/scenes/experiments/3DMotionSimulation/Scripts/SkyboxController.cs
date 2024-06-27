using System.Collections.Generic;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    private static SkyboxController _instance;

    [SerializeField] private Material _grass;
    [SerializeField] private Material _space;
    [SerializeField] private Material _experimentRoom;

    /// <summary>
    /// Sets the background of the scene
    /// </summary>
    /// <param name="background">Background to use</param>
    public void SetBackground(string background)
    {
        switch (background)
        {
            case "ExperimentRoom":
                SetExperimentRoom();
                break;
            case "Grass":
                SetGrass();
                break;
            case "Space":
                SetSpace();
                break;
            default:
                SetExperimentRoom();
                break;
        }   
    }

    /// <summary>
    /// Sets the space background 
    /// </summary>
    public void SetSpace()
    {
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        RenderSettings.skybox = _space;
    }

    /// <summary>
    /// Sets the grass background 
    /// </summary>
    public void SetGrass()
    {
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        RenderSettings.skybox = _grass;
    }

    /// <summary>
    /// Sets the default experiment room as background
    /// </summary>
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
}
