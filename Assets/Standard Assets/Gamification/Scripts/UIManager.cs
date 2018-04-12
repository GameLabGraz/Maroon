using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    private GameObject uiBox;
    private bool setUI = false;
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
        setUI = true;
        uiBox.SetActive(true);
    }

    public void HideUI()
    {
        setUI = false;
        if (!collided)
         uiBox.SetActive(false);
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
