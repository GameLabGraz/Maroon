using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK.UnityEventHelper;

public class SceneLoadOnTouch : MonoBehaviour
{
    private VRTK_InteractableObject_UnityEvents _controllerEvents;

    public String SceneName;
    
    private void Awake()
    {
        _controllerEvents = GetComponent<VRTK_InteractableObject_UnityEvents>();
    }

    private void Start()
    {
        _controllerEvents.OnUse.AddListener((o, e) => SceneManager.LoadScene(SceneName));
    }
}
