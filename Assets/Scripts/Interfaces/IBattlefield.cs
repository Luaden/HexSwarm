using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattlefield
{
    void PlaceNewUnit(IUnit unit, Vector3Int position);
    void MoveUnit(Vector3Int unitPosition, Vector3Int destination);
    void DestroyUnits(Vector3Int unitPosition);
}
