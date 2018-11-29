using UnityEngine;

public class DielectricSizeAdjuster : MonoBehaviour
{
    [SerializeField]
    private GameObject plate1;

    [SerializeField]
    private GameObject plate2;

    [SerializeField]
    private Vector3 plateOffset = Vector3.zero;


    void Update()
    {
        Vector3 dielectricSize = this.transform.localScale;

        Vector3 plate1Size = plate1.transform.localScale;
        Vector3 plate2Size = plate2.transform.localScale;

        if (plate1Size.x < plate2Size.x)
            dielectricSize.x = plate1Size.x;
        else
            dielectricSize.x = plate2Size.x;

        if (plate1Size.y < plate2Size.y)
            dielectricSize.y = plate1Size.y;
        else
            dielectricSize.y = plate2Size.y;

        dielectricSize.z = Vector3.Distance(plate1.transform.position, plate2.transform.position);
        dielectricSize.z -= plate1.transform.localScale.z;

        dielectricSize -= plateOffset;
        if (dielectricSize.x < 0)
            dielectricSize.x = 0;
        if (dielectricSize.y < 0)
            dielectricSize.y = 0;
        if (dielectricSize.z < 0)
            dielectricSize.z = 0;

        this.transform.localScale = dielectricSize;
        this.transform.position = Vector3.Lerp(plate1.transform.position, plate2.transform.position, 0.5f);
    }
}
