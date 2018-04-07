using UnityEngine;
using System.Collections;

public class ThrowObject : MonoBehaviour
{
    private GameObject uiBox;
    private ConstantForce cf;
    private GameObject player;
    private GameObject playerCam;
    private float throwForce = 200;
    bool hasPlayer = false;
    bool beingCarried = false;
    public int dmg;
    private bool touched = false;
    public bool isBalloon = false;
    private bool firstcarried = false;
    private bool setUI = false;
    private bool trigger = false;
    private DialogueManager dMan;
    public string ID;


    private void Awake()
    {
        uiBox = GameObject.FindWithTag("UI");
        playerCam  = GameObject.FindWithTag("MainCamera");
        player = GameObject.FindWithTag("Player");
        dMan = FindObjectOfType<DialogueManager>();
    }

    void Start()
    {
        cf = GetComponent<ConstantForce>();
       // uiBox = GameObject.FindGameObjectWithTag("UI");
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
        }
           

        if (!beingCarried && firstcarried && isBalloon)
        {
            cf.enabled = true;
            GetComponent<Rigidbody>().useGravity = false;
            Debug.Log("Flying");
        }

        float dist = Vector3.Distance(gameObject.transform.position, player.transform.position);
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
            transform.parent = playerCam.transform;
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
                GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * throwForce);
                GamificationManager.instance.OneBalloonSpawned = false;
            }
            else if (Input.GetMouseButtonDown(1) && !trigger)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                transform.parent = null;
                beingCarried = false;
                GamificationManager.instance.OneBalloonSpawned = false;
            }
        }
    }
  

  
    //If object collides with other object player looses it
    void OnTriggerEnter(Collider other)
    {

        if (beingCarried && other.CompareTag("Wall"))
        {
            GamificationManager.instance.OneBalloonSpawned = false; //enabbles to pick up next balloon
            touched = true;
        }

      
    }
}