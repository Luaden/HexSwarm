using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattlefieldManager : IBattlefield
{
    
    protected Dictionary<Vector3Int, Cell> world;

    public Dictionary<Vector3Int, Cell> World { get => world; }
    
    public BattlefieldManager(Dictionary<Vector3Int, Cell> world)
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
