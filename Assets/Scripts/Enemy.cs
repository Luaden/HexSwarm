using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Enemy after having StartTurnCalled will for each unit
/// randomly pick to call the AIUnit's best or random move
/// </summary>
public class Enemy : Team
{

    [SerializeField] IGameManager gameManager;
    [SerializeField] IGrid gridManager;

    

}
