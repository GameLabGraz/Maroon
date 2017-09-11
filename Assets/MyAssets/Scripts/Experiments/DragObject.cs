using UnityEngine;
using System.Collections;

public class DragObject : MonoBehaviour
{
	public Camera MainCamera;
	private Vector3 offset;
	private Vector3 screenPoint;
	private Transform cachedTransform;
	
	virtual public void Awake()
	{
		cachedTransform = transform;
	}

	void OnMouseDown()
	{
		Vector3 pos = cachedTransform.position;
		screenPoint = MainCamera.WorldToScreenPoint(pos);
		offset = pos - MainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	}
	
	void OnMouseDrag()
	{
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = MainCamera.ScreenToWorldPoint(curScreenPoint) + offset;
		// don't change the x-axis, so the glass rod is only movable in y- and z-axis
		Vector3 newPosition = new Vector3(cachedTransform.position.x, curPosition.y, curPosition.z);
		cachedTransform.position = newPosition;
	}
}

