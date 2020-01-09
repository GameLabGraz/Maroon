using UnityEngine;
using UnityEngine.UI;

public class PC_RegisterBase : MonoBehaviour
{
    public Color activeColor = new Color(1f, 1f, 1f, 0.4f);
    public Color inactiveColor = new Color(0.4f, 0.4f, 0.4f, 0.4f);

    public bool selectedOnStart;
    public GameObject registerTag;
    public GameObject registerContent;

    public PC_RegisterHandler registerHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        registerHandler.RegisterBaseRegister(this, selectedOnStart);
    }

    public void SetInactive()
    {
        registerContent.SetActive(false);
        registerTag.GetComponent<Image>().color = inactiveColor;
    }

    public void SetActive()
    {
        registerContent.SetActive(true);
        registerTag.GetComponent<Image>().color = activeColor;
    }

    public void Select()
    {
        registerHandler.SelectRegister(this);
    }
}
