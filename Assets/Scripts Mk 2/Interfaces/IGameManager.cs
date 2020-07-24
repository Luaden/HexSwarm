using UnityEngine;
using System.Collections.Generic;

public interface IGameManager
{
    int TurnCounter { get; }
    int LevelCounter { get; }

    Queue<ITeam> ActiveTeams { get; }

    bool NewGame();
    bool StartLevel();
    bool EndTurn();

    IUnit DisplayedUnit { get; }
    IAbility SelectedAbility { get; }
    void InspectUnitUnderMouse();
    bool PerformMove(IUnit unit, IAbility ablity, Direction direction, Vector3Int target, IEnumerable<Vector3Int> path = default);
}
