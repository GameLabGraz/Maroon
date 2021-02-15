using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PC_RegisterBase : MonoBehaviour
{
    public Color activeColor = new Color(1f, 1f, 1f, 0.4f);
    public Color inactiveColor = new Color(0.4f, 0.4f, 0.4f, 0.4f);
    public Color notInteractableColor = new Color(0.6f, 0.6f, 0.6f, 0.4f);
    public bool hideWhenInactive = false;

    public bool selectedOnStart;
    public GameObject registerTag;
    public GameObject registerContent;

    public PC_RegisterHandler registerHandler;

    private bool _interactable = true;

    // Start is called before the first frame update
    void Awake()
    {
        registerHandler.RegisterBaseRegister(this, selectedOnStart);
    }

    public void SetInactive()
    {
        if(hideWhenInactive)
            gameObject.SetActive(false);
        else
        {
            registerContent.SetActive(false);
            registerTag.GetComponent<Image>().color = inactiveColor;
        }
    }

    public void SetActive()
    {
        if (!_interactable) return;
        
        gameObject.SetActive(true);
        registerContent.SetActive(true);
        registerTag.GetComponent<Image>().color = activeColor;
    }

    public void Select()
    {
        if (!_interactable) return;
        registerHandler.SelectRegister(this);
    }

    public void SetInteractable(bool interactable)
    {
        if (interactable == _interactable) return;
        _interactable = interactable;
        
        if(_interactable)
            SetInactive();
        else
        {
            registerHandler.DeselectRegister(this);
            SetInactive();
            registerTag.GetComponent<Image>().color = inactiveColor;
        }

        var text = registerTag.GetComponentInChildren<TMP_Text>();
        if (text) 
            text.color = _interactable ? Color.black : Color.gray;
    }
}
