using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SortingElement : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    
    private int _value;
    private MeshRenderer _meshRenderer;
    private float _size;

    private float _maxSize;
    private Vector3 _centerPosition;
    
    private Color _defaultColor = new Color(0.5f, 0, 0);
    private Color _grayColor = new Color(0.5f, 0.5f, 0.5f);

    private enum AnimationState
    {
        AS_None,
        AS_FadeOut,
        AS_FadeIn,
        AS_MoveOutRight,
        AS_MoveOutLeft,
        AS_MoveInRight,
        AS_MoveInLeft
    }

    private AnimationState _animationState;
    private float _animationStartTime;
    private float _animationEndTime;

    private Vector3 _moveOffset;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _maxSize = transform.localScale.x;
        _centerPosition = transform.localPosition;
    }

    private void Update()
    {
        LookAtCamera();

        switch (_animationState)
        {
            case AnimationState.AS_None:
                break;
            case AnimationState.AS_FadeOut:
                float t1 = CalculateT();
                float fadeSize1 = _size * (1 - t1);
                transform.localScale = new Vector3(fadeSize1, fadeSize1, fadeSize1);
                break;
            case AnimationState.AS_FadeIn:
                float t2 = CalculateT();
                float fadeSize2 = _size * t2;
                transform.localScale = new Vector3(fadeSize2, fadeSize2, fadeSize2);
                break;
            case AnimationState.AS_MoveOutRight:
                float t3 = CalculateT();
                transform.localPosition = _centerPosition + t3 * _moveOffset;
                break;
            case AnimationState.AS_MoveOutLeft:
                float t4 = CalculateT();
                transform.localPosition = _centerPosition - t4 * _moveOffset;
                break;
            case AnimationState.AS_MoveInRight:
                float t5 = CalculateT();
                transform.localPosition = _centerPosition + (1 - t5) * _moveOffset;
                break;
            case AnimationState.AS_MoveInLeft:
                float t6 = CalculateT();
                transform.localPosition = _centerPosition - (1 - t6) * _moveOffset;
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private float CalculateT()
    {
        float t = (Time.time - _animationStartTime) / (_animationEndTime - _animationStartTime);
        if (t > 1)
        {
            t = 1.0f;
            _animationState = AnimationState.AS_None;
        }

        return t;
    }

    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            text.text = value.ToString();
            
            foreach (var mat in _meshRenderer.materials)
                mat.color = _defaultColor;
            
            _size = (0.4f + 0.6f * value / 99f) * _maxSize;
            transform.localScale = new Vector3(_size,_size,_size);
        }
    }

    private void LookAtCamera()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
    
    public void FadeOutSeconds(float seconds)
    {
        _animationState = AnimationState.AS_FadeOut;
        _animationStartTime = Time.time;
        _animationEndTime = _animationStartTime + seconds;
        
        transform.localScale = new Vector3(_size,_size,_size);
        transform.localPosition = _centerPosition;
    }
    
    public void FadeInSeconds(float seconds)
    {
        _animationState = AnimationState.AS_FadeIn;
        _animationStartTime = Time.time;
        _animationEndTime = _animationStartTime + seconds;
        
        transform.localScale = new Vector3(0,0,0);
        transform.localPosition = _centerPosition;
    }
    
    public void MoveOutRight(float seconds, Vector3 offset)
    {
        _animationState = AnimationState.AS_MoveOutRight;
        _animationStartTime = Time.time;
        _animationEndTime = _animationStartTime + seconds;

        _moveOffset = offset;
    }
    
    public void MoveOutLeft(float seconds, Vector3 offset)
    {
        _animationState = AnimationState.AS_MoveOutLeft;
        _animationStartTime = Time.time;
        _animationEndTime = _animationStartTime + seconds;

        _moveOffset = offset;
    }
    
    public void MoveInRight(float seconds, Vector3 offset)
    {
        _animationState = AnimationState.AS_MoveInRight;
        _animationStartTime = Time.time;
        _animationEndTime = _animationStartTime + seconds;

        _moveOffset = offset;
        transform.localPosition = _centerPosition + offset;
        transform.localScale = new Vector3(_size,_size,_size);
    }
    
    public void MoveInLeft(float seconds, Vector3 offset)
    {
        _animationState = AnimationState.AS_MoveInLeft;
        _animationStartTime = Time.time;
        _animationEndTime = _animationStartTime + seconds;

        _moveOffset = offset;
        transform.localPosition = _centerPosition - offset;
        transform.localScale = new Vector3(_size,_size,_size);
    }

    public void SetDefaultColor()
    {
        foreach (var mat in _meshRenderer.materials)
            mat.color = _defaultColor;
    }

    public void SetGrayColor()
    {
        foreach (var mat in _meshRenderer.materials)
            mat.color = _grayColor;
    }

    public void ResetVisualization()
    {
        SetDefaultColor();
        _animationState = AnimationState.AS_None;
        transform.localPosition = _centerPosition;
        transform.localScale = new Vector3(_size,_size,_size);
    }
}
