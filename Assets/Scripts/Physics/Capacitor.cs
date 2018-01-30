using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capacitor : PausableObject
{
    private enum ChargeState {IDLE, CHARGING, DISCHARGING};

    [SerializeField]
    private GameObject plate1;

    [SerializeField]
    private GameObject plate2;

    [SerializeField]
    private float seriesResistance = 5e10f;

    private const float vacuumPermittivity = 8.8542e-12f;

    private Dielectric dielectric;

    private float capacitance;

    [SerializeField]
    private float powerVoltage = 15;

    private float previousPowerVoltage = 0;

    private float voltage = 0;


    private float chargeTime = 0;

    private ChargeState chargeState = ChargeState.IDLE;

    [SerializeField]
    private ChargePoolHandler chargePoolHander;

    protected override void Start()
    {
        base.Start();

        if(plate1 == null || plate2 == null)
        {
            Debug.LogError("Capacitor requires two plates!");
            gameObject.SetActive(false);
        }

        if(chargePoolHander == null)
        {
            chargePoolHander = GameObject.FindObjectOfType<ChargePoolHandler>();
            if (chargePoolHander == null)
                Debug.LogError("Capacitor requires a charge pool handler!");
        }

        dielectric = GameObject.FindObjectOfType<Dielectric>();
	}

    protected override void Update()
    {
        float relativePermittivity = 1.0f;
        if (dielectric != null)
            relativePermittivity = dielectric.GetRelativePermittivity();

        capacitance = (GetOverlapPlateArea() * vacuumPermittivity * relativePermittivity) / GetPlateDistance();

        base.Update();
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

        if (CheckIfOverlapPlate(plate1WidthCorner, testDirection, plate2))
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

    protected override void HandleUpdate()
    {

    }

    protected override void HandleFixedUpdate()
    {
        switch(chargeState)
        {
            case ChargeState.IDLE:
                chargeTime = 0;

                if (powerVoltage > voltage)
                {
                    chargeState = ChargeState.CHARGING;
                    StartCoroutine("ElectronChargeEffect");
                }
                   
                else if (powerVoltage < voltage)
                {
                    chargeState = ChargeState.DISCHARGING;
                    StartCoroutine("ElectronDischargeEffect");
                }                   

                break;

            case ChargeState.CHARGING:              
                if (voltage >= powerVoltage * 0.99f)
                {
                    chargeState = ChargeState.IDLE;
                    previousPowerVoltage = voltage;
                    break;
                }
                else
                {
                    Charge();
                    chargeTime += Time.fixedDeltaTime * 0.25f;
                }
                break;

            case ChargeState.DISCHARGING:
                if (voltage <= powerVoltage)
                {
                    chargeState = ChargeState.IDLE;
                    previousPowerVoltage = voltage;
                }
                else
                {
                    Discharge();
                    chargeTime += Time.fixedDeltaTime * 0.25f;
                }
                break;

            default:
                break;
        }
    }

    private IEnumerator ElectronChargeEffect()
    {
        GameObject plusCable = GameObject.Find("Cable+");

        int numberOfElectrons = (int)(powerVoltage - voltage) * 2;

        float electronTimeInterval = 5 * seriesResistance * capacitance / numberOfElectrons;
        float electronSpeed = 0.01f;

        while (numberOfElectrons > 0 && chargeState == ChargeState.CHARGING)
        {
            Charge electron = chargePoolHander.GetNewElectron();

            electron.transform.position = plate2.transform.position;

            PathFollower pathFollower = electron.GetComponent<PathFollower>();
            pathFollower.SetPath(plusCable.GetComponent<IPath>());
            pathFollower.maxSpeed = electronSpeed;           

            numberOfElectrons--;

            yield return new WaitForSecondsWithPause(electronTimeInterval);
        }
    }

    private IEnumerator ElectronDischargeEffect()
    {
        GameObject minusCable = GameObject.Find("Cable-");

        int numberOfElectrons = (int)Mathf.Round(voltage - powerVoltage) * 2;

        float electronTimeInterval = 5 * seriesResistance * capacitance / numberOfElectrons;
        float electronSpeed = 0.01f;

        while (numberOfElectrons > 0 && chargeState == ChargeState.DISCHARGING)
        {
            List<Charge> electronsOnPlate = plate1.GetComponent<CapacitorPlateController>().GetCharges();

            if (electronsOnPlate.Count - 1 < 0)
                break;

            Charge electron = electronsOnPlate[electronsOnPlate.Count - 1];
            electron.transform.position = plate1.transform.position;
            electron.GetComponent<Charge>().JustCreated = true;

            plate1.GetComponent<CapacitorPlateController>().RemoveCharge(electron);

            PathFollower pathFollower = electron.GetComponent<PathFollower>();
            pathFollower.SetPath(minusCable.GetComponent<IPath>());
            pathFollower.maxSpeed = electronSpeed;
            pathFollower.reverseOrder = false;
            pathFollower.followPath = true;

            numberOfElectrons--;
            yield return new WaitForSecondsWithPause(electronTimeInterval);
        }
    }

    public void SetElectronOnPlate(GameObject electron, GameObject plate)
    {
        Charge proton = chargePoolHander.GetNewProton();

        if (plate == plate1)
        {
            plate1.GetComponent<CapacitorPlateController>().AddCharge(electron.GetComponent<Charge>());
            plate2.GetComponent<CapacitorPlateController>().AddCharge(proton);
        }
        else if(plate == plate2)
        {
            plate2.GetComponent<CapacitorPlateController>().AddCharge(electron.GetComponent<Charge>());
            plate1.GetComponent<CapacitorPlateController>().AddCharge(proton.GetComponent<Charge>());
        }
    }

    private void Charge()
    {
        voltage = previousPowerVoltage + (powerVoltage - previousPowerVoltage) * (1 - Mathf.Exp(-chargeTime / (seriesResistance * capacitance)));
    }

    private void Discharge()
    {
        voltage = powerVoltage + (previousPowerVoltage - powerVoltage) * Mathf.Exp(-chargeTime / (seriesResistance * capacitance));
    }

    public float GetElectricalFieldStrength()
    {
        float area = GetOverlapPlateArea();
        float permittivity = vacuumPermittivity * dielectric.GetRelativePermittivity();

        return (capacitance * voltage) / (area * permittivity);
    }

    public float GetVoltage()
    {
        return this.voltage;
    }

    public void getVoltageByReference(MessageArgs args)
    {
        args.value = this.voltage;
    }

    public float GetCapacitance()
    {
        return this.capacitance;
    }

    public void GetCapacitanceByReference(MessageArgs args)
    {
        args.value = this.capacitance;
    }

    public void SetPowerVoltage(float voltage)
    {
        this.powerVoltage = voltage;
    }
}
