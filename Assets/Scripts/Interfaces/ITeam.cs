using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public interface ITeam
{
    string Name { get; }
    string Description { get; }
    Sprite Icon { get; }
    TileBase Tile { get; }
    Color Color { get; }
    Teams Type { get; }
    bool HasMove { get; }
    IGameManager GameManager { get; }
    IEnumerable<IUnit> Units { get;}
    IEnumerable<Vector3Int> HighlightMove { get; }
    IEnumerable<Vector3Int> HighlightAttack { get; }
    IEnumerable<Vector3Int> HighlightOverlap { get; }

    bool HasUnitsAfterLosses(IEnumerable<IUnit> units);
    void StartTurn();
    void Undo();
    void EndTurn();
    void DoMove(IUnit unit, IPosAbilityDefault ablity, Vector3Int target);
    void ResolveHighlight(IUnit unit, IPosAbilityDefault ablity, Vector3Int target);
}

