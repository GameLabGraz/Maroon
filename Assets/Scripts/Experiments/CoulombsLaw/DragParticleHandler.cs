using System.Collections.Generic;
using UnityEngine;

public class DragParticleHandler : MonoBehaviour
{
    public bool pauseSimulationWhileMoving = true;
    public bool deleteIfOutsideBoundaries = true;
    public GameObject movingObject = null; 
    
    [Header("Movement Restrictions")]
    public Transform minBoundary;
    public Transform maxBoundary;

    public bool allowedXMovement = true;
    public bool allowedYMovement = true;
    public bool allowedZMovement = true;

    [Header("Movement Restrictions Appearances")]
    public List<GameObject> changeMaterialIfOutside;
    public float outsideTransparency = 0.7f;
    
    private bool _moving = false;
    private bool _isOutsideBoundaries = false;
    private float _distance;
    
    private SimulationController _simController;
    private CoulombLogic _coulombLogic;
    private Rigidbody _rigidbody;
    private ParticleBehaviour _particleBehaviour;

    private void Start()
    {
        if (movingObject == null) movingObject = gameObject;
        
        _particleBehaviour = movingObject.GetComponent<ParticleBehaviour>();
        _rigidbody = movingObject.GetComponent<Rigidbody>();
        var simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            _simController = simControllerObject.GetComponent<SimulationController>();
        simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
    }

    private void Update()
    {
        _rigidbody.isKinematic = !_simController.SimulationRunning || _particleBehaviour.fixedPosition;
    }

    public void SetBoundaries(GameObject min, GameObject max)
    {
        minBoundary = min.transform;
        maxBoundary = max.transform;
    }
    
    private void OnMouseDown()
    {
        if(!movingObject.activeSelf) return;
        if (!Input.GetMouseButtonDown(0)) return;
        
        _moving = true;
        _distance = Vector3.Distance(movingObject.transform.position, Camera.main.transform.position);
        if (pauseSimulationWhileMoving) _simController.SimulationRunning = false;
    }

    private void OnMouseDrag()
    {
        if (!_moving) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var pt = ray.GetPoint(_distance);
        var pos = movingObject.transform.position;

        if (!allowedXMovement) pt.x = pos.x;
        if (!allowedYMovement) pt.y = pos.y;
        if (!allowedZMovement) pt.z = pos.z;

        var outside = false;
        if (allowedXMovement && (pt.x < minBoundary.position.x || maxBoundary.position.x < pt.x))
            outside = true;
        else if (allowedYMovement && (pt.y < minBoundary.position.y || maxBoundary.position.y < pt.y))
            outside = true;
        else if (allowedZMovement && (pt.z < minBoundary.position.z || maxBoundary.position.z < pt.z))
            outside = true;
        
        // ReSharper disable once RedundantCheckBeforeAssignment
        if (outside != _isOutsideBoundaries)
        {
            _isOutsideBoundaries = outside;

            foreach(var obj in changeMaterialIfOutside)
            {
                if(!obj.activeSelf) continue;
                foreach (var mat in obj.GetComponent<MeshRenderer>().materials)
                {
                    var col = mat.color;
                    col.a = outside ? outsideTransparency : 1f;
                    mat.color = col;
                }
            }
        }
        
        movingObject.transform.position = pt;
    }

    private void OnMouseUp()
    {
        if (!Input.GetMouseButtonUp(0)) return;
        
        _moving = false;
        movingObject.GetComponent<ParticleBehaviour>().SetPosition(transform.position);
        _simController.ResetSimulation();
        
        if (_isOutsideBoundaries && deleteIfOutsideBoundaries)
        {
            _coulombLogic.RemoveParticle(movingObject.GetComponent<ParticleBehaviour>(), true);
            _simController.ResetSimulation();
        }
    }
}
