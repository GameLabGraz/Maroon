using UnityEngine;
using VRTK.UnityEventHelper;

public class ResetColorOnUngrab : MonoBehaviour {

	private Color _startColor;
	private VRTK_InteractableObject_UnityEvents _events;
	private ChargedParticle _cp;

	public void Awake()
	{
		_events = GetComponent<VRTK_InteractableObject_UnityEvents>();
		_cp = GetComponent<ChargedParticle>();
	}
	
	public void Start()
	{
		_events.OnUngrab.AddListener((o,e) => _cp.UpdateColor());
		_events.OnUntouch.AddListener((o,e) => _cp.UpdateColor());
	}
}
