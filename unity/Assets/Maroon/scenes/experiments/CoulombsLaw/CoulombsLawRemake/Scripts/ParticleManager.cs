using System.Collections.Generic;
using Maroon.Physics.Electromagnetism;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject minBoundary;
    [SerializeField] private GameObject maxBoundary;

    [SerializeField] private EField eField;
    [SerializeField] private int maxChargeCount = 10;
    [SerializeField] private bool allowVisualization = true;

    public int GetMaxChargesCount() => maxChargeCount;
    public bool IsVisualizationAllowed() => allowVisualization;
   
    private List<Maroon.Physics.Electromagnetism.Charge> _charges = new List<Maroon.Physics.Electromagnetism.Charge>();

    public GameObject CreateSource(GameObject prefab, Vector3 position, float chargeStrength)
    {
        var obj = Instantiate(prefab, position, Quaternion.identity);
        Debug.Assert(obj != null);

        var movement = obj.GetComponent<PC_DragHandler>();
        if (!movement) movement = obj.GetComponentInChildren<PC_DragHandler>();
        if (movement)
        {
            movement.SetBoundaries(minBoundary, maxBoundary);
        }

        var charge = obj.GetComponent<Maroon.Physics.Electromagnetism.Charge>();
        charge.strength = chargeStrength;

        _charges.Add(charge);

        eField.addProducerToSet(obj);
        eField.updateProducers();

        ChangeColorOfParticle(obj, chargeStrength);

        return obj;
    }

    public void RemoveSourceFromEField(GameObject source)
    {
        eField.removeProducerFromSet(source);
        _charges.Remove(source.GetComponent<Maroon.Physics.Electromagnetism.Charge>());
    }

    public void RemoveAllSourcesFromEField()
    {
        foreach (var charge in _charges)
        {
            eField.removeProducerFromSet(charge.gameObject);
            Destroy(charge.gameObject);
        }

        _charges.Clear();
    }

    public void ChangeColorOfParticle(GameObject obj, float chargeStrength)
    {
        var particleBase = obj.transform.Find("Base").GetComponent<MeshRenderer>();
        var mat = particleBase.materials;

        if (chargeStrength < 0)
        {
            mat[0].color = Color.blue;
            mat[1].color = Color.white;
            mat[2].color = mat[0].color;
        }
        else if (chargeStrength > 0)
        {
            mat[0].color = Color.red;
            mat[1].color = mat[2].color = Color.white;
        }
        else
        {
            mat[0].color = Color.green;
            mat[0].color = mat[1].color = mat[2].color;
        }

        particleBase.materials = mat;
    }

    public List<Vector4> GetChargesAsVector()
    {
        var list = new List<Vector4>();
        foreach (var charge in _charges)
        {
            var pos = charge.transform.position;
            list.Add(new Vector4(pos.x, pos.y, pos.z, charge.Strength));
        }

        return list;
    }

}
