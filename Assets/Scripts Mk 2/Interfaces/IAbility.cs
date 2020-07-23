using UnityEngine;
using System.Collections.Generic;

public interface IAbility
{
    Abilitys ID { get; }
    string Name { get; }
    string Description { get; }
    Sprite Icon { get; }
    IMoveHighlight DefaultRanges { get; }
    int MovementRange { get; }
    bool IsJump { get; }
    bool IsSpawn { get; }
    IEnumerable<ICell> GetAttack(Direction direction, Vector3Int origin);
}

