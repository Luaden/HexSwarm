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

    //IUnit DisplayedUnit { get; }
    //void InspectUnitUnderMouse();
    //bool PerformMove(IUnit unit, IPosAbilityDefault ablity, Vector3Int target);

    //void AnimateMove();
    //Vector3Int GetMousePosition();
}

