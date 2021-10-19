using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This script receives the transform of a (virtual reality) object that defines the position used for tracking.
///     It handles offsets and applies the transform to the camera.
/// </summary>
public class SecondScreenPositionUpdater : MonoBehaviour
{
    private SecondScreenPositionTracker _positionTracker = null;

    private int _reconnectDelay = 5;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // MonoBehaviour Functions

    private void Start()
    {
        LinkSecondScreenPositionTracker();
    }

    // Update is called once per frame
    private void Update()
    {
        SyncPosition();
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Linking
    private void LinkSecondScreenPositionTracker()
    {
        // Try to find SecondScreenPositionTracker
        this._positionTracker = FindObjectOfType<SecondScreenPositionTracker>();

        // If not found, try again later
        if(this._positionTracker == null)
        {
            Debug.Log("[SecondScreenPositionUpdater] SecondScreenPositionTracker not found");
            Invoke("LinkSecondScreenPositionTracker", this._reconnectDelay);
        }

        // If found, set connection
        this._positionTracker.SetSecondScreenPositionUpdater(this);
    }

    public void OnSecondScreenPositionTrackerDisabled()
    {
        this._positionTracker = null;
        Invoke("LinkSecondScreenPositionTracker", this._reconnectDelay);
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Position Update

    private void SetOffset()
    {
        // TODO: Calculate and set offset based on user calibration
    }

    private void SyncPosition()
    {
        // Check if position tracker is initialized
        if(this._positionTracker == null)
        {
            return;
        }

        // TODO: Apply offset

        // Apply position and rotation, ignore scaling
        this.gameObject.transform.position = this._positionTracker.transform.position;
        this.gameObject.transform.rotation = this._positionTracker.transform.rotation;
    }
}
