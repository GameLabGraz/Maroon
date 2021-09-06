using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ObjectsInUse;

public class initCoordSystem : MonoBehaviour, IResetObject
{
    private static initCoordSystem _instance;
    
    [SerializeField] private Transform _coordOrigin;
    [SerializeField] private GameObject _trajectory;
    [SerializeField] private GameObject _particle;
    [SerializeField] private GameObject _satellite;
    [SerializeField] private GameObject _soccerBall;
    [SerializeField] private GameObject _planet;

    [SerializeField] private TMP_Text _xLabel;
    [SerializeField] private TMP_Text _yLabel;
    [SerializeField] private TMP_Text _zLabel;
    [SerializeField] private TMP_Text _xLabel2;
    [SerializeField] private TMP_Text _yLabel2;
    [SerializeField] private TMP_Text _zLabel2;

    private string _text;
    private string _textX = "";
    private string _textY = "";
    private string _textZ = "";
    private string _textX2 = "";
    private string _textY2 = "";
    private string _textZ2 = "";

    private Vector3 _coordOriginPosition;
    private Vector3 _xMax;
    private Vector3 _yMax;
    private Vector3 _zMax;
    
    private bool _borderValuesSet = false;
    private Color _color = Color.black;

    private List<GameObject> _objects = new List<GameObject>();
    private List<GameObject> _origin = new List<GameObject>();

    private Vector3 _scaleXYZ = new Vector3(4.5f, 3.5f, 3.5f);

    [SerializeField] private Vector3 _realXYZMin = new Vector3(-30.0f, -30.0f, -30.0f);
    [SerializeField] private Vector3 _realXYZMax = new Vector3(30.0f, 30.0f, 30.0f);

    // Start is called before the first frame update
    void Start()
    {
        Vector3 scaleTrajectory = new Vector3(-0.05f, -0.05f, -0.05f);
        Vector3 scaleParticle = new Vector3(-0.09f, -0.09f, -0.09f);
        Vector3 scalePlanet = new Vector3(-0.02f, -0.02f, -0.02f);

        _trajectory.transform.localScale = scaleTrajectory;
        _particle.transform.localScale = scaleParticle;
        _satellite.transform.localScale = scaleParticle;
        _planet.transform.localScale = scalePlanet;

        DrawAxis();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawAxis()
    {
        Vector3 end;
        float origin_x = _coordOrigin.position.x;
        float origin_y = _coordOrigin.position.y;
        float origin_z = _coordOrigin.position.z;
        _coordOriginPosition = new Vector3(origin_x, origin_y, origin_z);

        end = new Vector3(origin_x + _scaleXYZ.x, origin_y, origin_z);
        _xMax = end;
        end = new Vector3(origin_x, origin_y + _scaleXYZ.y, origin_z);
        _yMax = end;
        end = new Vector3(origin_x, origin_y, origin_z + _scaleXYZ.z);
        _zMax = end;
    }

    void DrawHelperLine(Vector3 start, Vector3 end, float duration = 0.2f)
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

    void DrawLine(Vector3 start, Vector3 end, float duration = 0.2f)
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

    void DrawOrigin(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
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
        _origin.Add(myLine);
        lr.numCapVertices = 5;
    }

    public void SetLabelX(string text)
    {
        _xLabel.text = text;
    }

    public void SetLabelY(string text)
    {
        _yLabel.text = text;
    }

    public void SetLabelZ(string text)
    {
        _zLabel.text = text;
    }

    public void SetLabelOrigin(string text)
    {
        _xLabel2.text = text;
    }

    // set real coord-values 
    public void SetRealCoordBorders(Vector3 min, Vector3 max)
    {
        _realXYZMin = min;
        _realXYZMax = max;

        _textX = "X: " + RoundDisplayedValues(_realXYZMax.x) + "m";
        _textY = "Y: " + RoundDisplayedValues(_realXYZMax.z) + "m";
        _textZ = "Z: " + RoundDisplayedValues(_realXYZMax.y) + "m";

        _textX2 = "X: " + RoundDisplayedValues(_realXYZMax.x) + "m";
        _textY2 = "Y: " + RoundDisplayedValues(_realXYZMax.z) + "m";
        _textZ2 = "Z: " + RoundDisplayedValues(_realXYZMax.y) + "m";

        _borderValuesSet = true;
        DrawOriginGrid(true);
    }

    private string RoundDisplayedValues(float value)
    {
        if (System.Math.Round(value, 2) == 0.00f)
            return value.ToString();
        else
            return (System.Math.Round(value, 2)).ToString();
    }

    // map real coord-values to unity-coord values
    public Vector3 MapValues(Vector3 point)
    {
        Vector3 tmp = point;

        tmp.x = ScaleCoords(point.x, _realXYZMin.x, _realXYZMax.x, _coordOriginPosition.x, _xMax.x);
        tmp.y = ScaleCoords(point.y, _realXYZMin.y, _realXYZMax.y, _coordOriginPosition.y, _yMax.y);
        tmp.z = ScaleCoords(point.z, _realXYZMin.z, _realXYZMax.z, _coordOriginPosition.z, _zMax.z);
        
        return tmp;
    }

    public Vector3 MapValuesBack(Vector3 point)
    {
        Vector3 tmp = point;

        tmp.x = ScaleCoordsBack(point.x, _realXYZMin.x, _realXYZMax.x, _coordOriginPosition.x, _xMax.x);
        tmp.y = ScaleCoordsBack(point.y, _realXYZMin.y, _realXYZMax.y, _coordOriginPosition.y, _yMax.y);
        tmp.z = ScaleCoordsBack(point.z, _realXYZMin.z, _realXYZMax.z, _coordOriginPosition.z, _zMax.z);
        
        return tmp;
    }

    private float ScaleCoords(double value, double min, double max, double unityCoordA, double unityCoordB)
    {
        return (float)((((unityCoordB - unityCoordA) * (value - min)) / (max - min)) + unityCoordA);
    }

    private float ScaleCoordsBack(double value, double unityCoordA, double unityCoordB, double min, double max)
    {
        return (float)((((unityCoordB - unityCoordA) * (value - min)) / (max - min)) + unityCoordA);
    }


    public void DrawPoint(Vector3 point, bool drawTrajectory, ParticleObject particleInUse)
    {
        switch (particleInUse)
        {
            case ParticleObject.Default:
                _particle.transform.position = point;
                break;
            case ParticleObject.Ball:           
                _soccerBall.transform.position = point;
                break;
            case ParticleObject.Satellite:
                _satellite.transform.position = point;
                break;
            default:
                _particle.transform.position = point;
                break;
        }

        if (drawTrajectory)
        {
            _objects.Add(Instantiate(_trajectory, point, Quaternion.identity));
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
        // deleting game objects
        foreach (GameObject trajectory in _objects)
            Destroy(trajectory);

        // deleting origin lines
        foreach (GameObject line in _origin)
            Destroy(line);

        _objects.Clear();
        _origin.Clear();

        _particle.SetActive(false);
        _satellite.SetActive(false);
        _soccerBall.SetActive(false);
        _planet.SetActive(false);

        _textX = "";
        _textY = "";
        _textZ = "";
        _textX2 = "";
        _textY2 = "";
        _textZ2 = "";

        _borderValuesSet = false;
        _color = Color.black;
    }

    public void SetParticleActive(ParticleObject particleInUse)
    {
        switch (particleInUse)
        {
            case ParticleObject.Default:
                _particle.SetActive(true);
                break;
            case ParticleObject.Ball:
                _soccerBall.SetActive(true);
                break;
            case ParticleObject.Satellite:
                _satellite.SetActive(true);
                _planet.SetActive(true);
                break;
            default:
                _particle.SetActive(true);
                break;
        }
    }

    // UI toggle
    public void ShowLabels(bool show)
    {
        if (show)
        {
            SetLabelX(_textX);
            SetLabelY(_textY);
            SetLabelZ(_textZ);
            _xLabel2.text = _textX2;
            _yLabel2.text = _textY2;
            _zLabel2.text = _textZ2;
        }
        else
        {
            SetLabelX("");
            SetLabelY("");
            SetLabelZ("");
            _xLabel2.text = "";
            _yLabel2.text = "";
            _zLabel2.text = "";
        }
    }

    public void DrawOriginGrid(bool draw)
    {
        if (draw && _borderValuesSet)
        {
            //Debug.Log("Draw Grid");
            Vector3 start;
            Vector3 end;

            // x line
            start = new Vector3(_realXYZMin.x, 0, 0);
            end = new Vector3(_realXYZMax.x, 0, 0);
            DrawOrigin(MapValues(start), MapValues(end), _color);
            //end = new Vector3(real_xyz_max.x + 0.5f, 0, 0);
            _xLabel.transform.position = MapValues(end);
            _xLabel2.transform.position = MapValues(start);

            // y line
            start = new Vector3(0, 0, _realXYZMin.z);
            end = new Vector3(0, 0, _realXYZMax.z);
            DrawOrigin(MapValues(start), MapValues(end), _color);
            _yLabel.transform.position = MapValues(end);
            _yLabel2.transform.position = MapValues(start);

            // z line
            start = new Vector3(0, _realXYZMin.y, 0);
            end = new Vector3(0, _realXYZMax.y, 0);
            DrawOrigin(MapValues(start), MapValues(end), _color);
            _zLabel.transform.position = MapValues(end);
            _zLabel2.transform.position = MapValues(start);
 
            _planet.transform.position = new Vector3(-0.2f, 1.75f, 6.4f);
        }
        else
        {
            // deleting origin lines
            foreach (GameObject line in _origin)
                Destroy(line);
            _origin.Clear();
        }
    }

    public void SetColor(Color col)
    {
        _color = col;
    }
}
