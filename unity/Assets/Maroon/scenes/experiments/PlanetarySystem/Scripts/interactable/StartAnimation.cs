using UnityEngine;

public class StartAnimation : MonoBehaviour
{                                           //want it:
    public Material stars_skybox;           //on

    public GameObject sun;                  //on
    public GameObject Environment;          //off
    public GameObject MainCamera;           //off
    public GameObject SolarSystemCamera;    //on
    public GameObject Planets;              //off/on
    public GameObject SortingMinigame;      //off
    public GameObject Interactibles;        //off
    public GameObject Userinterface;        //on
    public GameObject HideUI;               //on
    public GameObject PlanetInfoUI;         //off
    public GameObject FormulaUI;            //on

    public GameObject GameController;

    private FlyCamera flyCameraScript;      //on


    private void Awake()
    {
        //Debug.Log("Start Animation Awake()");
        sun.SetActive(true);
    }


    void Start()
    {
        //Debug.Log("Start Animation Start()");
        Planets.SetActive(false);

        flyCameraScript = GameController.GetComponent<FlyCamera>();
        if (flyCameraScript == null)
        {
            Debug.LogError("Script FlyCamera not found");
        }
    }


    void OnMouseDown()
    {
        StartAnimationOnInput();
    }


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            StartAnimationOnInput();
        }
        
    }


    /*
     * 
     */
    void StartAnimationOnInput()
    {
        //Debug.Log("StartAnimationOnInput OnMouseDown() pressed!");
        RenderSettings.skybox = stars_skybox;

        Environment.SetActive(false);
        MainCamera.SetActive(false);
        SolarSystemCamera.SetActive(true);
        Planets.SetActive(true);
        SortingMinigame.SetActive(false);
        Interactibles.SetActive(false);
        Userinterface.SetActive(true);
        HideUI.SetActive(true);
        PlanetInfoUI.SetActive(false);
        FormulaUI.SetActive(false);
        flyCameraScript.enabled = true;
    }
}