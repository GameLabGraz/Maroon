using UnityEngine;
using System.Collections;

public class ThrowObject : MonoBehaviour
{
    private ConstantForce cf;
    private GameObject player;
    private GameObject playerCam;
    private float throwForce = 200;
    public  bool hasPlayer = false;
    bool beingCarried = false;
    public int dmg;
    private bool touched = false;
    public bool isBalloon = false;
    private bool firstcarried = false;
    private bool trigger = false;
    private DialogueManager dMan;
    public string ID;
    private bool isPlaced = false;
    private MeshRenderer render;
    private Color color;

    private void Awake()
    {
        playerCam  = GameObject.FindWithTag("MainCamera");
        player = GameObject.FindWithTag("Player");
        dMan = FindObjectOfType<DialogueManager>();
        if (ID != "magnet" && ID != "weight")
        {
            render = gameObject.GetComponent<MeshRenderer>();
            color = render.material.color;
        }      
    }

    void Start()
    {
        cf = GetComponent<ConstantForce>();
        // uiBox = GameObject.FindGameObjectWithTag("UI");
        if (isPlaced)
            this.gameObject.SetActive(false);
    }


  
    //0 = transparent, 1 = normal
    void Alpha(int i)
    {
        if (ID != "magnet" && ID != "weight")
        {
            color.a = i;
            render.material.color = color;
            Debug.Log("no trans");
        }
        if (i == 0)
            SoundManager.instance.PlaySingle(SoundManager.instance.pickupSound);
        else
            SoundManager.instance.PlaySingle(SoundManager.instance.pickdownSound);

    }

    void Update()
    {
       
        if (beingCarried)
        {
            GamificationManager.instance.holdingItem = true;
                
        }
        else
        {               
            GamificationManager.instance.holdingItem = false;
        }
        if (hasPlayer)
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
      

        if (hasPlayer && !beingCarried && Input.GetMouseButtonDown(0))
        {
            //Item is picked up
            Debug.Log("Carry");
            GetComponent<Rigidbody>().isKinematic = true;
            transform.parent = playerCam.transform;
            beingCarried = true;
            trigger = true;
            Alpha(0);
          
          

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
                Alpha(1);
                touched = false;
               
            }
            if (Input.GetMouseButtonDown(0) && !trigger)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                transform.parent = null;
                beingCarried = false;
                Alpha(1);
                GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * throwForce);
                GamificationManager.instance.OneBalloonSpawned = false;
                PlaceItem();
            }
            else if (Input.GetMouseButtonDown(1) && !trigger)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                transform.parent = null;
                beingCarried = false;
                Alpha(1);
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