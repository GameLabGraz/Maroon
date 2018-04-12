using UnityEngine;

public class CapacitorFieldLineManager : FieldLineManager
{
    [SerializeField]
    private Capacitor capacitor;

    [SerializeField]
    private GameObject fieldLinePrefab;

    [SerializeField]
    private int numOfFieldLines;

    [SerializeField]
    private Vector3 fieldLineAxis = Vector3.up;

    protected override void Start()
    {
        UpdateFieldLines();
    }

    public void SetNumOfFieldsLineOnPlate(int numOfFieldLines)
    {
        this.numOfFieldLines = numOfFieldLines;
        UpdateFieldLines();
    }

    private void UpdateFieldLines()
    {
        ClearFieldLines();

        GameObject capacitorPlate = capacitor.Plate1;
        for(int i = 0; i < numOfFieldLines; i++)
        {
            float plateSizeOnAxis = Vector3.Dot(capacitorPlate.transform.localScale, fieldLineAxis);

            GameObject fieldLine = GameObject.Instantiate(fieldLinePrefab, capacitorPlate.transform, false);

            Vector3 offset = fieldLineAxis * (-plateSizeOnAxis / 2 + (plateSizeOnAxis / (numOfFieldLines-1)) * i);
            fieldLine.transform.position = capacitorPlate.transform.position + offset;

            fieldLines.Add(fieldLine.GetComponent<FieldLine>());

        }
    }

    private void ClearFieldLines()
    {
        foreach (FieldLine fieldLine in fieldLines)
            DestroyImmediate(fieldLine.gameObject);
        fieldLines.Clear();
    }

}
