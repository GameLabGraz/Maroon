using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capacitor : MonoBehaviour
{
    [SerializeField]
    private GameObject plate1;

    [SerializeField]
    private GameObject plate2;

    [SerializeField]
    private float seriesResistance = 500;

    private float vacuumPermittivity = 8.8542e-12f;

    private float relativePermittivity = 1.0f;

    private float capacitance;

    private float voltage;

	private void Start ()
    {
        if(plate1 == null || plate2 == null)
        {
            Debug.LogError("Capacitor requires two plates!");
            gameObject.SetActive(false);
        }
	}

    private void FixedUpdate()
    {
        capacitance = (GetOverlapPlateArea() * vacuumPermittivity * relativePermittivity) / GetPlateDistance();
    }

    private float GetOverlapPlateArea()
    {
        float area = 0;
        float overlapWidth = 0;
        float overlapHeight = 0;      

        Vector3 plate1Size = plate1.GetComponent<Renderer>().bounds.size;
        Vector3 plate2Size = plate2.GetComponent<Renderer>().bounds.size;

        Vector3 plate1WidthCorner = plate1.transform.position + new Vector3(plate1Size.x / 2, 0, 0);
        Vector3 plate1HeightCorner = plate1.transform.position + new Vector3(0, plate1Size.y / 2, 0);

        Vector3 testDirection = plate2.transform.position - plate1.transform.position;

        if(CheckIfOverlapPlate(plate1WidthCorner, testDirection, plate2))
            overlapWidth = plate1Size.x;
        else
            overlapWidth = plate2Size.x;

        if (CheckIfOverlapPlate(plate1HeightCorner, testDirection, plate2))
            overlapHeight = plate1Size.y;
        else
            overlapHeight = plate2Size.y;

        area = overlapHeight * overlapWidth;
        return area;
    }

    private bool CheckIfOverlapPlate(Vector3 cornerPoint, Vector3 testDirection, GameObject plateToCheck)
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(cornerPoint, testDirection, out hit, GetPlateDistance()))
        {
            if (hit.collider.gameObject == plateToCheck)
                return true;
        }
        return false;
    }

    public float GetPlateDistance()
    {
        return Vector3.Distance(plate1.transform.position, plate2.transform.position);
    }

    public void SetRelativePermittivity(float relativePermittivity)
    {
        this.relativePermittivity = relativePermittivity;
    }
}
