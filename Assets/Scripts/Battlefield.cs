using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlefield : MonoBehaviour, IBattlefield
{
    protected HashSet<Cell> world;
    
    public Battlefield(HashSet<Cell> world)
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
