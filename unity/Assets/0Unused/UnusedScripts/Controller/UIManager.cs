using UnityEngine;

public class UIManager : MonoBehaviour {

    private GameObject uiBox;
    public static UIManager instance = null; //so we have access to Manager from other files
    private bool collided = false;

    public void ShowUICollided()
    {
        collided = true;
        uiBox.SetActive(true);
    }

    public void HideUICollided()
    {
        collided = false;
        uiBox.SetActive(false);
    }

    public void ShowUI()
    {
        uiBox.SetActive(true);
        Debug.Log("Show");
    }

    public void HideUI()
    {
        if (!collided)
        {
            uiBox.SetActive(false);
            Debug.Log("Hide");
        }
    }

    private void Awake()
    {
        instance = this;
        uiBox = GameObject.FindWithTag("UI");
    }

    // Use this for initialization
    void Start ()
    {
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
