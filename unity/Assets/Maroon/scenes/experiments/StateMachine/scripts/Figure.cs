using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
public class Figure : MonoBehaviour
{
    [SerializeField] Player _player;
    [SerializeField] Moves _possibleMoves;
    private int _positionX {get;set;}
    private int _positionY {get;set;}
}
