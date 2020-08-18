using System;
using Antares.Evaluation.LearningContent;
using TMPro;
using UnityEngine;

public class SortingMachine : MonoBehaviour
{
    public enum SortingMachineState
    {
        SMS_Pause,
        //Insert Operation
        SMS_ToSource,
        SMS_SourceDown,
        SMS_SourceUp,
        SMS_ToDestination,
        SMS_DestinationDown,
        SMS_DestinationUp,
        //Swap Operation
        SMS_ElementDisappear,
        SMS_ElementAppear,
        SMS_ElementFinish
    }
    
    public SortingMachineState sortingState = SortingMachineState.SMS_Pause;
    public SortingLogic sortingLogic;
    
    [Header("Speed Settings")]
    [Range(1f, 10f)]
    public float timePerMove = 3f;

    [Header("Highlight Settings")] 
    public GameObject highlighter;
    
    [Header("Grapper Settings")] 
    public TextMeshPro displayText;
    public GameObject bigGrapper;
    public float bigGrapperStart = 0;
    public float bigGrapperEnd = -0.2f;
    public GameObject middleGrapper;
    public float middleGrapperStart = 0.65f;
    public float middleGrapperEnd = 0.5f;
    public GameObject smallGrapper;
    public float smallGrapperStart = 0f;
    public float smallGrapperEnd = -0.45f;
    public GameObject grapperPointer;


    private float _waitTime = -1f;
    private int _sourceIdx;
    private int _destinationIdx;
    private GameObject _movingElement;
    private float _currentDistancePerSecond;

    // Update is called once per frame
    void Update()
    {
        if (_waitTime > 0f) {
            _waitTime -= Time.deltaTime;
            return;
        }
        
        switch (sortingState)
        {
            case SortingMachineState.SMS_Pause: //to nothing as we pause
                break;
            case SortingMachineState.SMS_ToSource:
            {
                if (moveHorizontal(sortingLogic.ArrayPlaces[_sourceIdx].transform.position.z, _currentDistancePerSecond))
                {
                    sortingState = SortingMachineState.SMS_SourceDown;
                    
                    var wholeDistance = Mathf.Abs(sortingLogic.ArrayPlaces[_sourceIdx].sortElement.transform.position.y - grapperPointer.transform.TransformPoint(Vector3.zero).y);
                    _currentDistancePerSecond = wholeDistance / (timePerMove * 0.15f);
                }
            } break;
            case SortingMachineState.SMS_SourceDown:
            {
                if (GoDown(sortingLogic.ArrayPlaces[_sourceIdx].sortElement.transform.position.y,
                    _currentDistancePerSecond))
                {
                    //Grab Source
                    highlighter.SetActive(true);
                    _movingElement = sortingLogic.ArrayPlaces[_sourceIdx].sortElement;
                    sortingLogic.ArrayPlaces[_sourceIdx].sortElement = null;
                    _movingElement.transform.parent = smallGrapper.transform;

                    sortingLogic.RearrangeArrayElements(timePerMove * 0.15f);
                    sortingState = SortingMachineState.SMS_SourceUp;
                }
            } break;
            case SortingMachineState.SMS_SourceUp:
                if (GoUp(_currentDistancePerSecond))
                {
                    sortingState = SortingMachineState.SMS_ToDestination;
                    
                    var wholeDistance = Mathf.Abs(sortingLogic.ArrayPlaces[_destinationIdx].transform.position.z- grapperPointer.transform.position.z);
                    _currentDistancePerSecond = wholeDistance / (timePerMove * 0.3f);
                } break;
            case SortingMachineState.SMS_ToDestination:
                if (moveHorizontal(sortingLogic.ArrayPlaces[_destinationIdx].transform.position.z, _currentDistancePerSecond))
                {
                    sortingState = SortingMachineState.SMS_DestinationDown;
                    sortingLogic.MakePlaceInArray(_destinationIdx, timePerMove * 0.15f);
                    
                    var wholeDistance = Mathf.Abs(sortingLogic.ArrayPlaces[_destinationIdx].elementPlace.transform.position.y 
                                                  + _movingElement.GetComponent<SortingElement>().size / 2f 
                                                  - grapperPointer.transform.TransformPoint(Vector3.zero).y);
                    _currentDistancePerSecond = wholeDistance / (timePerMove * 0.15f);
                }
                break;
            case SortingMachineState.SMS_DestinationDown:
                if (GoDown(sortingLogic.ArrayPlaces[_destinationIdx].elementPlace.transform.position.y
                           + _movingElement.GetComponent<SortingElement>().size / 2f, _currentDistancePerSecond))
                {
                    //Place Element At Destination
                    highlighter.SetActive(false);
                    sortingLogic.ArrayPlaces[_destinationIdx].SetSortElement(_movingElement, 1f);
                    _movingElement = null;
                    sortingState = SortingMachineState.SMS_DestinationUp;
                }
                break;
            case SortingMachineState.SMS_DestinationUp:
                if (GoUp(_currentDistancePerSecond))
                {
                    sortingState = SortingMachineState.SMS_Pause;
                    sortingLogic.MoveFinished();
                } break;
            case SortingMachineState.SMS_ElementDisappear:
            {
                _waitTime = timePerMove / 2f;
                sortingLogic.ArrayPlaces[_sourceIdx].Highlight(true);
                sortingLogic.ArrayPlaces[_destinationIdx].Highlight(true);
                sortingLogic.ArrayPlaces[_sourceIdx].StartDisappear(_waitTime);
                sortingLogic.ArrayPlaces[_destinationIdx].StartDisappear(_waitTime);
                sortingState = SortingMachineState.SMS_ElementAppear;
            } break;
            case SortingMachineState.SMS_ElementAppear:
            {
                var tmp = sortingLogic.ArrayPlaces[_sourceIdx].sortElement;
                sortingLogic.ArrayPlaces[_sourceIdx].SetSortElement(sortingLogic.ArrayPlaces[_destinationIdx].sortElement, -1f);
                sortingLogic.ArrayPlaces[_destinationIdx].SetSortElement(tmp, -1f);
                
                _waitTime = timePerMove / 2f;
                sortingLogic.ArrayPlaces[_sourceIdx].StartAppear(_waitTime);
                sortingLogic.ArrayPlaces[_destinationIdx].StartAppear(_waitTime);
                sortingState = SortingMachineState.SMS_ElementFinish;
            } break;
            case SortingMachineState.SMS_ElementFinish:
                sortingLogic.ArrayPlaces[_sourceIdx].Highlight(false);
                sortingLogic.ArrayPlaces[_destinationIdx].Highlight(false);
                sortingLogic.MoveFinished();
                sortingState = SortingMachineState.SMS_Pause;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool Insert(int fromIdx, int toIdx)
    {
        if (sortingState != SortingMachineState.SMS_Pause) return false;

        _sourceIdx = fromIdx;
        _destinationIdx = toIdx;
        sortingState = SortingMachineState.SMS_ToSource;
        
        var wholeDistance = Mathf.Abs(sortingLogic.ArrayPlaces[_sourceIdx].sortElement.transform.position.z- grapperPointer.transform.position.z);
        _currentDistancePerSecond = wholeDistance / (timePerMove * 0.3f);
        return true;
    }

    public bool Swap(int element1, int element2)
    {
        if (sortingState != SortingMachineState.SMS_Pause) return false;

        _sourceIdx = element1;
        _destinationIdx = element2;
        sortingState = SortingMachineState.SMS_ElementDisappear;
        return true;
    }

    public bool Compare(int element1, int element2)
    {
        //TODO: Animation with highlight and finish (so it takes same time as other operations)
        sortingLogic.MoveFinished();
        return true;
    }

    private bool GoDown(float sourceY, float distancePerSecond) //returns if the sourceY was reached
    {
        var distance = Mathf.Abs(sourceY - grapperPointer.transform.position.y);
        var retValue = false;

        if (distance > distancePerSecond * Time.deltaTime)
            distance = distancePerSecond * Time.deltaTime;
        else
        {
            //next thing will be grapping the source
            retValue = true;
        }

        //biggest grapper
        var pos = bigGrapper.transform.localPosition;
        var diffY = pos.y - bigGrapperEnd;
        if (diffY >= 0)
        {
            pos.y -= diffY > distance ? distance : diffY;
            distance -= diffY > distance ? distance : diffY;
            bigGrapper.transform.localPosition = pos;
        }
                
        //middle grapper
        pos = middleGrapper.transform.localPosition;
        diffY = pos.y - middleGrapperEnd;
        if (diffY >= 0)
        {
            pos.y -= diffY > distance ? distance : diffY;
            distance -= diffY > distance ? distance : diffY;
            middleGrapper.transform.localPosition = pos;
        }
                
        //small grapper
        pos = smallGrapper.transform.localPosition;
        diffY = pos.y - smallGrapperEnd;
        if (diffY >= 0)
        {
            pos.y -= diffY > distance ? distance : diffY;
            distance -= diffY > distance ? distance : diffY;
            smallGrapper.transform.localPosition = pos;
        }

        return retValue;
    }

    private bool GoUp(float distancePerSecond)
    {
        var madeDistance = 0f;
        var allowedDistance = distancePerSecond * Time.deltaTime;
        var retvalue = true;
        
        //small grapper
        var pos = smallGrapper.transform.localPosition;
        var diffY = smallGrapperStart - pos.y;

        if (diffY > allowedDistance)
        {
            diffY = allowedDistance;
            allowedDistance = -1f;
        }
        else
        {
            allowedDistance -= diffY;
        }

        if (diffY >= 0)
        {
            pos.y = pos.y + diffY > smallGrapperStart ? smallGrapperStart : pos.y + diffY;
            smallGrapper.transform.localPosition = pos;
        }

        if (allowedDistance < 0f) return false;
        
        //middleGrapper
        pos = middleGrapper.transform.localPosition;
        diffY = middleGrapperStart - pos.y;

        if (diffY > allowedDistance)
        {
            diffY = allowedDistance;
            allowedDistance = -1f;
        }

        if (diffY >= 0)
        {
            pos.y = pos.y + diffY > middleGrapperStart ? middleGrapperStart : pos.y + diffY;
            middleGrapper.transform.localPosition = pos;
        }

        if (allowedDistance < 0f) return false;
        
        //bigGrapper
        pos = bigGrapper.transform.localPosition;
        diffY = bigGrapperStart - pos.y;

        if (diffY > allowedDistance)
        {
            diffY = allowedDistance;
            allowedDistance = -1f;
        }

        if (diffY >= 0)
        {
            pos.y = pos.y + diffY > bigGrapperStart ? bigGrapperStart : pos.y + diffY;
            bigGrapper.transform.localPosition = pos;
        }

        if (allowedDistance < 0f) return false;

        return true;
    }

    private bool moveHorizontal(float sourceZ, float distancePerSecond)
    {
        var newPos = transform.position;
        var retValue = false;
        
        if (newPos.z < sourceZ)
        {
            newPos.z += distancePerSecond * Time.deltaTime;
            if (newPos.z >= sourceZ)
            {
                newPos.z = sourceZ;
                retValue = true;
            }
        }
        else
        {
            newPos.z -= distancePerSecond * Time.deltaTime;
            if (newPos.z <= sourceZ)
            {
                newPos.z = sourceZ;
                retValue = true;
            }
        }
        transform.position = newPos;
        return retValue;
    }

    public void setSwapsOperations(int swaps, int operations)
    {
        displayText.text = "<style=\"header\">Count:</style>\nOperations:\n<align=\"right\">"
                           + operations
                           + "</align>\nSwaps:\n<align=\"right\">"
                           + swaps
                           + "</align>";
    }
}
