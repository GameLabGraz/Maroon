using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollider : MonoBehaviour
{
    public bool ignoreInOwnChildren;
    public bool ignoreInOtherChildren;

    public List<GameObject> IgnoreColliders = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (var collider in GetComponents<Collider>())
        {
            IgnoreFromOther(collider);
        }

        if (!ignoreInOwnChildren) return;
        foreach (var collider in GetComponentsInChildren<Collider>())
            IgnoreFromOther(collider);
        
    }

    private void IgnoreFromOther(Collider myCollider)
    {
        foreach (var obj in IgnoreColliders)
        {
            foreach (var collider in obj.GetComponents<Collider>())
            {
                Physics.IgnoreCollision(myCollider, collider);
            }

            if (!ignoreInOtherChildren) continue;
            
            foreach (var collider in obj.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(myCollider, collider);
            }
        }
    }
}
