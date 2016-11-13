using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

[RequireComponent(typeof(FirstPersonController))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioListener))]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CharacterController))]
public class NetworkPlayerController : NetworkBehaviour {

    public GameObject avatar_;
    public GameObject eyes_;
    public GameObject messagePrefab_;
    private MeshRenderer[] ren;
    private GameObject bar;
    private InputField if_;
    private NetworkManagerHUD gui_;
    private string text_;

    [SyncVar] private Color color;

    void Start()
    {
        ren = avatar_.GetComponentsInChildren<MeshRenderer>();
        if_ = GameObject.FindGameObjectWithTag("Input").GetComponent<InputField>();
        gui_ = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManagerHUD>();
        if (!isLocalPlayer)
        {
            GetComponentInChildren<FirstPersonController>().enabled = false;
            GetComponentInChildren<AudioSource>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<CharacterController>().enabled = false;

            CmdSetColor(color);
            foreach (MeshRenderer i in ren)
            {
                i.material.color = color;
            }
        }
        else
        {
            CmdSetColor(color);
            bar = GameObject.FindGameObjectWithTag("ColorBar");
            bar.GetComponentInChildren<Image>().color = color;
            DontDestroyOnLoad(bar);
            avatar_.SetActive(false);
            eyes_.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer)
        { 
            if (if_.isFocused)
            {
                gui_.showGUI = false;
                GetComponentInChildren<FirstPersonController>().enabled = false;
            }
            else
            {
                gui_.showGUI = true;
                GetComponentInChildren<FirstPersonController>().enabled = true;
            }

            text_ = if_.text;
            if (text_.Length > 0 && Input.GetKeyDown(KeyCode.Return))
            {
                if_.text = "";
                CmdMessage(text_);
            }
        }
    }

    public bool isFocused()
    {
        return if_.isFocused;
    }

    [Command]
    void CmdMessage(string t)
    {
        var msg = (GameObject)Instantiate(
            messagePrefab_,
            new Vector3(0,0,0),
            new Quaternion(0,0,0,0));

        msg.GetComponentInChildren<Text>().text = t;
        msg.GetComponentInChildren<Text>().color = color;

        NetworkServer.Spawn(msg);
    }

    void OnDestroy()
    {
        if(bar != null)
            bar.GetComponentInChildren<Image>().color = Color.white;
    }

    public override void OnStartClient()
    {
        if (isServer)
        {
            color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            RpcSetColor(color);
        }
    }

    [ClientRpc] void RpcSetColor(Color c)
    {
        color = c;
    }

    [Command] void CmdSetColor(Color c)
    {
        color = c;
    }
}
