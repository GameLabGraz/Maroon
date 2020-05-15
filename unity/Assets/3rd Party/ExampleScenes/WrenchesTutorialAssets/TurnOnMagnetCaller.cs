using UnityEngine;

public class TurnOnMagnetCaller : MonoBehaviour {
	void OnMouseUp() {
        GameObject.Find("StopAndStickTrigger").GetComponent<TurnOnMagnet>().OnMouseUp();
	}
}
