using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Localization;


//This manager will be created one time at the beginning and stays troughout all scenes to manage 
//the game and save settings etc. Since the game doesn't have a load/save function, we don't need Prefabs

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager Instance = null; 

    [SerializeField]
    private string _mlgFile;

    private static GameObject _player;
    private static Vector3 _playerPosition;
    private static Quaternion _playerRotation;

    private LanguageManager _languageManager;

    private string _startScene;
    public string StartScene { get{ return _startScene; } }

    private string _scene; //name of scene player is currently

    public AudioSource menuSound; //sound when player goes to menu

    private AsyncOperation _async;

    public bool LabLoaded { get; private set; }

    //Manager will be created only once and then stays for all scenes
    private void Awake()
    {
        DontDestroyOnLoad(this);

        var language = Application.systemLanguage.ToString();
        _player = GameObject.FindGameObjectWithTag("Player");
        _startScene = SceneManager.GetActiveScene().name;

        if (Instance == null)
        {
            Instance = this;
        }            
        else if (Instance != this)
        {
            language = LanguageManager.Instance.CurrentLanguage;

            if (_player != null)
            {
                _player.transform.position = _playerPosition;
                _player.transform.rotation = _playerRotation;
            }

            Instance.LabLoaded = true;
            Destroy(gameObject);
        }

        _languageManager = LanguageManager.Load(_mlgFile);
        _languageManager.CurrentLanguage = language;
    }

    public void PlayMenuSound()
    {
        menuSound.volume = SoundManager.Instance.EfxVolume;
        menuSound.Play();
    }

    //Load Laboratory Scene in Background to avoid lag
    //Use StartLoading to start loading the scene in background. Called in FadeOnEnter.cs and LoadOnEnter.cs
    public void StartLoading()
    {
        StartCoroutine("Load");
    }

    private IEnumerator Load()
    {
        Debug.LogWarning("ASYNC LOAD STARTED - " +
           "DO NOT EXIT PLAY MODE UNTIL SCENE LOADS... UNITY WILL CRASH. SMALL LAG JUST IN EDITOR, WORKS SMOOTH IN BUILD");
        _async = SceneManager.LoadSceneAsync(_scene);
        _async.allowSceneActivation = false;
        yield return _async;
    }

    //Use Activate to switch to the loaded scene immediately
    public void ActivateScene()
    {
        _async.allowSceneActivation = true;
    }

    private void Update ()
    {
        if (_player != null)
        {
            _playerPosition = _player.transform.position;
            _playerRotation = _player.transform.rotation;
        }            

        var activeScene = SceneManager.GetActiveScene().name;
        if (Input.GetKeyDown(KeyCode.Escape) && activeScene != "Menu")
        {
            _scene = activeScene;
            PlayMenuSound();
            SceneManager.LoadScene("Menu");
        }
    }

    public void LoadSceneAfterMenu()
    {
        SceneManager.LoadScene(_scene);
    }

}

