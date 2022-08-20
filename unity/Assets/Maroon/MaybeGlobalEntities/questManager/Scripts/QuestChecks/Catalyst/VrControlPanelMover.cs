using System;
using UnityEngine;
    
public class VrControlPanelMover : MonoBehaviour
{
    [SerializeField] CatalystVrControlPanel controlPanel;
    [SerializeField] Material highlightMaterial;

    private MeshRenderer _meshRenderer;
    private Material _initialMaterial;
    private bool _isTouched;

    private void Start()
    {
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        _initialMaterial = _meshRenderer.material;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_isTouched && other.gameObject.name.Contains("HandColliderRight"))
        {
            _isTouched = true;
            _meshRenderer.material = highlightMaterial;
            controlPanel.UpdatePositionToPlayer();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (_isTouched && other.gameObject.name.Contains("HandColliderRight"))
        {
            _isTouched = false;
            _meshRenderer.material = _initialMaterial;
        }
    }
}