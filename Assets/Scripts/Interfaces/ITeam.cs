using UnityEngine;
using System.Collections.Generic;

public interface ITeam
{
    string Name { get; }
    string Discription { get; }
    Sprite Icon { get; }
    Color Color { get; }
    Teams Type { get; }

    IGameManager Game { get; }
    IEnumerable<IUnit> Units { get;}
    bool hasUnitsAfterLosses(IEnumerable<IUnit> losses);
    bool hasMove { get; }
    void StartTurn();
    void Undo();
    void EndTurn();

    IEnumerable<Vector3Int> HighlightMove { get; }
    IEnumerable<Vector3Int> HighlightAttack { get; }
    IEnumerable<Vector3Int> HighlightOverlap { get; }

    void DoMove(IUnit unit, IPosAbilityDefault ablity, Vector3Int target);
    void ResolveHighlight(IUnit unit, IPosAbilityDefault ablity, Vector3Int target);
}

