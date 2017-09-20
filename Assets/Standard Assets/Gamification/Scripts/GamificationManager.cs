using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 


//This manager will be created one time at the beginning and stays troughout all scenes to manage 
//the game and save settings etc. Since the game doesnt have a load/save function, we dont need Prefabs

public class GamificationManager : MonoBehaviour
{

    [HideInInspector]
    public float player_position_x;
    [HideInInspector]
    public float player_position_y;
    [HideInInspector]
    public float player_position_z;
    [HideInInspector]
    public Quaternion clockSmallRotation;
    [HideInInspector]
    public Quaternion clockLongRotation;
    [HideInInspector]
    public static GamificationManager instance = null; //so we have access to Manager from other files
    public LanguageManager l_manager;
    private string scene;
    public AudioSource menuSound;

    //Gamification Bools
    public bool gameStarted = false;
    [HideInInspector]
    public bool headset = false;
    [HideInInspector]
    public bool coroutineRunning = false;
    [HideInInspector]
    public bool xmlLoaded = false;
    [HideInInspector]
    public bool deactivateDialogue = false;
    [HideInInspector]



    //Manager will be created only once and then stays for all scenes
    public void Awake()
    {

        //  Debug.Log(l_manager.GetString("he", Language.English));
        scene = "Laboratory";
        DontDestroyOnLoad(this);
        //this prevents object being duplicated when player goes back to laboratory
        //simple solution instead of singleton pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);


    }

    public void PlayMenuSound()
    {
        menuSound.volume = SoundManager.instance.efxSource.volume;
        menuSound.Play();
    }

    public void Start()
    {
        l_manager = LanguageManager.Load();
        //Set language based on user system language
        if (Application.systemLanguage.ToString() == "German")
            l_manager.SetCurrentLanguage(Language.German);
        else if (Application.systemLanguage.ToString() == "French")
            l_manager.SetCurrentLanguage(Language.French);
        else
            l_manager.SetCurrentLanguage(Language.English);



    }

    public void Resume()
    {
        Debug.Log(scene);
            SceneManager.LoadScene(scene);
            if (scene == "Laboratory")
                Cursor.lockState = CursorLockMode.Locked;       
    }

    // Locking and unlocking mouse and loading menu and going back from menu
    void Update ()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
    
        if (Input.GetKeyDown("escape") && sceneName != "Menu")
        {
            if (sceneName != "Menu")
                scene = sceneName;
            PlayMenuSound();
            SceneManager.LoadScene("Menu");
        }
        //Locking and unlocking mouse
        if (sceneName == "Laboratory")
        {
            if (Cursor.lockState == CursorLockMode.None)
                Cursor.lockState = CursorLockMode.Locked;
          
             
        }      
      
        else
            Cursor.lockState = CursorLockMode.None;
     

    }

   public void LoadSceneAfterMenu()
    {
        SceneManager.LoadScene(scene);
    }

}

