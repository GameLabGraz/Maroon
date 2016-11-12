using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MessageScript : NetworkBehaviour {

    [SyncVar(hook = "OnChangeText")]
    private string text_;
    [SyncVar(hook = "OnChangeColor")]
    private Color col_;
    private Text field_;

	// Use this for initialization
	void Start () {
        field_ = GetComponentInChildren<Text>();
        if (isServer)
        {
            text_ = field_.text;
            col_ = field_.color;
            RpcSetText(text_, col_);
        }
        field_.text = text_;
        field_.color = col_;
        DontDestroyOnLoad(gameObject);
        Destroy(gameObject, 5);
    }

    [ClientRpc]
    void RpcSetText(string t, Color c)
    {
        text_ = t;
        col_ = c;
    }

    void OnChangeText(string text)
    {
        field_.text = text;
    }

    void OnChangeColor(Color color)
    {
        field_.color = color;
    }

}
