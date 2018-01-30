using System.Collections.Generic;
using UnityEngine;

public class ChargePoolHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject electronPrefab;

    [SerializeField]
    private GameObject protonPrefab;

    private ulong electronIDcount = 0;
    private Dictionary<ulong, Charge> electrons = new Dictionary<ulong, Charge>();

    private ulong protonIDcount = 0;
    private Dictionary<ulong, Charge> protons = new Dictionary<ulong, Charge>();

    private void Start()
    {
        // Find charges in scene
        GameObject[] charges = GameObject.FindGameObjectsWithTag("Charge");

        foreach(GameObject chargeObj in charges)
        {
            Charge charge = chargeObj.GetComponent<Charge>();
            if (charge == null)
                continue;

            // positive charge => proton
            if (charge.ChargeValue > 0) 
            {
                charge.Id = protonIDcount;
                protons.Add(protonIDcount++, charge);
            }

            // negative charge => electron
            else
            {
                charge.Id = electronIDcount;
                electrons.Add(electronIDcount++, charge);
            }

            charge.ChargePoolHandler = this;
        }
    }

    public Charge GetProton(ulong id)
    {
        return protons[id];
    }

    public Charge GetElectron(ulong id)
    {
        return electrons[id];
    }

    public Charge GetNewProton()
    {
        GameObject protonObj = GameObject.Instantiate(protonPrefab);
        Charge proton = protonObj.GetComponent<Charge>();
        proton.Id = protonIDcount;
        proton.ChargePoolHandler = this;

        protons.Add(protonIDcount++, proton);
        return proton;
    }

    public Charge GetNewElectron()
    {
        GameObject electronObj = GameObject.Instantiate(electronPrefab);
        Charge electron = electronObj.GetComponent<Charge>();
        electron.Id = electronIDcount;
        electron.ChargePoolHandler = this;

        electrons.Add(electronIDcount++, electron);
        return electron;
    }

    public bool RemoveCharge(Charge charge)
    {
        if (charge.ChargeValue > 0)
            return protons.Remove(charge.Id);
        else
            return electrons.Remove(charge.Id);
    }
}
