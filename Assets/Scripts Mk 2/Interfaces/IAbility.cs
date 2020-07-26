using UnityEngine;
using System.Collections.Generic;

public interface IAbility
{
    Abilitys ID { get; }
    string Name { get; }
    string Description { get; }
    Sprite Icon { get; }
    int MovementRange { get; }
    bool IsJump { get; }
    bool IsSpawn { get; }
    bool IsSpawnVoid { get; }
    bool NeedsClearLand { get; }
    IEnumerable<ICell> GetMoves(Vector3Int gridOrigin, Direction direction = Direction.Zero);
    IEnumerable<ICell> GetAttack(Direction direction, Vector3Int origin);
}

