using System.Collections.Generic;
using PlatformControls.PC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class UIItemDragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("General Settings")]
    public Canvas parentCanvas;
    public float returnTime = 1f;
    public bool defaultNegativeCharge = true;

    public float maxValue;
    public float minValue;
    
    public Color maxColor;
    public Color minColor;
    public Color zeroColor;

    [Header("Image Components")] 
    public Image BackgroundImage;
    public Image MinusImage;
    public Image PlusImage;

    [Header("Particle Movement Restrictions")]
    public GameObject minPosition;
    public GameObject maxPosition;

    private Vector3 _initialPosition;
    private Vector3 _initialMousePosition;

    private bool _slowlyReturnToOrigin = false;
    private Vector3 _returnDirection;
    private float _time;
    
    private CoulombLogic _coulombLogic;

    private float _currentCharge = 0f;
    private bool _fixedPosition = false;

    private void Start()
    {
        GameObject simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
        
        GameObject charge = GameObject.Find(defaultNegativeCharge?"ElectronSlider":"ProtonSlider");
        if (charge)        
            SetCharge(charge.GetComponent<PC_Slider>().value * (defaultNegativeCharge ? -1f : 1f));
    }

    private void Update()
    {
        if (_slowlyReturnToOrigin)
        {
            transform.position += Time.deltaTime * _returnDirection / returnTime;
            _time += Time.deltaTime;

            if (_time >= returnTime)
            {
                transform.position = _initialPosition;
                _slowlyReturnToOrigin = false;
            }
        }
    }
    public void SetElectronCharge(float charge)
    {
        SetCharge(-charge);
    }

    public void SetProtonCharge(float charge)
    {
        SetCharge(charge);
    }
    
    private void SetCharge(float charge)
    {
        _currentCharge = charge;

        if (Mathf.Abs(charge) < 0.0001f)
        {
            BackgroundImage.color = zeroColor;
            MinusImage.gameObject.SetActive(false);
            PlusImage.gameObject.SetActive(false);
            return;
        }
        
        
        MinusImage.gameObject.SetActive(charge < 0);
        PlusImage.gameObject.SetActive(charge > 0);
        var newCol = Color.Lerp(minColor, maxColor, (Mathf.Abs(charge) - minValue) / (maxValue - minValue));
        
//        Debug.Log("(" + minColor + " - " + maxColor + ") -> " + newCol);
        BackgroundImage.color = newCol;
    }
    
    public void SetFixedPosition(bool isPositionFixed)
    {
        _fixedPosition = isPositionFixed;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _initialPosition = transform.position;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        var screenPoint = Input.mousePosition;
        var finish = parentCanvas.worldCamera.ScreenToWorldPoint(screenPoint);
        finish.z = 0f;
        transform.position = finish;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, 100f);

//        if (EventSystem.current.IsPointerOverGameObject())
//        {
//            Debug.Log("Pointer over game object");
//            _slowlyReturnToOrigin = true;
//            _time = 0f;
//            _returnDirection = _initialPosition - transform.position;
//            return;
//        }
        
        bool hitVectorField = false;
        for(var i = 0; i < hits.Length; ++i)
        {
            if (hits[i].transform.CompareTag("VectorField"))
            {
                hitVectorField = true;
                _coulombLogic.AddParticle(CreateParticle(hits[i].point));
                transform.position = _initialPosition;           
            }
        }

        if (!hitVectorField)
        {
            _slowlyReturnToOrigin = true;
            _time = 0f;
            _returnDirection = _initialPosition - transform.position;
        }
    }

    private ParticleBehaviour CreateParticle(Vector3 position)
    {
        var newGameObj = Instantiate(Resources.Load("Particle", typeof(GameObject))) as GameObject;
        Debug.Assert(newGameObj != null);
        
        var particle = newGameObj.GetComponent<ParticleBehaviour>();
        Debug.Assert(particle != null);
        particle.SetPosition(position);
        particle.SetFixedPosition(_fixedPosition);
        particle.SetCharge(_currentCharge, BackgroundImage.color);

        var movement = newGameObj.GetComponent<DragParticleHandler>();
        Debug.Assert(movement != null);
        movement.SetBoundaries(minPosition, maxPosition);
        newGameObj.SetActive(true);
        
        return particle;
    }
    
}
