using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class BattlefieldManager : IBattlefield
{
    //Cached references
    GridManager gridManager;

    //State variables
    protected Dictionary<Vector3Int, Cell> world;
    protected IUnit currentUnit;
    protected Cell fromCell;
    protected Cell toCell;
    protected Cell tempCell;

    //Properties
    public Dictionary<Vector3Int, Cell> World { get => world; }
    
    public BattlefieldManager(Dictionary<Vector3Int, Cell> world, GridManager gridManager)
    {
        this.world = world;
        this.gridManager = gridManager;
    }

    public void DestroyUnits(Vector3Int Vector2)
    {
        throw new System.NotImplementedException();
    }

    public void MoveUnit(Vector3Int from, Vector3Int to)
    {
        world.TryGetValue(from, out fromCell);
        world.TryGetValue(to, out toCell);

        toCell.Unit = fromCell.Unit;
        (toCell.Unit as Unit).Location = to;
        fromCell.Unit = default;        
        
        gridManager.PaintUnitTile(fromCell.Position, default);
        gridManager.PaintUnitTile(toCell.Position, toCell.Unit.Tile);
    }

    public void PlaceNewUnit(IUnit unit, Vector3Int position)
    {
        toCell = null;
        world.TryGetValue(position, out toCell);

        if (toCell.Unit != null)
            return;

        toCell = new Cell(toCell.Position, unit, toCell.Tile);

        world.Remove(toCell.Position);
        world.Add(toCell.Position, toCell);

        gridManager.PaintUnitTile(toCell.Position, toCell.Unit.Tile);
    }
}
