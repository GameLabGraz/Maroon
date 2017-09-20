using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public AudioSource efxSource; //for sounds
    public AudioSource musicSource; //for music
    public static SoundManager instance = null; //Damit man von anderen Scripts auf dieses zugreifen kann. Funktioniert nur, wenn es wirklich nur 1 Instanz dieses Objektes gibt

    public float lowPitchRange = .95f; //-5% von Original Pitch
    public float highPitchRange = 1.05f; //+5% von Original Pitch

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        musicSource = GetComponent<AudioSource>();
    }

    public void SetMusicVolume(float v)
    {
        musicSource.volume = v;
    }

    public void SetEfxVolume(float v)
    {
        efxSource.volume = v;
    }

    //Sound wird gespielt
    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }







}
