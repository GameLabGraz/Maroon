using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 


//This manager will be created one time at the beginning and stays troughout all scenes to manage 
//the game and save settings etc. Since the game doesnt have a load/save function, we dont need Prefabs

public class GamificationManager : MonoBehaviour
{
    [HideInInspector]
    public bool playerIsMoving = false;
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
    public AudioClip AchievementSound;
    private GameObject player;

    //Gamification Bools
    [HideInInspector]
    public bool gameStarted = false; //is set true after game mechanics started
    [HideInInspector]
    public bool headset = false;
    [HideInInspector]
    public bool coroutineRunning = false;
    [HideInInspector]
    public bool xmlLoaded = false;
    [HideInInspector]
    public bool deactivateDialogue = false; //no dialogues if true
    [HideInInspector]
    public bool doorDialogue = false;  //this is the dialogue which is played once when the door is first opened
    [HideInInspector]
    public bool holdingItem = false; //if player is holding an item, menu call is not allowed
    [HideInInspector]
    public bool playerCanPickItem = false; //true if player is in range to pick up an item
    [HideInInspector]
    public bool OneBalloonSpawned = false; //player can hold only one balloon at the same time
    [HideInInspector]
    public bool doorIsOpen = false;
    [HideInInspector]
    public bool hasPlayer = false;

    //experiments and experiment built bools
 
    public GameObject[] pickups; //All pickups
    public GameObject graaf1Experiment;
    public GameObject graaf2Experiment;
    public GameObject fallingExperiment;
    public GameObject faradayExperiment;
    public GameObject pendulumExperiment;
    [HideInInspector]
    public bool vandegraaf2generatorEnabled = false;
    [HideInInspector]
    public bool vandegraaf2ballonEnabled = false;
    [HideInInspector]
    public bool vandegraaf2grounderEnabled = false;
    [HideInInspector]
    public bool vandegraaf2electrodeEnabled = false;
    [HideInInspector]
    public bool vandegraaf1generatorEnabled = false;
    [HideInInspector]
    public bool vandegraaf1grounderEnabled = false;
    [HideInInspector]
    public bool vandegraaf1electrodeEnabled = false;
    [HideInInspector]
    public bool fallingcoilmagnetEnabled = false;
    [HideInInspector]
    public bool fallingcoilringEnabled = false;
    [HideInInspector]
    public bool faradayslawmagnetEnabled = false;
    [HideInInspector]
    public bool faradayslawringEnabled = false;
    [HideInInspector]
    public bool pendulumweightEnabled = false;

    //Variables for loading laboratory in background from other scene 
    public string levelName;
    AsyncOperation async;
    //Prefabs and Variables to display the UI-Achievement-Messages and manage Achievements
    public GameObject parent;



    public Object achievementPrefab;
    private List<GameObject> spawnedAchievementUIs = new List<GameObject>();
    public int howMuchSpawnedAchievementUIs;
    [HideInInspector]
    private const int numberOfAchievements = 8;
    public int finishedAchievements = 0;
    [HideInInspector]
    public bool spokenWithHelpi = false;
    [HideInInspector]
    public bool spokenWithLaunch = false;
    [HideInInspector]
    public bool spokenWithDoor = false;
    [HideInInspector]



    /*List of dialogueKeys and IDs:
     * Achievement 1 : Helpi
     * Achievement 2 : Door
     * Achievement 3 : Build 
     * Achievement 4: Build Vandegraaf1
     * Achievement 5: Build Vandegraaf2
     * Achievement 6: Build Faradayslaw
     * Achievement 7: Build FallingCoil
     * Achievement 8: Build Pendulum
     */

    //this bools are true if the experiment was built correctly and is ready to use
    public bool vandegraaf1Complete = false;
    [HideInInspector]
    public bool vandegraaf2Complete = false;
    [HideInInspector]
    public bool pendulumComplete = false;
    [HideInInspector]
    public bool faradayslawComplete = false;
    [HideInInspector]
    public bool fallingcoilComplete = false;
    [HideInInspector]

    public int getNumberOfAchievements()
    {
        return numberOfAchievements;
    }

    //Manager will be created only once and then stays for all scenes
    public void Awake()
    {
        levelName = "Laboratory";
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

        //Add first achievement
        AddAchievement("Achievement 1", "Helpi");

        //Add pickups
        pickups = GameObject.FindGameObjectsWithTag("pickup");




    }


    //Load Laboratory Scene in Background to avoid lag
    //Use StartLoading to start loading the scene in background. Called in FadeOnEnter.cs and LoadOnEnter.cs
    public void StartLoading()
    {
        StartCoroutine("load");
    }

    IEnumerator load()
    {
        Debug.LogWarning("ASYNC LOAD STARTED - " +
           "DO NOT EXIT PLAY MODE UNTIL SCENE LOADS... UNITY WILL CRASH. SMALL LAG JUST IN EDITOR, WORKS SMOOTH IN BUILD");
        async = SceneManager.LoadSceneAsync(scene);
        async.allowSceneActivation = false;
        yield return async;
    }

    //Use Activate to switch to the loaded scene immediately
    public void ActivateScene()
    {
        async.allowSceneActivation = true;
       

    }

    //Resume Game from Menu
    public void Resume()
    {
       

    }


    public bool isAchievementInList(string id)
    {
        foreach (var it in spawnedAchievementUIs)
        {
            if (it.GetComponent<AchievementController>().getID() == id)
            {
                return true;
            }
        }
        return false;
    }

    //Delete the achievement with the ID when the goal is accomplished
    //Moving other achievements correctly
    public void DeleteAchievement(string id)
    {
        SoundManager.instance.PlaySingle(AchievementSound);
        int number = 0;
        foreach (var it in spawnedAchievementUIs)
        {
            if (it.GetComponent<AchievementController>().getID() == id)
            {
                number = it.GetComponent<AchievementController>().number;
            }
        }
        Destroy(spawnedAchievementUIs[number]);
        //Change position of other achievements
        for (int i = number+1; i < spawnedAchievementUIs.Count; i++)
        {
            AchievementController script = spawnedAchievementUIs[i].GetComponent<AchievementController>();
            script.ChangePosition();
         
        }
        //Nach Hinten rücken
        spawnedAchievementUIs.RemoveAt(number);
        howMuchSpawnedAchievementUIs--;
    }

    //Show Achievements when player is in laboratory
    void ShowAchievements()
    {

        foreach (var it in spawnedAchievementUIs)
        {
            it.SetActive(true);
        }
    }

    //Hide Achievements when player is not in laboratory
    void HideAchievements()
    {
        foreach (var it in spawnedAchievementUIs)
        {
            it.SetActive(false);
        }
    }

    //Add a new achievement to list and instantiate the UI-element
    //dialogueKey : Key of achievement-string
    //ID : Unique ID of achievement - important for deleting 
    public void AddAchievement(string dialogeKey, string ID)
    {
        GameObject achievement = Instantiate(achievementPrefab, new Vector3(parent.transform.position.x, parent.transform.position.y, parent.transform.position.z),
            Quaternion.identity, parent.transform) as GameObject;
        achievement.GetComponent<AchievementController>().SetText(l_manager.GetString(dialogeKey), ID); //Access script function SetText
        spawnedAchievementUIs.Add(achievement);

    }

    // Locking and unlocking mouse and loading menu and going back from menu
    void Update ()
    {
        //Add pickups
        pickups = GameObject.FindGameObjectsWithTag("pickup");
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (sceneName == "Laboratory")
            ShowAchievements();
        else
            HideAchievements();
    
        if (Input.GetKeyDown("escape") && sceneName != "Menu" && !holdingItem)
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

        if (holdingItem)
            Cursor.lockState = CursorLockMode.Locked;     

    }

   public void LoadSceneAfterMenu()
    {
        SceneManager.LoadScene(scene);
    }

}

