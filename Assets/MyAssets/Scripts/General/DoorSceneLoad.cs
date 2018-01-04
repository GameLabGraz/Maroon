using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK.UnityEventHelper;

public class DoorSceneLoad : MonoBehaviour
{
	private VRTK_InteractableObject_UnityEvents _events;

	public string SceneName;
	
	private void Awake()
	{
		_events = GetComponentInChildren<VRTK_InteractableObject_UnityEvents>();
	}

	private void Start()
	{
		_events.OnUse.AddListener((o, e)=>SceneManager.LoadScene(SceneName));
	}
}
