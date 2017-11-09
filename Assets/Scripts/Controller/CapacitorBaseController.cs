using System;
using UnityEngine;

public class CapacitorBaseController : MonoBehaviour, IVoltagePoleTrigger
{
    [SerializeField]
    private GameObject capacitorPlate;

    [SerializeField]
    private float electronRadius;

    [SerializeField]
    private float electronDistance;

    private int numberOfElectronsPerRow = 0;

    private int numberOfRows = 0;

    private int numberOfElectrons = 0;


    private void Start()
    {
        numberOfElectronsPerRow = (int)(capacitorPlate.transform.localScale.x / ((electronRadius + electronDistance) * 2));

        numberOfRows = (int)(capacitorPlate.transform.localScale.y / ((electronRadius + electronDistance) * 2));
    }

    public void PullVoltagePoleTrigger(Collider other, GameObject source)
    {
        if(capacitorPlate == null)
        {
            Destroy(other.gameObject);
            return;
        }

        other.GetComponent<PathFollower>().followPath = false;
        other.transform.position = GetNextElectronPositionOnPlate();
        other.transform.parent = this.transform;
    }

    Vector3 GetNextElectronPositionOnPlate()
    {
        Vector3 position = capacitorPlate.transform.position;

        if (numberOfElectrons  > numberOfElectronsPerRow * numberOfRows)
            return position;

       
        if (numberOfElectrons % 2 == 0)
            position.x += (electronRadius + electronDistance) * 2 * (int)((numberOfElectrons + 2)  / 2);
        else
            position.x -= (electronRadius + electronDistance) * 2 * (int)((numberOfElectrons + 2) / 2);


        numberOfElectrons++;
        return position;
    }

}
