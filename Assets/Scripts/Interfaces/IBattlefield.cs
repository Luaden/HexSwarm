using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattlefield
{
    void PlaceNewUnit(IUnit unit, Vector3Int position);
    void MoveUnit(Vector3Int from, Vector3Int to);
    void DestroyUnits(Vector3Int Vector2);
}
