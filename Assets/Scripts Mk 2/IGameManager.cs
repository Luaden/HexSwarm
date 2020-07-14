using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public interface IGameManager
{
    int Level { get; }
    int Turn { get; }
    Queue<ITeam> TeamList { get; }
    /// <summary>
    /// Setup for hotseat or just a quickplay
    /// </summary>
    int HumanPlayers { get; }
    /// <summary>
    /// Called In order to setup a gameboard
    /// Starts the game for the first time
    /// </summary>
    void StartLevel();

    void NewGame();
    void EndTurn();

    IUnit DisplayedUnit { get; }
    void InspectUnitUnderMouse();

    bool PerformMove(IUnit unit, IAbility ablity, Vector3Int target);
}

