using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class initCoordSystem : MonoBehaviour, IResetObject
{
    private static initCoordSystem _instance;

    [SerializeField] private Transform coordOrigin;
    [SerializeField] private GameObject trajectory;
    [SerializeField] private GameObject particle;

    [SerializeField] private TMP_Text xLabel;
    [SerializeField] private TMP_Text yLabel;
    [SerializeField] private TMP_Text zLabel;
    [SerializeField] private TMP_Text originLabel;

    private string text;

    private Vector3 coordOriginPosition;
    private Vector3 xMax;
    private Vector3 yMax;
    private Vector3 zMax;

    private List<GameObject> objects_ = new List<GameObject>();
    private List<GameObject> origin_ = new List<GameObject>();

    [SerializeField] private Vector3 scale_xyz = new Vector3(3.5f, 2.5f, 2.5f);

    [SerializeField] private Vector3 real_xyz_min = new Vector3(-30.0f, -30.0f, -30.0f);
    [SerializeField] private Vector3 real_xyz_max = new Vector3(30.0f, 30.0f, 30.0f);

    // Start is called before the first frame update
    void Start()
    {
        Vector3 scaleTrajectory = new Vector3(-0.05f, -0.05f, -0.05f);
        Vector3 scaleParticle = new Vector3(-0.09f, -0.09f, -0.09f);

        trajectory.transform.localScale = scaleTrajectory;
        particle.transform.localScale = scaleParticle;

        drawAxis();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //IEnumerator drawAxis()
    void drawAxis()
    {
        Vector3 end;
        float origin_x = coordOrigin.position.x;
        float origin_y = coordOrigin.position.y;
        float origin_z = coordOrigin.position.z;
        coordOriginPosition = new Vector3(origin_x, origin_y, origin_z);

        // yield return new WaitForSeconds(1);

        // draw x-axis
        end = new Vector3(origin_x + scale_xyz.x, origin_y, origin_z);
        xMax = end;
        drawLine(coordOriginPosition, xMax);

        // draw y-axis
        end = new Vector3(origin_x, origin_y + scale_xyz.y, origin_z);
        yMax = end;
        drawLine(coordOriginPosition, yMax);

        // draw x-axis
        end = new Vector3(origin_x, origin_y, origin_z + scale_xyz.z);
        zMax = end;
        drawLine(coordOriginPosition, zMax);

        // draw helper lines
        Vector3 tmp;
        Vector3 tmp2;

        // x line
        tmp = new Vector3(xMax.x, xMax.y, zMax.z);
        drawHelperLine(zMax, tmp);
        // 
        tmp = new Vector3(xMax.x, yMax.y, xMax.z);
        drawHelperLine(yMax, tmp);
        //
        tmp = new Vector3(xMax.x, yMax.y, zMax.z);
        tmp2 = new Vector3(zMax.x, yMax.y, zMax.z);
        drawHelperLine(tmp, tmp2);


        // y lines
        tmp = new Vector3(zMax.x, yMax.y, zMax.z);
        drawHelperLine(zMax, tmp);
        // 
        tmp = new Vector3(xMax.x, origin_y, zMax.z);
        tmp2 = new Vector3(xMax.x, yMax.y, zMax.z);
        drawHelperLine(tmp, tmp2);
        //
        tmp = new Vector3(xMax.x, yMax.y, xMax.z);
        drawHelperLine(xMax, tmp);

        // z line
        tmp = new Vector3(xMax.x, xMax.y, zMax.z);
        drawHelperLine(xMax, tmp);
        //
        tmp = new Vector3(xMax.x, yMax.y, xMax.z);
        tmp2 = new Vector3(xMax.x, yMax.y, zMax.z);
        drawHelperLine(tmp, tmp2);
        //
        tmp = new Vector3(origin_x, yMax.y, zMax.z);
        drawHelperLine(yMax, tmp);


    }

    void drawHelperLine(Vector3 start, Vector3 end, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Specular"));
        lr.material.color = Color.blue;
        lr.SetWidth(0.04f, 0.04f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.numCapVertices = 5;
    }

    void drawLine(Vector3 start, Vector3 end, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Specular"));
        lr.material.color = Color.black;
        lr.SetWidth(0.04f, 0.04f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.numCapVertices = 5;

    }

    void drawOrigin(Vector3 start, Vector3 end, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Specular"));
        lr.material.color = Color.black;
        lr.SetWidth(0.02f, 0.02f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        origin_.Add(myLine);
        lr.numCapVertices = 5;
    }

    public void setLabelX(string text)
    {
        xLabel.text = text;
    }

    public void setLabelY(string text)
    {
        yLabel.text = text;
    }

    public void setLabelZ(string text)
    {
        zLabel.text = text;
    }

    public void setLabelOrigin(string text)
    {
        originLabel.text = text;
    }

    public void setScaleXYZ(Vector3 scale)
    {
        scale_xyz = scale;
    }

    // set real coord-values 
    public void setRealCoordBorders(Vector3 min, Vector3 max)
    {
        real_xyz_min = min;
        real_xyz_max = max;
        
        setLabelX("X " + (real_xyz_max.x).ToString() + "m");
        setLabelY("Y " + (real_xyz_max.z).ToString() + "m");
        setLabelZ("Z " + (real_xyz_max.y).ToString() + "m");
        setLabelOrigin("X: " + (real_xyz_min.x).ToString() + " Y: " + (real_xyz_min.z).ToString() + " Z: " + (real_xyz_min.y).ToString());

        Vector3 tmp = mapValues(real_xyz_min);
        Vector3 tmp2 = mapValues(real_xyz_max);
        Vector3 tmp3;
        Vector3 tmp4;

        // x line
        tmp3 = new Vector3(tmp.x, (tmp2.y + tmp.y) / 2, (tmp2.z + tmp.z) / 2);
        tmp4 = new Vector3(tmp2.x, (tmp2.y + tmp.y) / 2, (tmp2.z + tmp.z) / 2);
        drawOrigin(tmp3, tmp4);

        // y line
        tmp3 = new Vector3((tmp2.x + tmp.x) / 2, tmp.y, (tmp2.z + tmp.z) / 2);
        tmp4 = new Vector3((tmp2.x + tmp.x) / 2, tmp2.y, (tmp2.z + tmp.z) / 2);
        drawOrigin(tmp3, tmp4);

        // z line
        // y line
        tmp3 = new Vector3((tmp2.x + tmp.x) / 2, (tmp2.y + tmp.y) / 2, tmp.z);
        tmp4 = new Vector3((tmp2.x + tmp.x) / 2, (tmp2.y + tmp.y) / 2, tmp2.z);
        drawOrigin(tmp3, tmp4);

    }

    // map real coord-values to unity-coord values
    public Vector3 mapValues(Vector3 point)
    {
        Vector3 tmp = point;

        tmp.x = scaleCoords(point.x, real_xyz_min.x, real_xyz_max.x, coordOriginPosition.x, xMax.x);
        tmp.y = scaleCoords(point.y, real_xyz_min.y, real_xyz_max.y, coordOriginPosition.y, yMax.y);
        tmp.z = scaleCoords(point.z, real_xyz_min.z, real_xyz_max.z, coordOriginPosition.z, zMax.z);
        /*
        Debug.Log("DEBUG MAP VALUES\nXold = " + (point.x).ToString() + " -> Xnew = " + (tmp.x).ToString() + "\n" +
            "Yold = " + (point.y).ToString() + "->Ynew = " + (tmp.y).ToString() + "\n" +
            "Zold = " + (point.z).ToString() + "->Znew = " + (tmp.z).ToString());
        */
        return tmp;
    }

    public Vector3 mapValuesBack(Vector3 point)
    {
        Vector3 tmp = point;

        tmp.x = scaleCoordsBack(point.x, real_xyz_min.x, real_xyz_max.x, coordOriginPosition.x, xMax.x);
        tmp.y = scaleCoordsBack(point.y, real_xyz_min.y, real_xyz_max.y, coordOriginPosition.y, yMax.y);
        tmp.z = scaleCoordsBack(point.z, real_xyz_min.z, real_xyz_max.z, coordOriginPosition.z, zMax.z);
        /*
        Debug.Log("DEBUG MAP VALUES BACK\nXold = " + (point.x).ToString() + " -> Xnew = " + (tmp.x).ToString() + "\n" +
            "Yold = " + (point.y).ToString() + "->Ynew = " + (tmp.y).ToString() + "\n" +
            "Zold = " + (point.z).ToString() + "->Znew = " + (tmp.z).ToString());
        */
        return tmp;
    }

    private float scaleCoords(double value, double min, double max, double unity_coord_a, double unity_coord_b)
    {
        return (float)((((unity_coord_b - unity_coord_a) * (value - min)) / (max - min)) + unity_coord_a);
    }

    private float scaleCoordsBack(double value, double unity_coord_a, double unity_coord_b, double min, double max)
    {
        return (float)((((unity_coord_b - unity_coord_a) * (value - min)) / (max - min)) + unity_coord_a);
    }


    public void drawPoint(Vector3 point, bool draw_trajectory)
    {
        particle.transform.position = point;

        if (draw_trajectory)
        {
            objects_.Add(Instantiate(trajectory, point, Quaternion.identity));
        }
        
    }

    public static initCoordSystem Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<initCoordSystem>();
            return _instance;
        }
    }
   
    /// <summary>
    /// Resets the object
    /// </summary>
    public void ResetObject()
    {
        Debug.Log("Coord System\n");
        // deleting game objects
        foreach (GameObject trajectory in objects_)
            Destroy(trajectory);

        // deleting origin lines
        foreach (GameObject line in origin_)
            Destroy(line);

        objects_.Clear();
        origin_.Clear();

        particle.SetActive(false);

        setLabelX("X ");
        setLabelY("Y ");
        setLabelZ("Z ");
        setLabelOrigin("X: Y: Z: ");
    }

    public void setParticleActive()
    {
        particle.SetActive(true);
    }
}
