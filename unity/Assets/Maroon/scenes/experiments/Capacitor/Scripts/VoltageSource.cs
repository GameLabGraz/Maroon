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
        GameObject electron = other.gameObject;
        electron.GetComponent<Charge>().JustCreated = true;

        PathFollower pathFollower = electron.GetComponent<PathFollower>();

        if (source == positiveVoltagePole)
        {
            electron.transform.position = negativeVoltagePole.transform.position;

            pathFollower.SetPath(minusCable.GetComponent<IPath>());
            pathFollower.reverseOrder = true;
            pathFollower.followPath = true;
        }

        if(source == negativeVoltagePole)
        {
            electron.transform.position = positiveVoltagePole.transform.position;

            pathFollower.SetPath(plusCable.GetComponent<IPath>());
            pathFollower.reverseOrder = true;          
            pathFollower.followPath = true;
        }
    }
}
