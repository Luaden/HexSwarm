using UnityEngine;
using System.Collections.Generic;

public interface IGameManager
{
    int turnCounter { get; }
    int levelCounter { get; }
    
    Queue<ITeam> ActiveTeams { get; }


    void NewGame();
    void StartLevel();
    void EndTurn();

    IUnit DisplayedUnit { get; }
    void InspectUnitAt(Vector3Int location);

    void AnimateMove();
}

