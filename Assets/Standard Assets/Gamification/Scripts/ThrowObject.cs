using UnityEngine;
using System.Collections;

public class ThrowObject : MonoBehaviour
{
    public GameObject uiBox;
    private ConstantForce cf;
    public Transform player;
    public Transform playerCam;
    public float throwForce = 10;
    bool hasPlayer = false;
    bool beingCarried = false;
    public AudioClip[] soundToPlay;
    private AudioSource audio;
    public int dmg;
    private bool touched = false;
    public bool isBalloon = false;
    private bool firstcarried = false;
    private bool setUI = false;
    private bool trigger = false;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        cf = GetComponent<ConstantForce>();
    }


    void ShowUI()
    {
        if (hasPlayer && !setUI)
        {
            setUI = true;
            uiBox.SetActive(true);
        }
        else if (!hasPlayer && setUI)
        {
            setUI = false;
            uiBox.SetActive(false);
        }
    }


    void Update()
    {
        if (beingCarried)
            GamificationManager.instance.holdingItem = true;
        else
            GamificationManager.instance.holdingItem = false;

        if (hasPlayer)
            GamificationManager.instance.playerCanPickItem = true;
        else
            GamificationManager.instance.playerCanPickItem = false;

        ShowUI();
     


        if (beingCarried)
        {
            firstcarried = true;
            Debug.Log("touched");
        }
           

        if (!beingCarried && firstcarried && isBalloon)
        {
            cf.enabled = true;
            GetComponent<Rigidbody>().useGravity = false;
            Debug.Log("Flying");
        }

        float dist = Vector3.Distance(gameObject.transform.position, player.position);
        if (dist <= 2.5f)
        {
            hasPlayer = true;
        }
        else
        {
            hasPlayer = false;
        }
        if (hasPlayer && !beingCarried && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Carry");
            GetComponent<Rigidbody>().isKinematic = true;
            transform.parent = playerCam;
            beingCarried = true;
            trigger = true;
        }
        if (hasPlayer && beingCarried && Input.GetMouseButtonUp(0))
        {
            trigger = false;
        }
        if (beingCarried)
        {
            if (touched)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                transform.parent = null;
                beingCarried = false;
                touched = false;
            }
            if (Input.GetMouseButtonDown(0) && !trigger)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                transform.parent = null;
                beingCarried = false;
                GetComponent<Rigidbody>().AddForce(playerCam.forward * throwForce);
                RandomAudio();
            }
            else if (Input.GetMouseButtonDown(1) && !trigger)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                transform.parent = null;
                beingCarried = false;
            }
        }
    }
    void RandomAudio()
    {
        if (audio.isPlaying)
        {
            return;
        }
        audio.clip = soundToPlay[Random.Range(0, soundToPlay.Length)];
        audio.Play();

    }

  

    void OnTriggerEnter(Collider other)
    {
        if (beingCarried && !other.CompareTag("Door"))
        {
            Debug.Log("ouch");
            touched = true;
        }

      
    }
}