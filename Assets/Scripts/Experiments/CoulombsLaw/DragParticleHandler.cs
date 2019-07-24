using System.Collections.Generic;
using UnityEngine;

public class DragParticleHandler : MonoBehaviour
{
    public bool pauseSimulationWhileMoving = true;
    public bool deleteIfOutsideBoundaries = true;
    public GameObject MovingObject = null; 
    
    [Header("Movement Restrictions")]
    public Transform minBoundary;
    public Transform maxBoundary;

    public bool allowedXMovement = true;
    public bool allowedYMovement = true;
    public bool allowedZMovement = true;

    [Header("Movement Restrictions Apparences")]
    public List<GameObject> changeMaterialIfOutside;
    public float outsideTransparency = 0.7f;
    
    private bool _moving = false;
    private bool _isOutsideBoundaries = false;
    private float _distance;
    
    private SimulationController simController;
    private CoulombLogic _coulombLogic;
    private Rigidbody _rigidbody;
    private ParticleBehaviour _particleBehaviour;

    private void Start()
    {
        if (MovingObject == null) MovingObject = gameObject;
        
        _particleBehaviour = MovingObject.GetComponent<ParticleBehaviour>();
        _rigidbody = MovingObject.GetComponent<Rigidbody>();
        var simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();
        simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
    }

    private void Update()
    {
        _rigidbody.isKinematic = !simController.SimulationRunning || _particleBehaviour.fixedPosition;
    }

    public void SetBoundaries(GameObject min, GameObject max)
    {
        minBoundary = min.transform;
        maxBoundary = max.transform;
    }
    
    private void OnMouseDown()
    {
        if(!MovingObject.activeSelf) return;
        if (!Input.GetMouseButtonDown(0)) return;

        var arrowControlled = GetComponentInChildren<ArrowControlledMovement>();
        if (arrowControlled && arrowControlled.MouseDown())
            return;
        
        _moving = true;
        _distance = Vector3.Distance(MovingObject.transform.position, Camera.main.transform.position);
        if (pauseSimulationWhileMoving) simController.SimulationRunning = false;
    }

    private void OnMouseDrag()
    {
        var arrowControlled = GetComponentInChildren<ArrowControlledMovement>();
        if (arrowControlled)
            arrowControlled.MouseDrag();
        
        if (!_moving) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var pt = ray.GetPoint(_distance);
        var pos = MovingObject.transform.position;

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
        
        MovingObject.transform.position = pt;
    }

    private void OnMouseUp()
    {
        if (!Input.GetMouseButtonUp(0)) return;
        
        var arrowControlled = GetComponentInChildren<ArrowControlledMovement>();
        if (arrowControlled)
            arrowControlled.MouseUp();
        
        _moving = false;
        MovingObject.GetComponent<ParticleBehaviour>().SetPosition(transform.position);
        simController.ResetSimulation();
        
        if (_isOutsideBoundaries && deleteIfOutsideBoundaries)
        {
            _coulombLogic.RemoveParticle(MovingObject.GetComponent<ParticleBehaviour>(), true);
            simController.ResetSimulation();
        }
        
    }
}
