using Maroon.Physics.Electromagnetism;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject minBoundary;
    [SerializeField] private GameObject maxBoundary;

    [SerializeField] private EField eField;

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
        
        obj.GetComponent<Maroon.Physics.Electromagnetism.Charge>().strength = chargeStrength;

        eField.addProducerToSet(obj);
        eField.updateProducers();

        ChangeColorOfParticle(obj, chargeStrength);

        return obj;
    }

    public void RemoveSourceFromEField(GameObject source)
    {
        eField.removeProducerFromSet(source);
    }

    public void ChangeColorOfParticle(GameObject obj, float chargeStrength)
    {
        var particleBase = obj.transform.Find("Base").GetComponent<MeshRenderer>();
        var mat = particleBase.materials;

        if (chargeStrength < 0)
        {
            mat[0].color = Color.blue;
            mat[2].color = mat[0].color;
            mat[1].color = Color.white;
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
}
