using System;
using UnityEngine;
using VRTK;

public class VR_CoulombAddOnCollision : MonoBehaviour
{
    private CoulombLogic _coulombLogic;

    public Transform ReferencePosition;
    public bool MatchXCoord;
    public bool MatchYCoord;
    public bool MatchZCoord;

    // Start is called before the first frame update
    void Start()
    {
        var obj = GameObject.Find("CoulombLogic");
        if (obj)
            _coulombLogic = obj.GetComponent<CoulombLogic>();
        
        Debug.Assert(_coulombLogic != null);
    }


    private void OnTriggerEnter(Collider other)
    {
        var coulombBehavior = other.gameObject.GetComponent<CoulombChargeBehaviour>();
        if (coulombBehavior)
        {
            coulombBehavior.UpdateResetPosition();
            _coulombLogic.AddParticle(coulombBehavior, false);

            var interactObj = other.gameObject.GetComponent<VRTK_InteractableObject>();
            if (interactObj)
            {
                interactObj.InteractableObjectUngrabbed += (sender, args) =>
                {
                    coulombBehavior = interactObj.GetComponent<CoulombChargeBehaviour>();
                    if (!_coulombLogic.ContainsParticle(coulombBehavior)) return;

                    if (ReferencePosition != null)
                    {
                        var pos = other.transform.position;
                        if (MatchXCoord)
                            pos.x = ReferencePosition.position.x;
                        if (MatchYCoord)
                            pos.y = ReferencePosition.position.y;
                        if (MatchZCoord)
                            pos.z = ReferencePosition.position.z;

                        other.transform.rotation = ReferencePosition.rotation;
                        other.transform.position = pos;
                    }

                    coulombBehavior.UpdateResetPosition();
                };
            }
        }
    }
    
    

    private void OnTriggerExit(Collider other)
    {
        var coulombBehavior = other.gameObject.GetComponent<CoulombChargeBehaviour>();
        
        if (coulombBehavior)
        {
            _coulombLogic.RemoveParticle(coulombBehavior);
        }
    }
}