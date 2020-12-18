using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;

    [SerializeField]
    private AudioSource _efxSource; //for sounds

    [SerializeField]
    private AudioSource _musicSource; //for music

    public float MusicVolume
    {
        get { return _musicSource ? _musicSource.volume : 0f; }
        set { _musicSource.volume = value; }
    }

    public float EfxVolume
    {
        get { return _efxSource ? _efxSource.volume : 0f; }
        set { _efxSource.volume = value; }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        _musicSource = GetComponent<AudioSource>();
    }

    public void PlaySingle(AudioClip clip)
    {
        _efxSource.clip = clip;
        _efxSource.Play();
    }
}
