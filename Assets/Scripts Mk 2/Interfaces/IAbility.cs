using UnityEngine;
using System.Collections.Generic;

public interface IAbility
{
    IMoveHighlight DefaultRanges { get; } 
    int movementRange { get; }
    bool IsJump { get; }
    IEnumerable<Vector3Int> GetAttack(Direction direction, Vector3Int origin);
}

