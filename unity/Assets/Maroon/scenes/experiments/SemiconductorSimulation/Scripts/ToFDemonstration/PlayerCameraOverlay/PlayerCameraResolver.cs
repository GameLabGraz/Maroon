using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// VR and non VR cameras are different. This class resolves this correctly.
/// </summary>
public class PlayerCameraResolver : MonoBehaviour
{
    [SerializeField] private PlayerCameraSlot[] _possiblePlayerCameras;

    private readonly List<PlayerCameraSlot> _playerCameras = new List<PlayerCameraSlot>();

    /// <summary>
    /// Gives correct PlayerCamera. VR cameras are different than edit scene cameras.
    /// </summary>
    public List<PlayerCameraSlot> GetPlayerCameras()
    {
        _playerCameras.Clear();

        for (int i = 0; i < _possiblePlayerCameras.Length; i++)
        {
            if (_possiblePlayerCameras[i].Camera != null && _possiblePlayerCameras[i].Camera.isActiveAndEnabled)
                _playerCameras.Add(_possiblePlayerCameras[i]);
        }
        if (_playerCameras.Count == 0)
        {
            Debug.LogWarning("PlayerCameraResolver::GetPlayerCameras: no active player cameras found.");
        }

        return _playerCameras;
    }
}
