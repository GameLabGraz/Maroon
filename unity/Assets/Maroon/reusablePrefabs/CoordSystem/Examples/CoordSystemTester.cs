using Maroon.Physics.CoordinateSystem;
using UnityEngine;

public class CoordSystemTester : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Unit[] array = {Unit.cm, Unit.m, Unit.dm};
    
    [SerializeField] private float _x;
    [SerializeField] private float _y;
    [SerializeField] private float _z;

    public float XValue
    {
        get => _x;
        set
        {
            _x = value;
            this.transform.position = CoordSystem.Instance.GetPositionInWorldSpace(new Vector3(_x,_y,_z));
            Debug.Log($" Unit coords {CoordSystem.Instance.GetPositionInAxisUnits(transform.position)}");
        }
    }
    
    public float YValue
    {
        get => _y;
        set
        {
            _y = value;
            this.transform.position = CoordSystem.Instance.GetPositionInWorldSpace(new Vector3(_x,_y,_z));
            Debug.Log($" Unit coords {CoordSystem.Instance.GetPositionInAxisUnits(transform.position)}");
        }
    }
    
    public float ZValue
    {
        get => _z;
        set
        {
            _z = value;
            this.transform.position = CoordSystem.Instance.GetPositionInWorldSpace(new Vector3(_x,_y,_z));
            Debug.Log($" Unit coords {CoordSystem.Instance.GetPositionInAxisUnits(transform.position)}");
        }
    }
}