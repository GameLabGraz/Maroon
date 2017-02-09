using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(FirstPersonController))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioListener))]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CharacterController))]
public class NetworkPlayerController : NetworkBehaviour {

    public GameObject avatar_;
    public GameObject eyes_;
    public GameObject messagePrefab_;
    private MeshRenderer[] ren_;
    private GameObject bar_;
    private InputField if_;
    private NetworkManagerHUD gui_;
    private Camera cam_;
    private FirstPersonController fps_;
    private string text_;

    [SyncVar] private Color color;

    void Start()
    {
        ren_ = avatar_.GetComponentsInChildren<MeshRenderer>();
        if_ = GameObject.FindGameObjectWithTag("Input").GetComponent<InputField>();
        gui_ = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManagerHUD>();
        cam_ = GetComponentInChildren<Camera>();
        fps_ = GetComponentInChildren<FirstPersonController>();

        if (!isLocalPlayer)
        {
            fps_.enabled = false;
            cam_.enabled = false;
            GetComponentInChildren<AudioSource>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            GetComponentInChildren<CharacterController>().enabled = false;

            CmdSetColor(color);
            foreach (MeshRenderer i in ren_)
            {
                i.material.color = color;
            }
        }
        else
        {
            gui_.showGUI = false;
            CmdSetColor(color);
            bar_ = GameObject.FindGameObjectWithTag("ColorBar");
            bar_.GetComponentInChildren<Image>().color = color;
            //DontDestroyOnLoad(bar);
            avatar_.SetActive(false);
            eyes_.SetActive(false);

            SceneManager.activeSceneChanged += switchCamera; // subscribe

            //cleanup messages if any
            GameObject[] old = GameObject.FindGameObjectsWithTag("Message");
            if (old.Length > 0)
            {
                foreach (GameObject o in old)
                    o.SetActive(false);
            }
        }
    }

    private void switchCamera(Scene previousScene, Scene newScene)
    {
        cam_.enabled = !cam_.enabled;
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer)
        {
            if (if_.isFocused)
            {
                gui_.showGUI = false;
                fps_.enabled = false;
            }
            else
            {
                //gui_.showGUI = true;
                fps_.enabled = true;
            }

            text_ = if_.text;

            if (text_.Length > 0 && Input.GetKeyDown(KeyCode.Return))
            {
                if_.text = "";
                CmdMessage(text_);
                if_.ActivateInputField();
            }
            if (text_.Length == 0 && Input.GetKeyDown(KeyCode.Return))
                if_.ActivateInputField();
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
        if (isLocalPlayer)
        {
            if (bar_ != null)
                bar_.GetComponentInChildren<Image>().color = Color.white;
            if (gui_ != null)
                gui_.showGUI = true;
            SceneManager.activeSceneChanged -= switchCamera; // unsubscribe
        }
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
