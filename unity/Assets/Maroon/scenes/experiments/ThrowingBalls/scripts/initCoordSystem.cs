using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ObjectsInUse;

public class initCoordSystem : MonoBehaviour, IResetObject
{
    private static initCoordSystem _instance;
    

    [SerializeField] private Transform coordOrigin;
    [SerializeField] private GameObject trajectory;
    [SerializeField] private GameObject particle;
    [SerializeField] private GameObject satellite;
    [SerializeField] private GameObject soccer_ball;
    [SerializeField] private GameObject planet_;

    [SerializeField] private TMP_Text xLabel;
    [SerializeField] private TMP_Text yLabel;
    [SerializeField] private TMP_Text zLabel;
    [SerializeField] private TMP_Text xLabel2;
    [SerializeField] private TMP_Text yLabel2;
    [SerializeField] private TMP_Text zLabel2;

    private string text;
    private string text_x_ = "";
    private string text_y_ = "";
    private string text_z_ = "";
    private string text_x2_ = "";
    private string text_y2_ = "";
    private string text_z2_ = "";

    private Vector3 coordOriginPosition;
    private Vector3 xMax;
    private Vector3 yMax;
    private Vector3 zMax;
    //
    private bool border_values_set_ = false;
    Color color = Color.black;

    private List<GameObject> objects_ = new List<GameObject>();
    private List<GameObject> origin_ = new List<GameObject>();

    private Vector3 scale_xyz = new Vector3(4.5f, 3.5f, 3.5f);
    //[SerializeField] private Vector3 scale_xyz = new Vector3(3.5f, 2.5f, 2.5f);

    [SerializeField] private Vector3 real_xyz_min = new Vector3(-30.0f, -30.0f, -30.0f);
    [SerializeField] private Vector3 real_xyz_max = new Vector3(30.0f, 30.0f, 30.0f);

    // Start is called before the first frame update
    void Start()
    {
        Vector3 scaleTrajectory = new Vector3(-0.05f, -0.05f, -0.05f);
        Vector3 scaleParticle = new Vector3(-0.09f, -0.09f, -0.09f);
        Vector3 scalePlanet = new Vector3(-0.02f, -0.02f, -0.02f);

        trajectory.transform.localScale = scaleTrajectory;
        particle.transform.localScale = scaleParticle;
        satellite.transform.localScale = scaleParticle;
        planet_.transform.localScale = scalePlanet;

        drawAxis(false); // set this to true if u want to display the coord-box
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void drawAxis(bool show_box)
    {
        Vector3 end;
        float origin_x = coordOrigin.position.x;
        float origin_y = coordOrigin.position.y;
        float origin_z = coordOrigin.position.z;
        coordOriginPosition = new Vector3(origin_x, origin_y, origin_z);

        end = new Vector3(origin_x + scale_xyz.x, origin_y, origin_z);
        xMax = end;
        end = new Vector3(origin_x, origin_y + scale_xyz.y, origin_z);
        yMax = end;
        end = new Vector3(origin_x, origin_y, origin_z + scale_xyz.z);
        zMax = end;

        if (show_box)
        {
            // draw x-axis
            drawLine(coordOriginPosition, xMax);

            // draw y-axis
            drawLine(coordOriginPosition, yMax);

            // draw x-axis
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

    void drawOrigin(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Specular"));
        lr.material.color = color;
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
        xLabel2.text = text;
    }

    // set real coord-values 
    public void setRealCoordBorders(Vector3 min, Vector3 max)
    {
        real_xyz_min = min;
        real_xyz_max = max;

        text_x_ = "X: " + roundDisplayedValues(real_xyz_max.x) + "m";
        //setLabelX(text_x_);

        text_y_ = "Y: " + roundDisplayedValues(real_xyz_max.z) + "m";
        //setLabelY(text_y_);

        text_z_ = "Z: " + roundDisplayedValues(real_xyz_max.y) + "m";
        //setLabelZ(text_z_);

        text_x2_ = "X: " + roundDisplayedValues(real_xyz_max.x) + "m";
        text_y2_ = "Y: " + roundDisplayedValues(real_xyz_max.z) + "m";
        text_z2_ = "Z: " + roundDisplayedValues(real_xyz_max.y) + "m";

        //xLabel2.text = text_x2_;
        //yLabel2.text = text_y2_;
        //zLabel2.text = text_z2_;

        border_values_set_ = true;
        drawOriginGrid(true);
    }

    private string roundDisplayedValues(float value)
    {
        if (System.Math.Round(value, 2) == 0.00f)
            return value.ToString();
        else
            return (System.Math.Round(value, 2)).ToString();
    }

    // map real coord-values to unity-coord values
    public Vector3 mapValues(Vector3 point)
    {
        Vector3 tmp = point;

        tmp.x = scaleCoords(point.x, real_xyz_min.x, real_xyz_max.x, coordOriginPosition.x, xMax.x);
        tmp.y = scaleCoords(point.y, real_xyz_min.y, real_xyz_max.y, coordOriginPosition.y, yMax.y);
        tmp.z = scaleCoords(point.z, real_xyz_min.z, real_xyz_max.z, coordOriginPosition.z, zMax.z);
        
        return tmp;
    }

    public Vector3 mapValuesBack(Vector3 point)
    {
        Vector3 tmp = point;

        tmp.x = scaleCoordsBack(point.x, real_xyz_min.x, real_xyz_max.x, coordOriginPosition.x, xMax.x);
        tmp.y = scaleCoordsBack(point.y, real_xyz_min.y, real_xyz_max.y, coordOriginPosition.y, yMax.y);
        tmp.z = scaleCoordsBack(point.z, real_xyz_min.z, real_xyz_max.z, coordOriginPosition.z, zMax.z);
        
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


    public void drawPoint(Vector3 point, bool draw_trajectory, ParticleObject particle_in_use_)
    {
        switch (particle_in_use_)
        {
            case ParticleObject.Default:
                particle.transform.position = point;
                break;
            case ParticleObject.Ball:           
                soccer_ball.transform.position = point;
                break;
            case ParticleObject.Satellite:
                satellite.transform.position = point;
                break;
            default:
                particle.transform.position = point;
                break;
        }

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
        //Debug.Log("Reset Coord System\n");

        // deleting game objects
        foreach (GameObject trajectory in objects_)
            Destroy(trajectory);

        // deleting origin lines
        foreach (GameObject line in origin_)
            Destroy(line);

        objects_.Clear();
        origin_.Clear();

        particle.SetActive(false);
        satellite.SetActive(false);
        soccer_ball.SetActive(false);
        planet_.SetActive(false);

        text_x_ = "";
        text_y_ = "";
        text_z_ = "";
        text_x2_ = "";
        text_y2_ = "";
        text_z2_ = "";
        /*
        setLabelX(text_x_);
        setLabelY(text_y_);
        setLabelZ(text_z_);
        xLabel2.text = "";
        yLabel2.text = "";
        zLabel2.text = "";
        */

        border_values_set_ = false;

        color = Color.black;
    }

    public void setParticleActive(ParticleObject particle_in_use_)
    {
        switch (particle_in_use_)
        {
            case ParticleObject.Default:
                particle.SetActive(true);
                break;
            case ParticleObject.Ball:
                soccer_ball.SetActive(true);
                break;
            case ParticleObject.Satellite:
                satellite.SetActive(true);
                planet_.SetActive(true);
                break;
            default:
                particle.SetActive(true);
                break;
        }
    }

    // UI toggle
    public void showLabels(bool show)
    {
        if (show)
        {
            setLabelX(text_x_);
            setLabelY(text_y_);
            setLabelZ(text_z_);
            xLabel2.text = text_x2_;
            yLabel2.text = text_y2_;
            zLabel2.text = text_z2_;
        }
        else
        {
            setLabelX("");
            setLabelY("");
            setLabelZ("");
            xLabel2.text = "";
            yLabel2.text = "";
            zLabel2.text = "";
        }
    }

    public void drawOriginGrid(bool draw)
    {
        if (draw && border_values_set_)
        {
            //Debug.Log("Draw Grid");
            Vector3 start;
            Vector3 end;

            // x line
            start = new Vector3(real_xyz_min.x, 0, 0);
            end = new Vector3(real_xyz_max.x, 0, 0);
            drawOrigin(mapValues(start), mapValues(end), color);
            //end = new Vector3(real_xyz_max.x + 0.5f, 0, 0);
            xLabel.transform.position = mapValues(end);
            xLabel2.transform.position = mapValues(start);

            // y line
            start = new Vector3(0, 0, real_xyz_min.z);
            end = new Vector3(0, 0, real_xyz_max.z);
            drawOrigin(mapValues(start), mapValues(end), color);
            yLabel.transform.position = mapValues(end);
            yLabel2.transform.position = mapValues(start);

            // z line
            start = new Vector3(0, real_xyz_min.y, 0);
            end = new Vector3(0, real_xyz_max.y, 0);
            drawOrigin(mapValues(start), mapValues(end), color);
            zLabel.transform.position = mapValues(end);
            zLabel2.transform.position = mapValues(start);
 
            planet_.transform.position = new Vector3(-0.2f, 1.75f, 6.4f);
        }
        else
        {
            //Debug.Log("Clear Grid");
            // deleting origin lines
            foreach (GameObject line in origin_)
                Destroy(line);
            origin_.Clear();
        }
    }

    public void setColor(Color col)
    {
        color = col;
    }
}
