using UnityEngine;

public class VoltageSource : MonoBehaviour, IVoltagePoleTrigger
{
    [SerializeField]
    private GameObject negativeVoltagePole;

    [SerializeField]
    private GameObject positiveVoltagePole;

    [SerializeField]
    private GameObject electronPrefab;

    private GameObject plusCable;

    private GameObject minusCable;

    private void Start()
    {
        plusCable = GameObject.Find("Cable+");
        minusCable = GameObject.Find("Cable-");
    }

    public void PullVoltagePoleTrigger(Collider other, GameObject source)
    {
        float electronSpeed = other.GetComponent<PathFollower>().maxSpeed;
        Destroy(other.gameObject);

        GameObject electron = GameObject.Instantiate(electronPrefab);      
        electron.GetComponent<Charge>().JustCreated = false;

        PathFollower pathFollower = electron.GetComponent<PathFollower>();
        pathFollower.maxSpeed = electronSpeed;

        if (source == positiveVoltagePole)
        {
            electron.transform.position = negativeVoltagePole.transform.position;

            pathFollower.reverseOrder = true;
            pathFollower.SetPath(minusCable.GetComponent<IPath>());
        }

        if(source == negativeVoltagePole)
        {
            electron.transform.position = positiveVoltagePole.transform.position;

            pathFollower.SetPath(plusCable.GetComponent<IPath>());
        }
    }
}
