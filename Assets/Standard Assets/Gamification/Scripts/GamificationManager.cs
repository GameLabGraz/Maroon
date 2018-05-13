using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;



//This manager will be created one time at the beginning and stays troughout all scenes to manage 
//the game and save settings etc. Since the game doesnt have a load/save function, we dont need Prefabs

public class GamificationManager : MonoBehaviour
{
    //create an instance so we have access to Manager from other files via GamificationManager.instance
    [HideInInspector]
    public static GamificationManager instance = null; 
    //Player position needs to be saved if player changes the scene and goes back
    private float player_position_x;
    private float player_position_y;
    private float player_position_z;
    public Vector3 Player_position
    {
        get { return new Vector3(player_position_x, player_position_y, player_position_z); }
        set { player_position_x = value.x; player_position_y = value.y; player_position_z = value.z; }
    }
    //Same for clock position
    private Quaternion clockSmallRotation;
    public Quaternion ClockSmallRotation
    {
        get { return clockSmallRotation; }
        set { clockSmallRotation = value; }
    }
    private Quaternion clockLongRotation;
    public Quaternion ClockLongRotation
    {
        get { return clockSmallRotation; }
        set { clockSmallRotation = value; }
    }
    //some important things
    public LanguageManager l_manager;
    private string scene; //name of scene player is currently
    public AudioSource menuSound; //sound when player goes to menu
    public AudioClip AchievementSound; //sound when player gets an achievement
    private GameObject player; //player
    //Gamification Bools
    private bool playerIsMoving = false;
    public bool PlayerIsMoving
    {
        set { playerIsMoving = value; }
    }
    private bool labLoaded = false;
    public bool LabLoaded
    {
        get { return labLoaded; }
        set { labLoaded = value; }
    }
    private bool gameStarted = false; //is set true after game mechanics started
    public bool GameStarted
    {
        get { return gameStarted; }
        set { gameStarted = value; }
    }
    private bool headset = false;
    public bool Headset
    {
        get { return headset; }
        set { headset = value; }
    }
    private bool coroutineRunning = false;
    public bool CoroutineRunning
    {
        get { return coroutineRunning; }
        set { coroutineRunning = value; }
    }
    private bool xmlLoaded = false;
    public bool XmlLoaded
    {
        get { return xmlLoaded; }
        set { xmlLoaded = value; }
    }
    private bool deactivateDialogue = false; //no dialogues if true
    public bool DeactivateDialogue
    {
        get { return deactivateDialogue; }
        set { deactivateDialogue = value; }
    }
    private bool doorDialogue = false;  //this is the dialogue which is played once when the door is first opened
    public bool DoorDialogue
    {
        get { return doorDialogue; }
        set { doorDialogue = value; }
    }
    private bool holdingItem = false; //if player is holding an item, menu call is not allowed
    public bool HoldingItem
    {
        get { return holdingItem; }
        set { holdingItem = value; }
    }
    private bool playerCanPickItem = false; //true if player is in range to pick up an item
    public bool PlayerCanPickItem
    {
        get { return playerCanPickItem; }
        set { playerCanPickItem = value; }
    }
    private bool oneBalloonSpawned = false; //player can hold only one balloon at the same time
    public bool OneBalloonSpawned
    {
        get { return oneBalloonSpawned; }
        set { oneBalloonSpawned = value; }
    }
    private bool doorIsOpen = false;
    public bool DoorIsOpen
    {
        get { return doorIsOpen; }
        set { doorIsOpen = value; }
    }
    private bool hasPlayer = false;
    public bool HasPlayer
    {
        get { return hasPlayer; }
        set { hasPlayer = value; }
    }

   
 
    //pickup variables. work automatically if you tag them correctly with "pickup" tag
    public GameObject[] pickups; //All pickups, array will be created automatically and contains all items with tag "pickup"
    private List<Vector3> pickupPositions = new List<Vector3>(); //Positions are saved for each pickup for scene change
    public List<System.String> namesOfDestroyedPickups = new List<string>(); //Destroyed pickups are saved for scene change

    //experiments and experiment built bools and getter/setters. For each item you need to place there is a bool
    private bool vandegraaf2generatorEnabled = false;
    public bool Vandegraaf2generatorEnabled
    {
        get { return vandegraaf2generatorEnabled; }
        set { vandegraaf2generatorEnabled = value; }
    }
    private bool vandegraaf2ballonEnabled = false;
    public bool Vandegraaf2ballonEnabled
    {
        get { return vandegraaf2ballonEnabled; }
        set { vandegraaf2ballonEnabled = value ; }
    }
    private bool vandegraaf2grounderEnabled = false;
    public bool Vandegraaf2grounderEnabled
    {
        get { return vandegraaf2grounderEnabled; }
        set { vandegraaf2grounderEnabled = value  ; }
    }
    private bool vandegraaf2electrodeEnabled = false;
    public bool Vandegraaf2electrodeEnabled
    {
        get { return vandegraaf2electrodeEnabled; }
        set { vandegraaf2electrodeEnabled = value; }
    }
    private bool vandegraaf1generatorEnabled = false;
    public bool Vandegraaf1generatorEnabled
    {
        get { return vandegraaf1generatorEnabled; }
        set { vandegraaf1generatorEnabled = value; }
    }
    private bool vandegraaf1grounderEnabled = false;
    public bool Vandegraaf1grounderEnabled
    {
        get { return vandegraaf1generatorEnabled; }
        set { vandegraaf1generatorEnabled = value; }
    }
    private bool vandegraaf1electrodeEnabled = false;
    public bool Vandegraaf1electrodeEnabled
    {
        get { return vandegraaf1electrodeEnabled; }
        set { vandegraaf1electrodeEnabled = value; }
    }
    private bool fallingcoilmagnetEnabled = false;
    public bool FallingcoilmagnetEnabled
    {
        get { return fallingcoilmagnetEnabled; }
        set { fallingcoilmagnetEnabled = value; }
    }
    private bool fallingcoilringEnabled = false;
    public bool FallingcoilringEnabled
    {
        get { return fallingcoilringEnabled; }
        set { fallingcoilringEnabled = value; }
    }
    private bool faradayslawmagnetEnabled = false;
    public bool FaradayslawmagnetEnabled
    {
        get { return faradayslawmagnetEnabled; }
        set { faradayslawmagnetEnabled = value; }
    }
    private bool faradayslawringEnabled = false;
    public bool FaradayslawringEnabled
    {
        get { return faradayslawringEnabled; }
        set { faradayslawringEnabled = value; }
    }
    private bool pendulumweightEnabled = false;
    public bool PendulumweightEnabled
    {
        get { return pendulumweightEnabled; }
        set { pendulumweightEnabled = value; }
    }

    //Variables for loading laboratory in background from other scene 
    public string levelName;
    AsyncOperation async;
    //Prefabs and Variables to display the UI-Achievement-Messages and manage Achievements
    public GameObject parent;



    public Object achievementPrefab;
    private List<GameObject> spawnedAchievementUIs = new List<GameObject>();
    private int howMuchSpawnedAchievementUIs;
    public int HowMuchSpawnedAchievementUIs
    {
        get { return howMuchSpawnedAchievementUIs; }
        set { howMuchSpawnedAchievementUIs = value; }
    }
    private const int numberOfAchievements = 8;
    private int finishedAchievements = 0;
    public int FinishedAchievements
    {
        get { return finishedAchievements; }
        set { finishedAchievements = value; }
    }
    private bool spokenWithHelpi = false;
    public bool SpokenWithHelpi
    {
        get { return spokenWithHelpi; }
        set { spokenWithHelpi = value; }
    }
    private bool spokenWithLaunch = false;
    public bool SpokenWithLaunch
    {
        get { return spokenWithLaunch; }
        set { spokenWithLaunch = value; }
    }
    private bool spokenWithDoor = false;
    public bool SpokenWithDoor
    {
        get { return spokenWithDoor; }
        set { spokenWithDoor = value; }
    }

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

        //Add pickups and positions
        pickups = GameObject.FindGameObjectsWithTag("pickup").OrderBy(go => go.name).ToArray();
        foreach (var pickup in pickups)
        {
            pickupPositions.Add(pickup.transform.position);
        }




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
        if (!labLoaded)
        {
            Debug.Log("return");
            return;

        }
        //Add pickups
        pickups = GameObject.FindGameObjectsWithTag("pickup").OrderBy(go => go.name).ToArray();
        //Replace all the pickups where they were left when player went out of laboratory
        for (int i = 0; i < pickups.Length; i++)
        {
            pickups[i].transform.position = pickupPositions[i];
        }
        //Set already placed objects to false
        if(namesOfDestroyedPickups.Count>0)
        {
            for (int i = 0; i < namesOfDestroyedPickups.Count; i++)
                GameObject.Find(namesOfDestroyedPickups[i]).SetActive(false);
        }

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
        finishedAchievements++;
        int number = -1;
        foreach (var it in spawnedAchievementUIs)
        {
            if (it.GetComponent<AchievementController>().getID() == id)
            {
                number = it.GetComponent<AchievementController>().number;
            }
        }
        if (number == -1)
            return;
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
        achievement.GetComponent<AchievementController>().setDialogue(dialogeKey, ID); //Access script function SetText
        spawnedAchievementUIs.Add(achievement);

    }

    //Update position of all Pickups
    void Pickups()
    {
        //pickupPositions[0] = pickups[0].transform.position;
       // pickupPositions[1] = pickups[1].transform.position;


        for (int i = 0; i < pickups.Length; i++)
         {
             pickupPositions[i] = pickups[i].transform.position;
         }
    }

    // Locking and unlocking mouse and loading menu and going back from menu
    void Update ()
    {
        labLoaded = true;
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (sceneName == "Laboratory")
        {
            ShowAchievements();
            Pickups(); //Because pickups are only in lab
        }        
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

