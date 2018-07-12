using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[HelpURL("https://www.olip.at/ditronic/")]
public class RealTimeClock : MonoBehaviour {

	// Use this for initialization
	[Header("Reference The Clock:")]
	[Tooltip("Reference to hours's Pointer")]
	public Transform hourTransform;
	[Tooltip("Reference to minute's Pointer")]
	public Transform minuteTransform;
	[Header("Adjust the Clock:")]
	[Range(0f,359.9f)]
	[Tooltip("Value in euler degrees how big is the angle between the Hours pointer and 12'o clock, when you write \"0\" in transform's rotation values")]
	public float offsetHours = 0f;
	[Range(0f,359.9f)]
	[Tooltip("Value in euler degrees how big is the angle between the Minutes pointer and 12'o clock, when you write \"0\" in transform's rotation values")]
	public float offsetMinutes = 0f;

	void Start () {
		if (minuteTransform == null || hourTransform == null) {
			Debug.LogWarning ("Reference to Clock Pointer not set");
			Destroy (this);
		}
	}
	
	// Update is called once per frame
	void Update () {
		hourTransform.rotation = Quaternion.Euler(180f,0f,floatToWatchFace(getfloatHour()/12f,offsetHours));
		minuteTransform.rotation = Quaternion.Euler(180f,0f,floatToWatchFace(getfloatMinute()/60f,offsetMinutes));
	}
	//don't touch my privates

	private float getfloatMinute()
	{
		return (float)DateTime.Now.Minute + ((float)DateTime.Now.Second / 60f);
	}
	private float getfloatHour()
	{
		return (float)DateTime.Now.Hour + (getfloatMinute () / 60f);
	}
	private float floatToWatchFace(float time, float offset)
	{
		return time*360f + offset;
	}
}
