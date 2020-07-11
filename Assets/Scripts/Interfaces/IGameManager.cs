using UnityEngine;
using System.Collections.Generic;

public interface IGameManager
{
    int TurnCounter { get; }
    int LevelCounter { get; }
    
    Queue<ITeam> ActiveTeams { get; }


    void NewGame();
    void StartLevel();
    void EndTurn();

    IUnit DisplayedUnit { get; }
    void InspectUnitAt(Vector3Int location);

    void AnimateMove();
}

