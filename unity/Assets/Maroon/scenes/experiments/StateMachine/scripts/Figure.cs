using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
public class Figure : MonoBehaviour
{
    public Player _player {get;set;}
    [SerializeField] Moves _possibleMoves;
    public int _positionRow {get;set;}
    public int _positionColumn {get;set;}
}
