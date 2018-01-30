using System.Collections.Generic;
using UnityEngine;

public class ChargePoolHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject electronPrefab;

    [SerializeField]
    private GameObject protonPrefab;

    private Dictionary<int, Charge> electrons = new Dictionary<int, Charge>();

    private Dictionary<int, Charge> protons = new Dictionary<int, Charge>();

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
                int protonId = protons.Count;
                charge.Id = protonId;
                protons.Add(protonId, charge);
            }

            // negative charge => electron
            else
            {
                int electronId = electrons.Count;
                charge.Id = electronId;
                electrons.Add(electronId, charge);
            }
        }
    }

    public Charge GetProton(int id)
    {
        return protons[id];
    }

    public Charge GetElectron(int id)
    {
        return electrons[id];
    }

    public Charge GetNewProton()
    {
        GameObject protonObj = GameObject.Instantiate(protonPrefab);
        Charge proton = protonObj.GetComponent<Charge>();
        int protonId = protons.Count;

        proton.Id = protonId;
        protons.Add(protonId, proton);

        return proton;
    }

    public Charge GetNewElectron()
    {
        GameObject electronObj = GameObject.Instantiate(electronPrefab);
        Charge electron = electronObj.GetComponent<Charge>();
        int electronId = electrons.Count;

        electron.Id = electronId;
        electrons.Add(electronId, electron);

        return electron;
    }

    public bool RemoveCharge(Charge charge)
    {
        if (charge.ChargeValue > 0)
            return protons.Remove(charge.Id);
        else
            return electrons.Remove(charge.Id);

    }

    public void DestroyCharge(Charge charge)
    {
        RemoveCharge(charge);
        Destroy(charge.gameObject);
    }
}
