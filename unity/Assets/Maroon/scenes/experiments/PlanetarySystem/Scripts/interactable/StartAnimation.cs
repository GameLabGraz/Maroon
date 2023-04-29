using UnityEngine;

public class StartAnimation : MonoBehaviour
{
    public Material stars_skybox;           //on

    public GameObject sun;                  //on
    public GameObject Environment;          //off
    public GameObject MainCamera;           //off
    public GameObject SolarSystemCamera;    //on
    public GameObject Planets;              //off/on
    public GameObject SortingMinigame;      //off
    public GameObject Interactibles;        //off
    public GameObject Userinterface;        //on

    void OnMouseDown()
    {
        Debug.Log("StartAnimationScreen OnMouseDown() pressed!");

        RenderSettings.skybox = stars_skybox;

        Environment.SetActive(false);
        MainCamera.SetActive(false);
        SolarSystemCamera.SetActive(true);
        Planets.SetActive(true);
        SortingMinigame.SetActive(false);
        Interactibles.SetActive(false);
        Userinterface.SetActive(true);
    }

    private void Awake()
    {
        //Debug.Log("Start Animation Awake()");
        sun.SetActive(true);
        
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Start Animation Start()");
        Planets.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            RenderSettings.skybox = stars_skybox;

            Environment.SetActive(false);
            MainCamera.SetActive(false);
            SolarSystemCamera.SetActive(true);
            Planets.SetActive(true);
            SortingMinigame.SetActive(false);
            Interactibles.SetActive(false);
            Userinterface.SetActive(true);
        }
        
    }
}
