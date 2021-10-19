using UnityEngine;

/// <summary>
///     This script identifies the (virtual reality) object that defines the position used for tracking. It outputs the
///     (raw) current position. Any offsets and settings are applied in the receiving script.
/// </summary>
public class SecondScreenPositionTracker : MonoBehaviour
{
    private SecondScreenPositionUpdater _positionUpdater = null;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // MonoBehaviour Functions
    private void Start()
    {
        Debug.Log("TODO: Initialized SecondScreenPositionTracker");
    }

    private void OnDisable()
    {
        NotifySecondScreenPositionTrackerDisable();
    }

    private void OnDestroy()
    {
        NotifySecondScreenPositionTrackerDisable();
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Linking

    public void SetSecondScreenPositionUpdater(SecondScreenPositionUpdater positionUpdater)
    {
        this._positionUpdater = positionUpdater;
    }

    private void NotifySecondScreenPositionTrackerDisable()
    {
        this._positionUpdater.OnSecondScreenPositionTrackerDisabled();
    }
}
