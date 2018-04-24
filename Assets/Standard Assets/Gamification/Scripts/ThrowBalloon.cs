using UnityEngine;
using System.Collections;

public class ThrowBalloon : MonoBehaviour
{
    private ConstantForce cf;
    private GameObject player;
    private GameObject playerCam;
    private float throwForce = 200;
    // bool GamificationManager.instance.hasPlayer = false;
    bool beingCarried = false;
    public int dmg;
    private bool touched = false;
    public bool isBalloon = false;
    private bool firstcarried = false;
    private bool trigger = false;
    private DialogueManager dMan;
    public string ID;
    private bool isPlaced = false;


    private void Awake()
    {
        playerCam = GameObject.FindWithTag("MainCamera");
        player = GameObject.FindWithTag("Player");
        dMan = FindObjectOfType<DialogueManager>();
    }

    void Start()
    {
        cf = GetComponent<ConstantForce>();
        // uiBox = GameObject.FindGameObjectWithTag("UI");
        if (isPlaced)
            this.gameObject.SetActive(false);
    }




  

          
        
    


    void Update()
    {

        if (beingCarried)
            GamificationManager.instance.holdingItem = true;
        else
            GamificationManager.instance.holdingItem = false;

        if (GamificationManager.instance.hasPlayer)
            GamificationManager.instance.playerCanPickItem = true;
        else
            GamificationManager.instance.playerCanPickItem = false;



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
            GamificationManager.instance.hasPlayer = true;
        }
        else
        {
            GamificationManager.instance.hasPlayer = false;
        }
        if (GamificationManager.instance.hasPlayer && !beingCarried && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Carry");
            GetComponent<Rigidbody>().isKinematic = true;
            transform.parent = playerCam.transform;
            beingCarried = true;
            trigger = true;

        }
        if (GamificationManager.instance.hasPlayer && beingCarried && Input.GetMouseButtonUp(0))
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
                PlaceItem();
            }
            else if (Input.GetMouseButtonDown(1) && !trigger)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                transform.parent = null;
                beingCarried = false;
                GamificationManager.instance.OneBalloonSpawned = false;
                PlaceItem();
            }
        }
    }

    //Check if item is placed correctly. ADD NEW EXPERIMENTS HERE
    void PlaceItem()
    {
        Collider collider = this.GetComponent<Collider>();
        BuildVandegraaf2 script = FindObjectOfType<BuildVandegraaf2>();
        BuildVandegraaf1 script2 = FindObjectOfType<BuildVandegraaf1>();
        BuildFallingcoil script3 = FindObjectOfType<BuildFallingcoil>();
        BuildFaradayslaw script4 = FindObjectOfType<BuildFaradayslaw>();
        BuildPendulum script5 = FindObjectOfType<BuildPendulum>();



        if (script.IsOverlapping(collider, ID) || script2.IsOverlapping(collider, ID)
            || script3.IsOverlapping(collider, ID) || script4.IsOverlapping(collider, ID)
            || script5.IsOverlapping(collider, ID))
        {
            isPlaced = true;
            SoundManager.instance.PlaySingle(GamificationManager.instance.AchievementSound);
            this.gameObject.SetActive(false);
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