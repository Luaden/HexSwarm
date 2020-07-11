using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldManager : MonoBehaviour, IBattlefield
{
    protected HashSet<Cell> world;
    
    public BattlefieldManager(HashSet<Cell> world)
    {
        this.world = world;
    }

    public void DestroyUnits(Vector3Int Vector2)
    {
        throw new System.NotImplementedException();
    }

    public void MoveUnit(Vector3Int from, Vector3Int to)
    {
        throw new System.NotImplementedException();
    }

    public void PlaceNewUnit(IUnit unit, Vector3Int position)
    {
        throw new System.NotImplementedException();
    }
}
