using System.Collections;
using UnityEngine;

public class Charge : MonoBehaviour, IGenerateE
{
    [SerializeField]
    private ulong id;

    [SerializeField]
    private float chargeValue;

    [SerializeField]
    private float mass;

    [SerializeField]
    private bool justCreated = true;

    EField eField;

    [SerializeField]
    private float forceFactor = 0.5f; //constant force multyplier

    private ChargePoolHandler chargePoolHandler;

    private void Start()
    {
        eField = GameObject.FindGameObjectWithTag("Field").GetComponent<EField>();
    }

    public ulong Id
    {
        get { return id; }
        set { id = value; }
    }

    public ChargePoolHandler ChargePoolHandler
    {
        get { return chargePoolHandler; }
        set { chargePoolHandler = value; }
    }

    public float ChargeValue
    {
        get { return chargeValue; }
    }

    public float Mass
    {
        get { return mass; }
    }

    public bool JustCreated
    {
        get { return justCreated; }
        set { justCreated = value; }
    }

    public void FadingOut(float fadingOutTime)
    {
        StartCoroutine("FadeOutSequence", fadingOutTime);
    }

    private IEnumerator FadeOutSequence(float fadingOutTime)
    {
        float fadingOutSpeed = 1.0f / fadingOutTime;

        Renderer rendererObj = GetComponent<Renderer>();
        float alphaValue = rendererObj.material.color.a;

        while (alphaValue >= 0)
        {
            alphaValue -= Time.deltaTime * fadingOutSpeed;
            Color newColor = rendererObj.material.color;
            newColor.a = alphaValue;
            rendererObj.material.SetColor("_Color", newColor);
            yield return null;
        }

        Destroy(this.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("ElectricField"))
            return;

        Vector3 force = chargeValue * forceFactor * eField.get(transform.position, gameObject);
        GetComponent<Rigidbody>().AddForce(force);
    }

    private void OnDestroy()
    {
        if (chargePoolHandler != null)
            chargePoolHandler.RemoveCharge(this);
    }

    public Vector3 getE(Vector3 position)
    {
        Vector3 direction = position - this.transform.position;
        float distance = Vector3.Distance(this.transform.position, position);

        return (chargeValue * direction) / (4 * Mathf.PI * 8.8542e-12f * Mathf.Pow(distance, 3));
    }

    public float getEFlux(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    public float getEPotential(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    public float getFieldStrength()
    {
        throw new System.NotImplementedException();
    }
}
