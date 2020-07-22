using UnityEngine;
using System.Collections.Generic;

public interface IAbility
{
    string Name { get; }
    string Description { get; }
    Sprite Icon { get; }
    IMoveHighlight DefaultRanges { get; }
    int MovementRange { get; }
    bool IsJump { get; }
    IEnumerable<ICell> GetAttack(Direction direction, Vector3Int origin);
}

