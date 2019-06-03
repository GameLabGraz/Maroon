using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BField_dyn : BField
{
    private CoulombLogic _coulombLogic;
    public void Start()
    {
        GameObject simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
    }
    
    public override Vector3 get(Vector3 position)
    {
        var field = _coulombLogic.GetParticles().Where(x => x.transform.gameObject.activeSelf).Aggregate(Vector3.zero, (current, x) => current + x.getB(position));
        return field.normalized;
    }
}
