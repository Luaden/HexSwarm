using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattlefield
{
    void PlaceNewUnit(IUnit unit, Vector3Int position);
    bool MoveUnit(Vector3Int unitPosition, Vector3Int destination, ITeam team);
    void DestroyUnits(Vector3Int unitPosition);
}
